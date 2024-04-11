using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public class Quadtree<T>
    {
        public Rectangle Boundary { get; set; }
        public int Capacity { get; set; }
        public bool Divided { get; set; }
        public List<QuadtreeData<T>> Points { get; private set; } = new List<QuadtreeData<T>>();

        /// <summary>
        /// North West, North East, South East, South West
        /// </summary>
        private Quadtree<T>[] _nodes = new Quadtree<T>[4];

        public Quadtree(Rectangle boundary, int capacity)
        {
            Boundary = boundary;
            Capacity = capacity;

            Divided = false;
        }

        /// <summary>
        /// Inserts a QuadtreeData into the Quadtree
        /// </summary>
        /// <returns>true if successful</returns>
        public bool Insert(QuadtreeData<T> data)
        {
            if (!Boundary.InBounds(data.Position))
                return false;

            if (Points.Count < Capacity)
            {
                Points.Add(data);
                return true;
            }
            if (!Divided)
                Subdivide();

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Insert(data))
                    return true;
            }

            return false;

        }

        private void Subdivide()
        {
            float width = Boundary.Size.x / 2;
            float height = Boundary.Size.y / 2;
            float x = Boundary.Position.x;
            float y = Boundary.Position.y;

            // North West
            _nodes[0] = new Quadtree<T>(new Rectangle(new Vector2(x - width, y + height), new Vector2(width, height)), Capacity);

            // North East
            _nodes[1] = new Quadtree<T>(new Rectangle(new Vector2(x + width, y + height), new Vector2(width, height)), Capacity);

            // South East
            _nodes[2] = new Quadtree<T>(new Rectangle(new Vector2(x + width, y - height), new Vector2(width, height)), Capacity);

            // South West
            _nodes[3] = new Quadtree<T>(new Rectangle(new Vector2(x - width, y - height), new Vector2(width, height)), Capacity);

            Divided = true;
        }

        /// <summary>
        /// Gets all the QuadtreeData in the given area
        /// </summary>
        /// <param name="pos">position of the rectangle</param>
        /// <param name="size">scale of the rectangle in halfs</param>
        /// <returns></returns>
        public List<QuadtreeData<T>> Receive(Vector2 pos, Vector2 size)
        {
            List<QuadtreeData<T>> data = new List<QuadtreeData<T>>();
            Rectangle rect = new Rectangle(pos, size);
            if (!Boundary.Intersects(rect))
                return data;

            foreach (var item in Points)
            {
                if (rect.InBounds(item.Position))
                    data.Add(item);
            }

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i] == null)
                    break;
                data.AddRange(_nodes[i].Receive(pos, size));
            }

            return data;
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
