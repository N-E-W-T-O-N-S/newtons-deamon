using NEWTONS.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public class KonvexCollider2D : Collider2D
    {

        private static readonly Vector2[] _defaultPoints = new Vector2[]
        {
            //Points of a hexagon start right going clockwise
            new Vector2(0.5f, 0),
            new Vector2(0.25f, 0.433f),
            new Vector2(-0.25f, 0.433f),
            new Vector2(-0.5f, 0),
            new Vector2(-0.25f, -0.433f),
            new Vector2(0.25f, -0.433f)
        };

        public KonvexCollider2D()
        {
            Size = new Vector2(1, 1);
            PointsRaw = _defaultPoints;
        }

        public KonvexCollider2D(Vector2[] points, Vector2 size, Vector2 scale, Rigidbody2D rigidbody, Vector2 center, float restitution, bool addToEngine = true) : base(rigidbody, scale, center, restitution, addToEngine)
        {
            Size = size;
            PointsRaw = points;
        }

        public override Vector2 Scale
        {
            get
            {
                return base.Scale;
            }
            set
            {
                base.Scale = value;
                p_scaledSizeNeedsUpdate = true;
            }
        }

        private Vector2 _size;

        public virtual Vector2 Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _size;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set 
            { 
                _size = new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
                p_scaledSizeNeedsUpdate = true;
            }
        }

        protected bool p_scaledSizeNeedsUpdate = true;

        private Vector2 _scaledSize;

        public virtual Vector2 ScaledSize
        {
            get
            {
                if (p_scaledSizeNeedsUpdate)
                    _scaledSize = Vector2.Scale(Size, Scale);

                p_scaledSizeNeedsUpdate = false;
                p_pointsNeedsUpdate = true;
                return _scaledSize;
            }
        }

        /// <summary>
        /// no rotation or scale
        /// </summary>
        public Vector2[] PointsRaw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        protected bool p_pointsNeedsUpdate = true;

        private Vector2[] _points = new Vector2[0];

        /// <summary>
        /// rotated and scaled points
        /// </summary>
        public Vector2[] Points
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!p_pointsNeedsUpdate) return _points;

                Vector2[] points = new Vector2[PointsRaw.Length];
                float deg2Rad = Mathf.Deg2Rad;
                Vector2 a = new Vector2(Mathf.Cos(Rotation * deg2Rad), Mathf.Sin(Rotation * deg2Rad)).Normalized;
                Vector2 b = new Vector2(-a.y, a.x);
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    Vector2 scaledPoints = Vector2.Scale(PointsRaw[i], ScaledSize);
                    points[i] = scaledPoints.x * a + scaledPoints.y * b;
                }

                _points = points;
                p_pointsNeedsUpdate = false;
                p_boundsNeedsUpdate = true;
                p_edgeNormalsNeedsUpdate = true;

                return _points;
            }
        }

        protected bool p_edgeNormalsNeedsUpdate = true;

        protected Vector2[] p_edgeNormals = new Vector2[0];

        /// <summary>
        /// 90° counterclockwise rotated edge normalized
        /// </summary>
        public virtual Vector2[] EdgeNormals
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!p_edgeNormalsNeedsUpdate) return p_edgeNormals;

                Vector2[] points = Points;
                Vector2[] normals = new Vector2[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 edge = points[(i + 1) % points.Length] - points[i];
                    normals[i] = new Vector2(-edge.y, edge.x).Normalized;
                }

                p_edgeNormals = normals;
                p_edgeNormalsNeedsUpdate = false;

                return p_edgeNormals;
            }
        }

        public override float Inertia => 1f;

        private Bounds2D _bounds;

        public override Bounds2D Bounds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!p_boundsNeedsUpdate) return _bounds;

                var ps = Points;
                Vector2 center = GlobalCenter;

                Bounds2D bounds = Bounds2D.InvertedBounds;

                for (int i = 0; i < ps.Length; i++)
                {
                    bounds.IncludePoint(ps[i] + center);
                }

                _bounds = bounds;
                p_boundsNeedsUpdate = false;

                return bounds;
            }
        }

        public Vector3[] ClosestPointsOnShape(KonvexCollider2D collider, out float sqrDist)
        {
            sqrDist = Mathf.Infinity;
            Vector2 contact1 = default;
            Vector2 contact2 = default;
            int contactCount = 1;


            Vector2[] points = Points;
            Vector2[] otherPoints = collider.Points;

            Vector2 otherCenter = collider.GlobalCenter;
            Vector2 center = GlobalCenter;
            foreach (var point in points)
            {
                for (int i = 0; i < otherPoints.Length; i++)
                {
                    Vector2 p1 = otherPoints[i] + otherCenter;
                    Vector2 p2 = otherPoints[(i + 1) % otherPoints.Length] + otherCenter;

                    Vector2 currentCp = Vector2.ClosestPointOnLine(p1, p2, point + center, out float dist);

                    float c = Mathf.Abs(dist - sqrDist);
                    if (c < PhysicsInfo.MaxColliderDistance && currentCp != contact1)
                    {
                        contact2 = currentCp;
                        contactCount = 2;
                    }
                    else if (dist < sqrDist)
                    {
                        sqrDist = dist;
                        contact1 = currentCp;
                        contactCount = 1;

                    }
                }
            }

            foreach (var point in otherPoints)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 p1 = points[i] + center;
                    Vector2 p2 = points[(i + 1) % points.Length] + center;

                    Vector2 currentCp = Vector2.ClosestPointOnLine(p1, p2, point + otherCenter, out float dist);

                    float c = Mathf.Abs(dist - sqrDist);
                    if (c < PhysicsInfo.MaxColliderDistance && currentCp != contact1)
                    {
                        contact2 = currentCp;
                        contactCount = 2;
                    }
                    else if (dist < sqrDist)
                    {
                        sqrDist = dist;
                        contact1 = currentCp;
                        contactCount = 1;
                    }
                }
            }

            if (contactCount == 1)
                return new Vector3[] { contact1 };


            return new Vector3[] { contact1, contact2 };
        }

        /// <summary>
        /// Gets the closest point on the konvex shape to the given point p
        /// </summary>
        /// <param name="sqrDist">squared distance from point p to the closest point</param>
        public Vector2 ClosestPointOnShape(Vector2 p, out float sqrDist)
        {
            Vector2[] points = Points;
            Vector2 center = GlobalCenter;

            sqrDist = Mathf.Infinity;
            Vector2 cp = Vector2.Infinity;
            ;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 p1 = points[i] + center;
                Vector2 p2 = points[(i + 1) % points.Length] + center;

                Vector2 currentCp = Vector2.ClosestPointOnLine(p1, p2, p, out float currentSqrDist);
                if (currentSqrDist < sqrDist)
                {
                    sqrDist = currentSqrDist;
                    cp = currentCp;
                }
            }

            return cp;
        }

        public override CollisionInfo IsColliding(Collider2D other)
        {
            CollisionInfo info = other switch
            {
                CuboidCollider2D cuboid => Konvex_Cuboid_Collision(this, cuboid),
                KonvexCollider2D konvex => Konvex_Konvex_Collision(this, konvex),
                CircleCollider circle => Konvex_Circle_Collision(this, circle),
                _ => throw new ArgumentException($"{other.GetType()} is not collidable with {GetType()}"),
            };

            return info;
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
    }
}
