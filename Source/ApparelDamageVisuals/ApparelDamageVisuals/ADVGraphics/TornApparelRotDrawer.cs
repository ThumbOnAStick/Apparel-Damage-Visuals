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
    internal class TornApparelRotDrawer
    {
        float durabilityCached;
        Material materialCached;

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

        private static void CopyIfPresent(Material src, Material dst, string prop)
        {
            if (src.HasProperty(prop)) dst.SetColor(prop, src.GetColor(prop));
        }

        private Material BuildTornMaterial(Material baseMat, int seed, IntRange holeCount)
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

        FloatRange HoleSize(float scale)
        {
            var minSize = (1 - this.durabilityCached) * (scale / 7);
            var maxSize = minSize * 1.5f;
            return new FloatRange(minSize, maxSize);
        }

        private Texture2D CloneAndStampAlpha(Texture2D src, int seed, IntRange holeCount)
        {
            if (src == null) return null;

            // Must be on main thread for all operations below
            if (!UnityData.IsInMainThread) return null;

            int w = src.width; int h = src.height;

            // Create a readable copy (handles non-readable textures)
            Texture2D readableTex;
            if (src.isReadable)
            {
                readableTex = UnityEngine.Object.Instantiate(src);
            }
            else
            {
                // This path touches the GPU; main-thread only (guarded above).
                RenderTexture tmp = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                Graphics.Blit(src, tmp);

                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = tmp;

                readableTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                readableTex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                readableTex.Apply();

                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(tmp);
            }

            var pixels = readableTex.GetPixels32();

            // Compute opaque (colored) bounds from alpha channel
            int minX, minY, maxX, maxY;
            bool hasOpaque = ComputeOpaqueBounds(pixels, w, h, 8, out minX, out minY, out maxX, out maxY);
            if (!hasOpaque)
            {
                // Fallback to full texture if fully transparent
                minX = 0; minY = 0; maxX = w - 1; maxY = h - 1;
            }

            // Use thread-local PRNG: deterministic and thread-safe
            var rng = new System.Random(unchecked(seed * 73856093 ^ 19349663));

            // Create shapes confined to the opaque bounds
            var shapes = CreateHoleShapes(w, h, minX, minY, maxX, maxY, holeCount, rng);

            foreach (var s in shapes)
            {
                if (s is Circle c)
                {
                    ShapeHelper.DrawTransparentCircle(ref pixels, c, w, h);
                }
                else
                {
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

            readableTex.SetPixels32(pixels);
            readableTex.Apply(false, false);
            return readableTex;
        }

        private static bool ComputeOpaqueBounds(Color32[] pixels, int w, int h, byte alphaThreshold,
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

        private IEnumerable<Shape> CreateHoleShapes(int w, int h, int minX, int minY, int maxX, int maxY, IntRange holeCount, System.Random rng)
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
                // Size relative to content bounds
                float r = holeSize.min + (float)rng.NextDouble() * (holeSize.max - holeSize.min);
                int x = minX + rng.Next(Math.Max(1, maxX - minX + 1));
                int y = minY + rng.Next(Math.Max(1, maxY - minY + 1));
                yield return new Circle(x, y, w, h, r);
            }
        }
    }
}
