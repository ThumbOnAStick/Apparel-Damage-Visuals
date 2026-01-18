using ApparelDamageVisuals.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals.ADVGraphics
{
    /// <summary>
    /// Drawer for broken clothes
    /// </summary>
    internal class TornApparelRotDrawer
    {
        float durabilityCached;
        Material materialCached;
        protected enum EdgeDirection {North, South, East, West}

        public TornApparelRotDrawer()
        {
            durabilityCached = 0;
        }

        bool CanRenderNow()
        {
            return ApparelDamageVisualsMod.Settings.MaxCameraZoom > Find.Camera.orthographicSize;
        }

        public Material GetMaterial(Material baseMat, int seed, IntRange holeCount, float durability)
        {

            // Use cached material as piority
            if (this.materialCached != null && durabilityCached == durability)
            {
                return this.materialCached;
            }

            // Avoid calling in main thread or when camera is too far away
            if (!UnityData.IsInMainThread || !CanRenderNow())
            {
                return baseMat;
            }


            this.durabilityCached = durability;
            // Render Torn Material on main thread only
            return this.materialCached = BuildTornMaterial(baseMat, seed, holeCount);
        }

        protected void CopyIfPresent(Material src, Material dst, string prop)
        {
            if (src.HasProperty(prop)) dst.SetColor(prop, src.GetColor(prop));
        }

        protected Material BuildTornMaterial(Material baseMat, int seed, IntRange holeCount)
        {
            var shader = baseMat.shader;
            var newMat = new Material(shader);

            // Copy colors
            if (baseMat.HasProperty(ShaderPropertyIDs.Color)) newMat.color = baseMat.color;
            CopyIfPresent(baseMat, newMat, "_ColorTwo");
            CopyIfPresent(baseMat, newMat, "_ColorThree");

            // Copy maskTex as-is
            if (baseMat.HasTexture("_MaskTex"))
            {
                var mask = baseMat.GetTexture("_MaskTex");
                if (mask != null) newMat.SetTexture("_MaskTex", mask);
            }

            // Clone and stamp mainTex
            var main = baseMat.mainTexture as Texture2D;
            var stamped = CloneAndStampAlpha(main, seed, holeCount);
            if (stamped != null)
                newMat.mainTexture = stamped;
            else
                newMat.mainTexture = baseMat.mainTexture;

            // Match render queue and keywords
            newMat.renderQueue = baseMat.renderQueue;
            foreach (var kw in baseMat.shaderKeywords) newMat.EnableKeyword(kw);

            return newMat;
        }

        protected FloatRange HoleSize(float scale)
        {
            var minSize = (1 - this.durabilityCached) * (scale / 7);
            var maxSize = minSize * 1.5f;
            return new FloatRange(minSize, maxSize);
        }

        protected Texture2D CloneAndStampAlpha(Texture2D src, int seed, IntRange holeCount)
        {
            if (src == null) return null;

            // Must be on main thread for all operations below
            if (!UnityData.IsInMainThread) return null;

            int w = src.width; int h = src.height;

            // Create a readable copy (handles non-readable textures)
            Texture2D readableTex;
            try
            {
                if (src.isReadable && IsFormatSupported(src.format))
                {
                    readableTex = UnityEngine.Object.Instantiate(src);
                }
                else
                {
                    RenderTexture tmp = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                    Graphics.Blit(src, tmp);

                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = tmp;

                    readableTex = new Texture2D(w, h, TextureFormat.ARGB32, false);
                    readableTex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                    readableTex.Apply();

                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(tmp);
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"[ApparelDamageVisuals] Failed to create readable texture: {ex.Message}");
                return null;
            }

            Color32[] pixels;
            try
            {
                pixels = readableTex.GetPixels32();
            }
            catch (Exception ex)
            {
                Log.Warning($"[ApparelDamageVisuals] Failed to read pixels: {ex.Message}");
                UnityEngine.Object.Destroy(readableTex);
                return null;
            }

            bool hasOpaque = ComputeOpaqueBounds(pixels, w, h, 8, out int minX, out int minY, out int maxX, out int maxY);
            if (!hasOpaque)
            {
                minX = 0; minY = 0; maxX = w - 1; maxY = h - 1;
            }

            var rng = new System.Random(unchecked(seed * 73856093 ^ 19349663));

            var shapes = CreateHoleShapes(w, h, minX, minY, maxX, maxY, holeCount, rng);

            foreach (var s in shapes)
            {
                if (s is Circle c)
                {
                    ShapeHelper.DrawTransparentCircle(ref pixels, c, w, h);
                }
                else if (s is Triangle t)
                {
                    ShapeHelper.DrawTransparentTriangle(ref pixels, t, w, h);
                }
                else
                {
                    // Fallback for any other shapes
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        int x = i % w;
                        int y = i / w;
                        if (s.IsCoordinateInShape(x, y))
                        {
                            var p = pixels[i];
                            p.a = 0;
                            pixels[i] = p;
                        }
                    }
                }
            }

            try
            {
                readableTex.SetPixels32(pixels);
                readableTex.Apply(false, false);
            }
            catch (Exception ex)
            {
                Log.Warning($"[ApparelDamageVisuals] Failed to apply pixels: {ex.Message}");
                UnityEngine.Object.Destroy(readableTex);
                return null;
            }

            return readableTex;
        }

        private static bool IsFormatSupported(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.ARGB32:
                case TextureFormat.RGBA32:
                case TextureFormat.BGRA32:
                case TextureFormat.RGB24:
                case TextureFormat.Alpha8:
                    return true;
                default:
                    // Compressed formats (DXT, ETC, ASTC, etc.) don't support GetPixels32 directly
                    return false;
            }
        }

        protected bool ComputeOpaqueBounds(Color32[] pixels, int w, int h, byte alphaThreshold,
                                              out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = w; minY = h; maxX = -1; maxY = -1;

            for (int y = 0; y < h; y++)
            {
                int row = y * w;
                for (int x = 0; x < w; x++)
                {
                    if (pixels[row + x].a > alphaThreshold)
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            return maxX >= 0;
        }

    
        protected virtual IEnumerable<Shape> CreateHoleShapes(int w, int h, int minX, int minY, int maxX, int maxY, IntRange holeCount, System.Random rng)
        {
            int numberOfHoles = holeCount.min <= holeCount.max
                ? holeCount.min + rng.Next(holeCount.max - holeCount.min + 1)
                : holeCount.min;

            int boundsW = Math.Max(1, maxX - minX + 1);
            int boundsH = Math.Max(1, maxY - minY + 1);
            float scale = Math.Min(boundsW, boundsH);

            var holeSize = HoleSize(scale);
            for (int i = 0; i < numberOfHoles; i++)
            {
                float r = holeSize.min + (float)rng.NextDouble() * (holeSize.max - holeSize.min);
                r *= ApparelDamageVisualsMod.Settings.Size;
                EdgeDirection edgeDirection = (EdgeDirection)rng.Next(4);

                int realMinX = minX;
                int realMaxX = maxX;
                int realMinY = minY;
                int realMaxY = maxY;
                switch(edgeDirection)
                {
                    case EdgeDirection.West: // West
                        realMaxX = minX + (int)r;
                        break;
                    case EdgeDirection.East: // East
                        realMinX = maxX - (int)r; 
                        break;
                    case EdgeDirection.North: // North
                        realMaxY = minY + (int)r;
                        break;
                    case EdgeDirection.South: // South
                        realMinY = maxY - (int)r;
                        break;  
                }
                int x = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                int y = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                yield return new Circle(x, y, w, h, r);

            }
        }
    }
}
