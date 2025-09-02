using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

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
            numberOfHoles /= 3; // Number of holes on amor are smaller
            int boundsW = Math.Max(1, maxX - minX + 1);
            int boundsH = Math.Max(1, maxY - minY + 1);
            
            for (int i = 0; i < numberOfHoles; i++)
            {
                // Size relative to content bounds
                int x1 = minX + rng.Next(Math.Max(1, maxX - minX + 1));
                int y1 = minY + rng.Next(Math.Max(1, maxY - minY + 1));
                int x2 = minX + rng.Next(Math.Max(1, maxX - minX + 1));
                int y2 = minY + rng.Next(Math.Max(1, maxY - minY + 1));
                int x3 = minX + rng.Next(Math.Max(1, maxX - minX + 1));
                int y3 = minY + rng.Next(Math.Max(1, maxY - minY + 1));
                yield return new Triangle(x1, y1, x2, y2, x3, y3, boundsW, boundsH);
            }
        }
    }
}
