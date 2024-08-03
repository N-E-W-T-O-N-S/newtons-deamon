﻿using NEWTONS.Debugger;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;

namespace NEWTONS.Core._2D
{
    public class BVH2D<T>
    {
        public BVHData2D<T>[] bvhData;
        public BVHNode2D[] nodes;

        public BVH2D()
        {
            bvhData = new BVHData2D<T>[0];
            nodes = new BVHNode2D[0];
        }

        public void Build(BVHData2D<T>[] bvhData)
        {
            this.bvhData = bvhData;
            nodes = new BVHNode2D[bvhData.Length * 2 - 1];

            nodes[0].leftChild = 0;
            nodes[0].startIndex = 0;
            nodes[0].indexCount = bvhData.Length;

            UpdateBounds(0);

            int usedNodes = 1;
            Subdivide(0, ref usedNodes);
        }

        public void UpdateBounds(int index)
        {
            ref var node = ref nodes[index];

            if (node.indexCount == 1)
            {
                nodes[index].bounds = this.bvhData[node.startIndex].bounds;
                return;
            }

            node.bounds = Bounds2D.InvertedBounds;

            for (uint i = 0; i < node.indexCount; i++)
            {
                Bounds2D bounds = bvhData[node.startIndex + i].bounds;

                node.bounds.IncludePoint(bounds.Min);
                node.bounds.IncludePoint(bounds.Max);
            }
        }

        public void Subdivide(int index, ref int usedNodes)
        {
            ref BVHNode2D node = ref nodes[index];

            if (node.indexCount == 1) return;
            if (node.indexCount == 2)
            {
                int leftChildIdx = usedNodes++;
                int rightChildIdx = usedNodes++;

                nodes[leftChildIdx].startIndex = node.startIndex;
                nodes[leftChildIdx].indexCount = 1;

                nodes[rightChildIdx].startIndex = node.startIndex + 1;
                nodes[rightChildIdx].indexCount = 1;

                node.leftChild = leftChildIdx;
                node.indexCount = 0;

                UpdateBounds(leftChildIdx);
                UpdateBounds(rightChildIdx);

                return;
            }

            Vector2 boundsSize = node.bounds.Max - node.bounds.Min;

            int axis = 0;
            if (boundsSize.y > boundsSize.x) axis = 1;

            // INFO: This whole thing does weired. the median object splits the sorted array in half
            // so the sorting part after this is useless because the array is already sorted and the split is in the half
            float[] axisPoints = new float[node.indexCount];
            for (int i = 0; i < node.indexCount; i++)
                axisPoints[i] = this.bvhData[node.startIndex + i].position[axis];

            // INFO: O(N log(N)) 🤮
            Array.Sort(axisPoints);

            //TODO: better way than offsetting the split position
            float splitPos = axisPoints[axisPoints.Length / 2] + 1e-6f;

            int start = node.startIndex;
            int j = start + node.indexCount - 1;
            while (start <= j)
            {
                if (bvhData[start].position[axis] < splitPos)
                    start++;
                else
                {
                    Swap(ref bvhData[start], ref bvhData[j]);
                    j--;
                }
            }

            int leftCount = start - node.startIndex;
            if (leftCount <= 0)
                leftCount = 1;
            else if (leftCount >= node.indexCount)
                leftCount = node.indexCount - 1;

            int leftChildIndex = usedNodes++;
            int rightChildIndex = usedNodes++;

            nodes[leftChildIndex].startIndex = node.startIndex;
            nodes[leftChildIndex].indexCount = leftCount;

            nodes[rightChildIndex].startIndex = start;
            nodes[rightChildIndex].indexCount = node.indexCount - leftCount;

            node.leftChild = leftChildIndex;
            node.indexCount = 0;

            UpdateBounds(leftChildIndex);
            UpdateBounds(rightChildIndex);

            Subdivide(leftChildIndex, ref usedNodes);
            Subdivide(rightChildIndex, ref usedNodes);
        }

        private void Swap<S>(ref S i, ref S j)
        {
            (j, i) = (i, j);
        }

        public void Receive(Bounds2D bounds, List<BVHData2D<T>> bvhData) => Receive(bounds, 0, bvhData);

        private void Receive(Bounds2D bounds, int nodeIndex, List<BVHData2D<T>> bvhData)
        {
            var node = nodes[nodeIndex];
            if (!bounds.Intersects(node.bounds))
            {
                //Debug.Log("not in bounds");
                return;
            }
            if (node.isLeaf)
            {
                //Debug.Log(node.indexCount);
                for (int i = 0; i < node.indexCount; i++)
                    bvhData.Add(this.bvhData[node.startIndex + i]);
                return;
            }

            Receive(bounds, node.leftChild, bvhData);
            Receive(bounds, node.rightChild, bvhData);
        }
    }
}
