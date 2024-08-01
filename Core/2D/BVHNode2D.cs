using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public struct BVHNode2D
    {
        public Bounds2D bounds;
        public uint leftChild;
        public uint startIndex, indexCount;

        public readonly uint rightChild => leftChild + 1;

        public readonly bool isLeaf => indexCount > 0;
    }
}
