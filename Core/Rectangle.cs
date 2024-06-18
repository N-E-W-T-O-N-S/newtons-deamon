using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public readonly struct Rectangle
    {
        /// <summary>
        /// position of the rectangle
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// Bottom left corner in world space
        /// </summary>
        public Vector2 Min { get; }

        /// <summary>
        /// Top right corner in world space
        /// </summary>
        public Vector2 Max { get; }

        public float Width { get; }

        public float Height { get; }

        public Rectangle (Vector2 position, float width, float height)
        {
            Position = position;

            Vector2 size = new Vector2 (width * 0.5f, height * 0.5f);
            Max = size + Position;
            Min = -size + Position;
            Width = width;
            Height = height;
        }

        public Rectangle (Vector2 min, Vector2 max)
        {
            float x = (max.x - min.x) * 0.5f + min.x;
            float y = (max.y - min.y) * 0.5f + min.y;
            Position = new Vector2(x, y);
            Min = min;
            Max = max;
            Width = max.x - min.x;
            Height = max.y - min.y;
        }

        public readonly bool InBounds(Vector2 point)
        {
            return point.x > Min.x && point.x < Max.x && point.y > Min.y && point.y < Max.y;
        }

        public readonly bool Intersects(Rectangle rect)
        {
            if (Max.x < rect.Min.x || Min.x > rect.Max.x)
                return false;
            if (Max.y < rect.Min.y || Min.y > rect.Max.y)
                return false;

            return true;
        }

        public readonly bool Encapsulates(Rectangle rect)
        {
            return rect.Min.x >= Min.x && rect.Min.y >= Min.y && rect.Max.x <= Max.x && rect.Max.y <= Max.y;
        }

    }
}
