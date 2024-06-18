using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public readonly struct QuadtreeData<T>
    {
        public Vector2 Position { get; }
        public Rectangle Area { get; }
        public T Data { get; }
        public DataType type { get; }

        public QuadtreeData(Vector3 position, T data)
        {
            Position = position;
            Area = default;
            Data = data;
            type = DataType.Point;
        }

        public QuadtreeData(Rectangle area, T data)
        {
            Position = default;
            Area = area;
            Data = data;
            type = DataType.Area;
        }

        public enum DataType
        {
            Point,
            Area
        }
    }
}
