using System;
using System.Runtime.Remoting.Messaging;

namespace ApparelDamageVisuals.ADVGraphics
{
    public class Circle : Shape
    {
        private readonly float radius;

        internal int CX => x;
        internal int CY => y;
        public override int MinX => Math.Max(0, (int)Math.Floor(CX - radius));
        public override int MaxX => Math.Min(boundaryX - 1, (int)Math.Ceiling(CX + radius));
        public override int MinY => Math.Max(0, (int)Math.Floor(CY - radius));
        public override int MaxY => Math.Min(boundaryY - 1, (int)Math.Ceiling(CY + radius));

        internal float Radius => radius;

        private float RadiusSquared => radius * radius;


        public Circle(int x, int y, int boundaryX, int boundaryY, float r) : base(x, y, boundaryX, boundaryY)
        {
            this.radius = r;
        }

        public override bool IsCoordinateInShape(int px, int py)
        {
            int dx = CX - px;
            int dy = CY - py;
            return (dx * (double)dx + dy * (double)dy) <= RadiusSquared;
        }
    }
}