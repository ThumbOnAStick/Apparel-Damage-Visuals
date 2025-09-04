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
            var holeSizeRange = HoleSize(scale) * 3; // Make these holes at least as large as round ones (Bad solution) 
            float size = holeSizeRange.min + (float)rng.NextDouble() * (holeSizeRange.max - holeSizeRange.min);
            size *= ApparelDamageVisualsMod.Settings.Size;
            int c = minX;
            int realMinX = minX;
            int realMaxX = maxX;
            int realMinY = minY;
            int realMaxY = maxY;
            switch (edge)
            {
                case 0: // West
                    realMaxX = minX + (int)size;
                    break;
                case 1: // East
                    realMinX = maxX - (int)size;
                    break;
                case 2: // North
                    realMaxY = minY + (int)size;
                    break;
                case 3: // South
                    realMinY = maxY - (int)size;
                    break;
            }
            for (int i = 0; i < numberOfHoles; i++)
            {
                // Size relative to content bounds
                int x1 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                int y1 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                int x2 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                int y2 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                int x3 = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX + 1));
                int y3 = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY + 1));
                yield return new Triangle(x1, y1, x2, y2, x3, y3, boundsW, boundsH);
            }
        }
    }
}
