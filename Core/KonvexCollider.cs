using System;
using System.Collections.Generic;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider : Collider
    {
        private static readonly Vector3[] _defaultPoints = new Vector3[5]
        {
            new Vector3 (-0.5f, -0.5f, -0.5f),
            new Vector3 (-0.5f, -0.5f,  0.5f),
            new Vector3 ( 0.5f, -0.5f,  0.5f),
            new Vector3 ( 0.5f, -0.5f, -0.5f),
            new Vector3 (   0f,  0.5f,    0f),
        };

        private static readonly int[] _defaultIndices = new int[16]
        {
            0, 1, 1, 2, 2, 3, 3, 0, 0, 5, 1, 5, 2, 5, 3, 5,
        };

        private static readonly Vector3[] _defaultNormals = new Vector3[5]
        {
            new Vector3(-0.894427f, 0.447214f,         0f),
            new Vector3(        0f, 0.447214f,  0.894427f),
            new Vector3( 0.894427f, 0.447214f,         0f),
            new Vector3(        0f, 0.447214f, -0.894427f),
            new Vector3(        0f,       -1f,         0f),
        };

        public KonvexCollider()
        {
            PointsRaw = _defaultPoints;
            Indices = _defaultIndices;
            NormalsRaw = _defaultNormals;
        }

        public KonvexCollider(Vector3[] points, int[] indices, Vector3[] normals, Vector3 scale, Rigidbody rigidbody, Vector3 center, PrimitiveShape shape, float restitution) : base(scale, rigidbody, center, shape, restitution)
        {
            PointsRaw = points;
            Indices = indices;
            NormalsRaw = normals;
        }

        public Vector3[] NormalsRaw;

        /// <summary>
        /// the normals of each face rotated by the collider
        /// </summary>
        public Vector3[] Normals
        {
            get
            {
                Vector3[] normals = new Vector3[NormalsRaw.Length];
                for (int i = 0; i < NormalsRaw.Length; i++)
                {
                    normals[i] = Quaternion.RotateVector(NormalsRaw[i], Rotation);
                }
                return normals;
            }
        }

        public int[] Indices;

        public Vector3[] PointsRaw;

        /// <summary>
        /// scaled and rotated points of the collider
        /// </summary>
        public Vector3[] Points
        {
            get
            {
                Vector3[] points = new Vector3[PointsRaw.Length];
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    points[i] = Quaternion.RotateVector(Vector3.Scale(PointsRaw[i], ScaledSize), Rotation);
                }
                return points;
            }
        }

        public override CollisionInfo IsColliding(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}