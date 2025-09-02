using ApparelDamageVisuals.ADVGraphics;
using System;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals.Utils
{
    public class ShapeHelper
    {
        public static void DrawTransparentCircle(ref Color32[] pixels, Circle circle, int w, int h)
        {
            // Rasterize only the bounding box of the circle
            int cx = circle.CX;
            int cy = circle.CY;
            float r = circle.Radius;
            float r2 = r * r;

            for (int yy = circle.MinY; yy <= circle.MaxY; yy++)
            {
                int dy = yy - cy;
                int dy2 = dy * dy;
                int rowStart = yy * w;

                for (int xx = circle.MinX; xx <= circle.MaxX; xx++)
                {
                    int dx = xx - cx;
                    // compare using float r2 (cheap) and int dx/dy
                    if (dx * dx + dy2 <= r2)
                    {
                        int idx = rowStart + xx;
                        var p = pixels[idx];
                        p.a = 0;
                        pixels[idx] = p;
                    }
                }
            }
        }

        public static void DrawTransparentTriangle(ref Color32[] pixels, Triangle triangle, int w, int h)
        {
            // Get triangle vertices for optimized scanline rasterization
            triangle.GetVertices(out int x1, out int y1, out int x2, out int y2, out int x3, out int y3);
            
            // Use scanline rasterization instead of point-in-triangle tests for better performance
            RasterizeTriangleScanline(ref pixels, w, h, x1, y1, x2, y2, x3, y3);
        }

        private static void RasterizeTriangleScanline(ref Color32[] pixels, int w, int h, 
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
                RasterizeTriangleHalf(ref pixels, w, h, x1, y1, x2, y2, x3, y3, y1, y2, true);
            }

            // Rasterize lower triangle (y2 to y3)
            if (y2 != y3)
            {
                RasterizeTriangleHalf(ref pixels, w, h, x1, y1, x2, y2, x3, y3, y2, y3, false);
            }
        }

        private static void RasterizeTriangleHalf(ref Color32[] pixels, int w, int h,
            int x1, int y1, int x2, int y2, int x3, int y3,
            int startY, int endY, bool isUpperHalf)
        {
            // Calculate edge slopes using integer arithmetic
            int longEdgeDx = x3 - x1;
            int longEdgeDy = y3 - y1;
            
            int shortEdgeDx, shortEdgeDy, shortStartX, shortStartY;
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

                // Calculate X intersections using integer arithmetic with proper rounding
                int longX, shortX;
                
                if (longEdgeDy != 0)
                {
                    // Use fixed-point arithmetic to avoid floating point
                    long longNumerator = (long)longEdgeDx * (y - y1);
                    longX = x1 + (int)(longNumerator / longEdgeDy);
                }
                else
                {
                    longX = x1;
                }

                if (shortEdgeDy != 0)
                {
                    long shortNumerator = (long)shortEdgeDx * (y - shortStartY);
                    shortX = shortStartX + (int)(shortNumerator / shortEdgeDy);
                }
                else
                {
                    shortX = shortStartX;
                }

                // Ensure left <= right
                int leftX = Math.Min(longX, shortX);
                int rightX = Math.Max(longX, shortX);

                // Clamp to texture bounds
                leftX = Math.Max(0, leftX);
                rightX = Math.Min(w - 1, rightX);

                // Fill scanline
                int rowStart = y * w;
                for (int x = leftX; x <= rightX; x++)
                {
                    int idx = rowStart + x;
                    var p = pixels[idx];
                    p.a = 0;
                    pixels[idx] = p;
                }
            }
        }

        private static void Swap(ref int a, ref int b) 
        {
            (b, a) = (a, b);
        }
    }
}
