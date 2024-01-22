using System;
using System.Collections.Generic;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider : Collider
    {
        public KonvexCollider()
        {
            
        }

        public KonvexCollider(Vector3[] points, int[] indices, Vector3 scale, KinematicBody kinematicBody, Vector3 center, Quaternion rotation, PrimitiveShape shape, float restitution) : base(scale, kinematicBody, center, rotation, shape, restitution)
        {
            PointsRaw = points;
            Indices = indices;
        }

        public int[] Indices;

        public Vector3[] PointsRaw;

        public Vector3[] Points
        {
            get 
            {
                Vector3[] points = new Vector3[PointsRaw.Length];
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    points[i] = Quaternion.RotateVector(Vector3.ComponentMultiply(PointsRaw[i], GlobalScales), Rotation);
                }
                return points;
            }
        }

        ///// <summary>
        ///// 90° counterclockwise rotated edge normalized
        ///// </summary>
        //public Vector3[] EdgeNormals
        //{
        //    get
        //    {
        //        Vector3[] points = Points;
        //        int[] indices = Indices;
        //        Vector3[] normals = new Vector3[points.Length];

        //        for (int i = 0; i < indices.Length; i += 2)
        //        {
        //            Vector3 axis = points[indices[i]] - points[indices[i + 1]];

        //        }

        //        return normals;
        //    }
        //}

        public virtual bool IsColliding(KonvexCollider other)
        {
            List<Vector3> edges1 = new List<Vector3>();
            List<Vector3> edges2 = new List<Vector3>();

            Vector3[] points1 = Points;
            Vector3[] points2 = other.Points;

            for (int i = 0; i < Indices.Length; i += 2)
            {
                edges1.Add(points1[Indices[i]] - points1[Indices[i + 1]]);
            }

            for (int i = 0; i < other.Indices.Length; i += 2)
            {
                edges2.Add(points2[other.Indices[i]] - points2[other.Indices[i + 1]]);
            }

            Vector3[] axisToCheck = new Vector3[(Indices.Length + other.Indices.Length) / 2];

            int j = 0;
            foreach (var edge1 in edges1)
            {
                foreach (var edge2 in edges2)
                {
                    axisToCheck[j] = Vector3.Cross(edge1, edge2).Normalized;
                    j++;
                }
            }



            return true;
        }

    }
}