using ApparelDamageVisuals.ADVGraphics;
using System;
using UnityEngine;

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
    }
}
