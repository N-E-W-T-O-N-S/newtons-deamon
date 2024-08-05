using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._3D
{
    public struct BVHData<T>
    {
        public Vector3 position;
        public Bounds bounds;
        public T data;

        public BVHData(Vector2 position, Bounds bounds, T data)
        {
            this.position = position;
            this.bounds = bounds;
            this.data = data;
        }
    }
}
