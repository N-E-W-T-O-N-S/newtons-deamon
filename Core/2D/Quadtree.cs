using NEWTONS.Debugger;
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
        public List<QuadtreeData<T>> Data { get; private set; } = new List<QuadtreeData<T>>();

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
            switch (data.type)
            {
                case QuadtreeData<T>.DataType.Point:
                    if (!Boundary.InBounds(data.Position))
                        return false;

                    if (Data.Count < Capacity)
                    {
                        Data.Add(data);
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
                case QuadtreeData<T>.DataType.Area:
                    if (!Boundary.Intersects(data.Area))
                        return false;
                    if (!Boundary.Encapsulates(data.Area))
                        return false;

                    if (Data.Count < Capacity)
                    {
                        Data.Add(data);
                        return true;
                    }

                    if (!Divided)
                        Subdivide();

                    for (int i = 0; i < _nodes.Length; i++)
                    {
                        if (_nodes[i].Insert(data))
                            return true;
                    }

                    Data.Add(data);
                    return true;
                default:
                    Debug.LogWarning("Quadtree Data Type (" + data.type.ToString() + ") not implemented!");
                    break;
            }

            return false;
        }

        private void Subdivide()
        {
            float hWidth = Boundary.Width * 0.25f;
            float hHeight = Boundary.Height * 0.25f;
            float width = Boundary.Width * 0.5f;
            float height = Boundary.Height * 0.5f;
            float x = Boundary.Position.x;
            float y = Boundary.Position.y;

            // North West
            _nodes[0] = new Quadtree<T>(new Rectangle(new Vector2(x - hWidth, y + hHeight), width, height), Capacity);

            // North East
            _nodes[1] = new Quadtree<T>(new Rectangle(new Vector2(x + hWidth, y + hHeight), width, height), Capacity);

            // South East
            _nodes[2] = new Quadtree<T>(new Rectangle(new Vector2(x + hWidth, y - hHeight), width, height), Capacity);

            // South West
            _nodes[3] = new Quadtree<T>(new Rectangle(new Vector2(x - hWidth, y - hHeight), width, height), Capacity);

            Divided = true;
        }

        /// <summary>
        /// Gets all the QuadtreeData in the given area
        /// </summary>
        /// <returns></returns>
        

        // TODO: Pass data list directly into the method
        public void Receive(Rectangle rect, List<QuadtreeData<T>> data)
        {
            if (!Boundary.Intersects(rect))
                return;

            foreach (var item in Data)
            {
                switch (item.type)
                {
                    case QuadtreeData<T>.DataType.Point:
                        if (rect.InBounds(item.Position))
                            data.Add(item);
                        break;
                    case QuadtreeData<T>.DataType.Area:
                        if (rect.Intersects(item.Area))
                            data.Add(item);
                        break;
                }

            }

            if (_nodes[0] == null)
                return;

            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i].Receive(rect, data);
            }
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
