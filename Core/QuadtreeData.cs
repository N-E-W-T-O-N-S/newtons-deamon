using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public struct QuadtreeData<T>
    {
        public Vector3 Position { get; set; }
        public T Data { get; set; }

        public QuadtreeData(Vector3 position, T data)
        {
            Position = position;
            Data = data;
        }
    }
}
