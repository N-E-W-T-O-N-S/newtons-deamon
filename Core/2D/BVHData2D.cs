using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public struct BVHData2D<T>
    {
        public Vector2 position;
        public Bounds2D bounds;
        public T data;

        public BVHData2D(Vector2 position, Bounds2D bounds, T data)
        {
            this.position = position;
            this.bounds = bounds;
            this.data = data;
        }
    }
}
