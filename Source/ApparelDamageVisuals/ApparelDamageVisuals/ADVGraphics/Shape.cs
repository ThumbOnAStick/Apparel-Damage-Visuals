using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ApparelDamageVisuals.ADVGraphics
{
    public abstract class Shape
    {
        // Coordinate of the shape
        protected int x;
        protected int y;
        protected int boundaryX;
        protected int boundaryY;


        public abstract int MaxX { get; }
        public abstract int MinX { get; }
        public abstract int MaxY { get; }
        public abstract int MinY { get; }

        public Shape(int x, int y, int boundaryX, int boundaryY)
        {
            this.x = x;
            this.y = y;
            this.boundaryX = boundaryX;
            this.boundaryY = boundaryY;
        }

        public abstract bool IsCoordinateInShape(int x, int y);
    }
}
