using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public struct Rectangle
    {
        /// <summary>
        /// position of the rectangle
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// the size of the rectangle in <b>half</b> widths and heights
        /// </summary>
        public Vector2 Size { get; set; }

        public Rectangle(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public bool InBounds(Vector2 point)
        {
            return point.x >= Position.x - Size.x && point.x <= Position.x + Size.x && point.y >= Position.y - Size.y && point.y <= Position.y + Size.y;
        }

        public bool Intersects(Rectangle rectangle)
        {
            return
                (
                rectangle.Position.x + rectangle.Size.x >= Position.x - Size.x &&
                rectangle.Position.x - rectangle.Size.x <= Position.x + Size.x &&
                rectangle.Position.y + rectangle.Size.y >= Position.y - Size.y &&
                rectangle.Position.y - rectangle.Size.y <= Position.y + Size.y
                );
        }

    }
}
