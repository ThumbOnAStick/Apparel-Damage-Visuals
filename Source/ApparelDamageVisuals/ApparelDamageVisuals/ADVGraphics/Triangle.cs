using System;

namespace ApparelDamageVisuals.ADVGraphics
{
    public class Triangle : Shape
    {
        private readonly int x1, y1, x2, y2, x3, y3;
        private readonly int minX, maxX, minY, maxY;

        public override int MinX => minX;
        public override int MaxX => maxX;
        public override int MinY => minY;
        public override int MaxY => maxY;

        // Create triangle from center point and radius (inscribed in circle)
        public Triangle(int centerX, int centerY, int boundaryX, int boundaryY, float radius) 
            : base(centerX, centerY, boundaryX, boundaryY)
        {
            // Create equilateral triangle inscribed in circle
            // Top vertex
            x1 = centerX;
            y1 = centerY - (int)(radius * 0.866f); // cos(30бу) б╓ 0.866

            // Bottom-left vertex  
            x2 = centerX - (int)(radius * 0.75f);
            y2 = centerY + (int)(radius * 0.433f); // sin(30бу)/2 б╓ 0.433

            // Bottom-right vertex
            x3 = centerX + (int)(radius * 0.75f);
            y3 = centerY + (int)(radius * 0.433f);

            // Calculate bounding box
            minX = Math.Max(0, Math.Min(x1, Math.Min(x2, x3)));
            maxX = Math.Min(boundaryX - 1, Math.Max(x1, Math.Max(x2, x3)));
            minY = Math.Max(0, Math.Min(y1, Math.Min(y2, y3)));
            maxY = Math.Min(boundaryY - 1, Math.Max(y1, Math.Max(y2, y3)));
        }

        // Create triangle from three explicit vertices
        public Triangle(int x1, int y1, int x2, int y2, int x3, int y3, int boundaryX, int boundaryY)
            : base((x1 + x2 + x3) / 3, (y1 + y2 + y3) / 3, boundaryX, boundaryY)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.x3 = x3;
            this.y3 = y3;

            // Calculate bounding box
            minX = Math.Max(0, Math.Min(x1, Math.Min(x2, x3)));
            maxX = Math.Min(boundaryX - 1, Math.Max(x1, Math.Max(x2, x3)));
            minY = Math.Max(0, Math.Min(y1, Math.Min(y2, y3)));
            maxY = Math.Min(boundaryY - 1, Math.Max(y1, Math.Max(y2, y3)));
        }

        /// <summary>
        /// Gets the triangle vertices for optimized rasterization
        /// </summary>
        public void GetVertices(out int vx1, out int vy1, out int vx2, out int vy2, out int vx3, out int vy3)
        {
            vx1 = x1; vy1 = y1;
            vx2 = x2; vy2 = y2;
            vx3 = x3; vy3 = y3;
        }

        public override bool IsCoordinateInShape(int px, int py)
        {
            // Use barycentric coordinates to test if point is inside triangle
            float denom = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);
            if (Math.Abs(denom) < 0.001f) return false; // Degenerate triangle

            float a = ((y2 - y3) * (px - x3) + (x3 - x2) * (py - y3)) / denom;
            float b = ((y3 - y1) * (px - x3) + (x1 - x3) * (py - y3)) / denom;
            float c = 1 - a - b;

            return a >= 0 && b >= 0 && c >= 0;
        }
    }
}