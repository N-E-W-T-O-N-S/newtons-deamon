using NEWTONS.Core._2D;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NEWTONS.Core._3D
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
            Size = new Vector3(1, 1, 1);
            PointsRaw = _defaultPoints;
            Indices = _defaultIndices;
            NormalsRaw = _defaultNormals;
        }

        public KonvexCollider(Vector3[] points, int[] indices, Vector3[] normals, Rigidbody rigidbody, Vector3 scale, Vector3 size, Vector3 center, float restitution, bool addToEngine = true) : base(rigidbody, scale, center, restitution, addToEngine)
        {
            PointsRaw = points;
            Indices = indices;
            NormalsRaw = normals;
            Size = size;
        }

        public override Vector3 Scale
        {
            get
            {
                return base.Scale;
            }
            set
            {
                if (base.Scale == value)
                    return;

                base.Scale = value;
                p_pointsNeedsUpdate = true;
                p_scaledSizeNeedsUpdate = true;
            }
        }

        public override Vector3 ScaleNoNotify
        {
            set
            {
                if (Scale == value)
                    return;

                base.ScaleNoNotify = value;
                p_pointsNeedsUpdate = true;
                p_scaledSizeNeedsUpdate = true;
            }

        }

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector3 size;

        /// <summary>
        /// size of the collider
        /// </summary>
        public virtual Vector3 Size
        {
            get => size;
            set
            {
                Vector3 newSize = new Vector3(Mathf.Max(value.x, 0), Mathf.Max(value.y, 0), Mathf.Max(value.z, 0));
                if (newSize == size)
                    return;

                size = newSize;
                p_pointsNeedsUpdate = true;
                p_scaledSizeNeedsUpdate = true;
            }
        }

        protected bool p_scaledSizeNeedsUpdate = true;

        private Vector3 _scaledSize;

        public virtual Vector3 ScaledSize
        {
            get
            {
                if (p_scaledSizeNeedsUpdate)
                    _scaledSize = Vector3.Scale(Size, Scale);

                p_scaledSizeNeedsUpdate = false;
                p_pointsNeedsUpdate = true;
                return _scaledSize;
            }
        }

        public Vector3[] NormalsRaw;

        protected bool p_normalsNeedsUpdate = true;

        protected Vector3[] p_normals = new Vector3[0];

        /// <summary>
        /// the normals of each face rotated by the collider
        /// </summary>
        public virtual Vector3[] Normals
        {
            get
            {
                if (!p_normalsNeedsUpdate)
                    return p_normals;

                Vector3[] normals = new Vector3[NormalsRaw.Length];
                for (int i = 0; i < NormalsRaw.Length; i++)
                {
                    normals[i] = Quaternion.RotateVector(NormalsRaw[i], Rotation);
                }

                p_normals = normals;
                p_normalsNeedsUpdate = false;

                return p_normals;
            }
        }

        public int[] Indices;

        public Vector3[] PointsRaw;

        protected bool p_pointsNeedsUpdate = true;

        private Vector3[] _points = new Vector3[0];

        /// <summary>
        /// scaled and rotated points of the collider
        /// </summary>
        public Vector3[] Points
        {
            get
            {
                if (!p_pointsNeedsUpdate)
                    return _points;

                Vector3[] points = new Vector3[PointsRaw.Length];
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    points[i] = Quaternion.RotateVector(Vector3.Scale(PointsRaw[i], ScaledSize), Rotation);
                }

                p_pointsNeedsUpdate = false;
                p_boundsNeedsUpdate = true;
                p_normalsNeedsUpdate = true;

                _points = points;

                return _points;
            }
        }

        private Bounds _bounds;

        public override Bounds Bounds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!p_boundsNeedsUpdate) return _bounds;

                var ps = Points;
                Vector3 center = GlobalCenter;

                Bounds bounds = Bounds.InvertedBounds;

                for (int i = 0; i < ps.Length; i++)
                {
                    bounds.IncludePoint(ps[i] + center);
                }

                _bounds = bounds;
                p_boundsNeedsUpdate = false;

                return bounds;
            }
        }
        internal override void RotationChanged()
        {
            base.RotationChanged();
            p_boundsNeedsUpdate = true;
            p_pointsNeedsUpdate = true;
        }

        internal override void PositionChanged()
        {
            base.PositionChanged();
            p_pointsNeedsUpdate = true;
            p_boundsNeedsUpdate = true;
        }


        public override CollisionInfo IsColliding(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}