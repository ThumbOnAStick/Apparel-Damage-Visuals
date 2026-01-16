using ApparelDamageVisuals.ADVGraphics;
using System;
using System.Configuration;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals.Utils
{
    public class ShapeHelper
    {
        /// <summary>
        /// Draws an antialiased transparent circle
        /// </summary>
        public static void DrawTransparentCircle(ref Color32[] pixels, Circle circle, int w, int h)
        {
            // Rasterize only the bounding box of the circle with antialiasing
            int cx = circle.CX;
            int cy = circle.CY;
            float r = circle.Radius;
            float r2 = r * r;

            // Expand bounds by 1 pixel for antialiasing
            int minX = Math.Max(0, circle.MinX - 1);
            int maxX = Math.Min(w - 1, circle.MaxX + 1);
            int minY = Math.Max(0, circle.MinY - 1);
            int maxY = Math.Min(h - 1, circle.MaxY + 1);

            for (int yy = minY; yy <= maxY; yy++)
            {
                int dy = yy - cy;
                int dy2 = dy * dy;
                int rowStart = yy * w;

                for (int xx = minX; xx <= maxX; xx++)
                {
                    int dx = xx - cx;
                    float distanceSquared = dx * dx + dy2;
                    float distance = Mathf.Sqrt(distanceSquared);
                    
                    int idx = rowStart + xx;
                    var p = pixels[idx];
                    
                    if (distance <= r - 0.5f)
                    {
                        // Fully inside circle - completely transparent
                        p.a = 0;
                    }
                    else if (ApparelDamageVisualsMod.Settings.Antialiasing && distance <= r + 0.5f + ApparelDamageVisualsMod.Settings.Thickness)
                    {
                        // Edge pixels - antialiased
                        float coverage = r + 0.5f - distance;
                        coverage = Mathf.Clamp01(coverage);
                        
                        // Blend alpha: reduce existing alpha by coverage amount
                        float newAlpha = p.a * (1f - coverage);
                        if (ApparelDamageVisualsMod.Settings.Outline)
                        {
                            p = SetBlack(p);
                        }
                        p.a = (byte)Mathf.RoundToInt(newAlpha);
                    }
                    // Outside circle - no change
                    
                    pixels[idx] = p;
                }
            }
        }

        /// <summary>
        /// Draws an antialiased transparent triangle using scanline rasterization
        /// </summary>
        public static void DrawTransparentTriangle(ref Color32[] pixels, Triangle triangle, int w, int h)
        {
            // Get triangle vertices for optimized scanline rasterization
            triangle.GetVertices(out int x1, out int y1, out int x2, out int y2, out int x3, out int y3);
            
            // Use antialiased scanline rasterization
            RasterizeTriangleScanlineAntialiased(ref pixels, w, h, x1, y1, x2, y2, x3, y3);
        }

        private static void RasterizeTriangleScanlineAntialiased(ref Color32[] pixels, int w, int h, 
            int x1, int y1, int x2, int y2, int x3, int y3)
        {
            // Sort vertices by Y coordinate (bubble sort for 3 elements)
            if (y1 > y2) { Swap(ref x1, ref x2); Swap(ref y1, ref y2); }
            if (y2 > y3) { Swap(ref x2, ref x3); Swap(ref y2, ref y3); }
            if (y1 > y2) { Swap(ref x1, ref x2); Swap(ref y1, ref y2); }

            // Handle degenerate cases
            if (y1 == y3) return; // All points on same horizontal line

            // Rasterize upper triangle (y1 to y2)
            if (y1 != y2)
            {
                RasterizeTriangleHalfAntialiased(ref pixels, w, h, x1, y1, x2, y2, x3, y3, y1, y2, true);
            }

            // Rasterize lower triangle (y2 to y3)
            if (y2 != y3)
            {
                RasterizeTriangleHalfAntialiased(ref pixels, w, h, x1, y1, x2, y2, x3, y3, y2, y3, false);
            }
        }

        private static void RasterizeTriangleHalfAntialiased(ref Color32[] pixels, int w, int h,
            int x1, int y1, int x2, int y2, int x3, int y3,
            int startY, int endY, bool isUpperHalf)
        {
            // Calculate edge slopes using floating point for smooth antialiasing
            float longEdgeDx = x3 - x1;
            float longEdgeDy = y3 - y1;
            
            float shortEdgeDx, shortEdgeDy, shortStartX, shortStartY;
            if (isUpperHalf)
            {
                shortEdgeDx = x2 - x1;
                shortEdgeDy = y2 - y1;
                shortStartX = x1;
                shortStartY = y1;
            }
            else
            {
                shortEdgeDx = x3 - x2;
                shortEdgeDy = y3 - y2;
                shortStartX = x2;
                shortStartY = y2;
            }

            // Rasterize scanlines
            for (int y = startY; y <= endY; y++)
            {
                if (y < 0 || y >= h) continue;

                // Calculate X intersections using floating point for subpixel precision
                float longX, shortX;
                
                if (Math.Abs(longEdgeDy) > 0.001f)
                {
                    longX = x1 + (longEdgeDx * (y - y1)) / longEdgeDy;
                }
                else
                {
                    longX = x1;
                }

                if (Math.Abs(shortEdgeDy) > 0.001f)
                {
                    shortX = shortStartX + (shortEdgeDx * (y - shortStartY)) / shortEdgeDy;
                }
                else
                {
                    shortX = shortStartX;
                }

                // Ensure left <= right
                float leftX = Math.Min(longX, shortX);
                float rightX = Math.Max(longX, shortX);

                // Expand range by 1 pixel for antialiasing
                int startPixel = Math.Max(0, (int)Math.Floor(leftX - 1));
                int endPixel = Math.Min(w - 1, (int)Math.Ceiling(rightX + 1));

                // Fill scanline with antialiasing
                int rowStart = y * w;
                for (int x = startPixel; x <= endPixel; x++)
                {
                    int idx = rowStart + x;
                    if (pixels.Length <= idx)
                    {
                        break;
                    }
                    var p = pixels[idx];
         
                    if (!ApparelDamageVisualsMod.Settings.Antialiasing)
                    {
                        p.a = 0;
                        pixels[idx] = p;
                    }
                    else
                    {
                        float coverage = CalculatePixelCoverage(x, leftX, rightX);
             
                        if (coverage >= 1f)
                        {
                            p.a = 0;
                            pixels[idx] = p;
                        }
                        else if (coverage > 0f)
                        {
                            float newAlpha = p.a * (1f - coverage);
                            if (ApparelDamageVisualsMod.Settings.Outline)
                            {
                                p = SetBlack(p);
                            }
                            p.a = (byte)Mathf.RoundToInt(newAlpha);
                            pixels[idx] = p;
                        }
                    }
                }
            }
        }

        static Color32 SetBlack(Color32 original)
        {
            return new Color32(0, 0, 0, original.a);
        }

        /// <summary>
        /// Calculate how much of a pixel is covered by the triangle edge
        /// </summary>
        private static float CalculatePixelCoverage(float pixelX, float leftEdge, float rightEdge)
        {
            float pixelLeft = pixelX - 0.5f;
            float pixelRight = pixelX + 0.5f;
            

            // Calculate intersection of pixel bounds 
            float intersectLeft = Math.Max(pixelLeft, leftEdge);
            float intersectRight = Math.Min(pixelRight, rightEdge);

            if (intersectLeft >= intersectRight)
            {
                if (intersectLeft - intersectRight > ApparelDamageVisualsMod.Settings.Thickness)
                    return 0f; // No coverage
                else
                {
                    // Coverage is the fraction of the pixel that's inside the expanded triangle
                    float pixelWidth = pixelRight - pixelLeft; // Always 1.0 for unit pixels
                    float coverage = (intersectRight - intersectLeft + ApparelDamageVisualsMod.Settings.Thickness) / (pixelWidth + ApparelDamageVisualsMod.Settings.Thickness);
                    return Mathf.Clamp01(coverage);
                }
            }
            return 1;
        }

        private static void Swap(ref int a, ref int b)
        {
            (b, a) = (a, b);
        }
    }
}
