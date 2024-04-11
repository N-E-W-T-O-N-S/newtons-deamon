using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public readonly struct QuadtreeData<T>
    {
        public Vector2 Position { get; }
        public T Data { get; }

        public QuadtreeData(Vector3 position, T data)
        {
            Position = position;
            Data = data;
        }
    }
}
