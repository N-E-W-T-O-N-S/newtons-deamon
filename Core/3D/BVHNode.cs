using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._3D
{
    public struct BVHNode
    {
        public Bounds bounds;
        public int leftChild;
        public int startIndex, indexCount;

        public readonly int rightChild => leftChild + 1;

        public readonly bool isLeaf => indexCount > 0;
    }
}
