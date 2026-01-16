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

            int minSpread = Math.Max(3, (int)(size * 0.4f));

            switch (edge)
            {
                case 0: // West
                    realMaxX = minX + (int)size;
                    int baseY_W = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY - minSpread * 2)) + minSpread;
                    x1 = realMaxX;
                    y1 = baseY_W;
                    x2 = realMinX;
                    y2 = baseY_W - minSpread - rng.Next(Math.Max(1, minSpread));
                    x3 = realMinX;
                    y3 = baseY_W + minSpread + rng.Next(Math.Max(1, minSpread));
                    break;
                case 1: // East
                    realMinX = maxX - (int)size;
                    int baseY_E = realMinY + rng.Next(Math.Max(1, realMaxY - realMinY - minSpread * 2)) + minSpread;
                    x1 = realMinX;
                    y1 = baseY_E;
                    x2 = realMaxX;
                    y2 = baseY_E - minSpread - rng.Next(Math.Max(1, minSpread));
                    x3 = realMaxX;
                    y3 = baseY_E + minSpread + rng.Next(Math.Max(1, minSpread));
                    break;
                case 2: // South
                    realMaxY = minY + (int)size;
                    int baseX_S = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX - minSpread * 2)) + minSpread;
                    x1 = baseX_S;
                    y1 = realMaxY;
                    x2 = baseX_S - minSpread - rng.Next(Math.Max(1, minSpread));
                    y2 = realMinY;
                    x3 = baseX_S + minSpread + rng.Next(Math.Max(1, minSpread));
                    y3 = realMinY;
                    break;
                case 3: // North
                    realMinY = maxY - (int)size;
                    int baseX_N = realMinX + rng.Next(Math.Max(1, realMaxX - realMinX - minSpread * 2)) + minSpread;
                    x1 = baseX_N;
                    y1 = realMinY;
                    x2 = baseX_N - minSpread - rng.Next(Math.Max(1, minSpread));
                    y2 = realMaxY;
                    x3 = baseX_N + minSpread + rng.Next(Math.Max(1, minSpread));
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
