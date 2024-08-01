using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public struct Bounds2D
    {
        public Vector2 Min;
        public Vector2 Max;

        public Bounds2D(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public Bounds2D(float size, Vector2 position)
        {
            Min = new Vector2(-size, -size) + position;
            Max = new Vector2(size, size) + position;
        }

        public void IncludePoint(Vector2 point)
        {
            if (point.x < Min.x) Min.x = point.x;
            if (point.x > Max.x) Max.x = point.x;

            if (point.y < Min.y) Min.y = point.y;
            if (point.y > Max.y) Max.y = point.y;
        }

        public readonly bool InBounds(Vector2 point)
        {
            return point.x > Min.x && point.x < Max.x && point.y > Min.y && point.y < Max.y;
        }

        public readonly bool Intersects(Bounds2D bounds)
        {
            if (Max.x < bounds.Min.x || Min.x > bounds.Max.x)
                return false;
            if (Max.y < bounds.Min.y || Min.y > bounds.Max.y)
                return false;

            return true;
        }

        public readonly bool Encapsulates(Bounds2D bounds)
        {
            return bounds.Min.x >= Min.x && bounds.Min.y >= Min.y && bounds.Max.x <= Max.x && bounds.Max.y <= Max.y;
        }

        public readonly Rectangle ToRectangle() => new Rectangle(Min, Max);

        /// <summary>
        /// A bounding box that is infinitely large but inverted
        /// </summary>
        public static Bounds2D InvertedBounds => new Bounds2D(Vector2.Infinity, Vector2.NegativeInfinity);

        public override string ToString()
        {
            return $"Min: {Min}, Max: {Max}";
        }
    }
}
