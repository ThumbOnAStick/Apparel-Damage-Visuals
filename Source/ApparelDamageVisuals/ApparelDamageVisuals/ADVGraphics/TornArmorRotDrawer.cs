using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace ApparelDamageVisuals.ADVGraphics
{
    /// <summary>
    /// Drawer for broken armor
    /// </summary>
    internal class TornArmorRotDrawer : TornApparelRotDrawer
    {
        public TornArmorRotDrawer() : base() 
        { 

        }

        protected override IEnumerable<Shape> CreateHoleShapes(int w, int h, int minX, int minY, int maxX, int maxY, IntRange holeCount, Random rng)
        {
            int numberOfHoles = holeCount.min <= holeCount.max
                ? holeCount.min + rng.Next(holeCount.max - holeCount.min + 1)
                : holeCount.min;
            int boundsW = Math.Max(1, maxX - minX + 1);
            int boundsH = Math.Max(1, maxY - minY + 1);
            float scale = Math.Min(boundsW, boundsH);
            var edge = rng.Next(4);
            var holeSizeRange = HoleSize(scale);
            float size = holeSizeRange.min + (float)rng.NextDouble() * (holeSizeRange.max - holeSizeRange.min);
            size *= ApparelDamageVisualsMod.Settings.Size * 2;  // Make these holes at least as large as round ones (Bad solution) 
            int c = minX;
            int realMinX = minX;
            int realMaxX = maxX;
            int realMinY = minY;
            int realMaxY = maxY;
            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;
            int x3 = 0;
            int y3 = 0;
            switch (edge)
            {
                case 0: // West
                    realMaxX = minX + (int)size;
                    x1 = realMaxX;
                    y1 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                    x2 = realMinX;
                    y2 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                    x3 = realMinX;
                    y3 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                    break;
                case 1: // East
                    realMinX = maxX - (int)size;
                    x1 = realMinX;
                    y1 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                    x2 = realMaxX;
                    y2 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                    x3 = realMaxX;
                    y3 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                    break;
                case 2: // South
                    realMaxY = minY + (int)size;
                    x1 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                    y1 = realMaxY;
                    x2 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                    y2 = realMinY;
                    x3 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                    y3 = realMinY;
                    break;
                case 3: // North
                    realMinY = maxY - (int)size;
                    x1 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                    y1 = realMinY;
                    x2 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                    y2 = realMaxY;
                    x3 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                    y3 = realMaxY;
                    break;
            }
            for (int i = 0; i < numberOfHoles; i++)
            {
        
     
                yield return new Triangle(x1, y1, x2, y2, x3, y3, boundsW, boundsH);
            }
        }
    }
}
