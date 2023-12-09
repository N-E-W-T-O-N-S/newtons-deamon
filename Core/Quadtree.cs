using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class Quadtree<T>
    {
        public Vector2 Center { get; set; }
        public float HalfWidth { get; set; }
        public float HalfHeight { get; set; }
        public int Capacity { get; set; }
        public int MaxDepth { get; set; }
        public bool Divided { get; set; }
        public List<QuadtreeData<T>> Points { get; private set; } = new List<QuadtreeData<T>>();

        /// <summary>
        /// North West, North East, South East, South West
        /// </summary>
        private Quadtree<T>[] _nodes = new Quadtree<T>[4];

        public Quadtree(Vector2 ceter, float halfWidth, float halfHeight, int capacity, int maxDepth, List<QuadtreeData<T>> points)
        {
            Center = ceter;
            HalfWidth = halfWidth;
            HalfHeight = halfHeight;
            Capacity = capacity;
            MaxDepth = maxDepth;
            Points = points;

            Divided = false;
        }

        public bool Insert(QuadtreeData<T> point)
        {
            if (!InBounds(point.Position))
                return false;

            if (Points.Count < Capacity)
            {
                Points.Add(point);
                return true;
            }
            if (!Divided)
                Subdivide();

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Insert(point))
                    return true;
            }

            return false;

        }

        private void Subdivide()
        {
            float width = HalfWidth / 2;
            float height = HalfHeight / 2;

            // North West
            _nodes[0] = new Quadtree<T>(new Vector2(Center.x - width, Center.y + height), width, height, Capacity, MaxDepth - 1, new List<QuadtreeData<T>>());

            // North East
            _nodes[1] = new Quadtree<T>(new Vector2(Center.x + width, Center.y + height), width, height, Capacity, MaxDepth - 1, new List<QuadtreeData<T>>());

            // South East
            _nodes[2] = new Quadtree<T>(new Vector2(Center.x + width, Center.y - height), width, height, Capacity, MaxDepth - 1, new List<QuadtreeData<T>>());

            // South West
            _nodes[3] = new Quadtree<T>(new Vector2(Center.x - width, Center.y - height), width, height, Capacity, MaxDepth - 1, new List<QuadtreeData<T>>());

            Divided = true;
        }

        public List<QuadtreeData<T>> Query(Vector2 pos, Vector2 scale)
        {
            List<QuadtreeData<T>> data = new List<QuadtreeData<T>>();
            if (!Intersects(pos, scale))
                return data;

            foreach (var item in Points)
            {
                if (InBounds(pos, scale, item.Position))
                    data.Add(item);
            }

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i] == null)
                    break;
                data.AddRange(_nodes[i].Query(pos, scale));
            }

            return data;
        }

        public bool InBounds(Vector2 point)
        {
            //TODO: edge cases => what if point is on the edge of the quadtree
            return point.x >= Center.x - HalfWidth && point.x <= Center.x + HalfWidth && point.y >= Center.y - HalfHeight && point.y <= Center.y + HalfHeight;
        }

        public bool InBounds(Vector2 ceter, Vector2 scale, Vector2 point)
        {
               return point.x >= ceter.x - scale.x && point.x <= ceter.x + scale.x && point.y >= ceter.y - scale.y && point.y <= ceter.y + scale.y;
        }

        public bool Intersects(Vector2 point, Vector2 scale)
        {
            return point.x + scale.x >= Center.x - HalfWidth && point.x - scale.x <= Center.x + HalfWidth && point.y + scale.y >= Center.y - HalfHeight && point.y - scale.y <= Center.y + HalfHeight;
        }

        public List<Quadtree<T>> GetTrees()
        {
            List<Quadtree<T>> qt = new List<Quadtree<T>>()
            {
                this
            };
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i] == null)
                    break;
                qt.AddRange(_nodes[i].GetTrees());
            }
            return qt;
        }
    }
}
