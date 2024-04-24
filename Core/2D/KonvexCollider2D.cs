using System;
using System.Collections.Generic;
using System.Linq;
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

        protected KonvexCollider2D()
        {
            Size = new Vector2(1, 1);
            PointsRaw = _defaultPoints;
        }

        public KonvexCollider2D(Vector2[] points, Vector2 size, Vector2 scale, Rigidbody2D rigidbody, Vector2 center, PrimitiveShape2D shape, bool addToEngine = true) : base(rigidbody, scale, center, shape, addToEngine)
        {
            Size = size;
            PointsRaw = points;
        }

        public Vector2 size;

        public virtual Vector2 Size { get => size; set => size = new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y)); }

        public virtual Vector2 ScaledSize => Vector2.Scale(Size, Scale);

        /// <summary>
        /// no rotation or scale
        /// </summary>
        public Vector2[] PointsRaw;

        /// <summary>
        /// rotated and scaled points
        /// </summary>
        public Vector2[] Points
        {
            get
            {
                Vector2[] points = new Vector2[PointsRaw.Length];
                float deg2Rad = Mathf.Deg2Rad;
                Vector2 a = new Vector2(Mathf.Cos(Rotation * deg2Rad), Mathf.Sin(Rotation * deg2Rad)).Normalized;
                Vector2 b = new Vector2(-a.y, a.x);
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    Vector2 scaledPoints = Vector2.Scale(PointsRaw[i], ScaledSize);
                    points[i] = scaledPoints.x * a + scaledPoints.y * b;
                }
                return points;
            }
        }

        [Obsolete("Use Points instead")]
        public Vector2[] RotatedPoints
        {
            get
            {
                Vector2[] rotatedPoints = new Vector2[PointsRaw.Length];
                float deg2Rad = Mathf.Deg2Rad;
                Vector2 a = new Vector2(Mathf.Cos(Rotation * deg2Rad), Mathf.Sin(Rotation * deg2Rad)).Normalized;
                Vector2 b = new Vector2(-a.y, a.x);
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    rotatedPoints[i] = PointsRaw[i].x * a + PointsRaw[i].y * b;
                }
                return rotatedPoints;
            }
        }

        [Obsolete("Use Points instead")]
        public Vector2[] ScaledPoints
        {
            get
            {
                Vector2[] scaledPoints = new Vector2[PointsRaw.Length];
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    scaledPoints[i] = Vector2.Scale(PointsRaw[i], ScaledSize);
                }
                return scaledPoints;
            }
        }

        /// <summary>
        /// 90° counterclockwise rotated edge normalized
        /// </summary>
        public Vector2[] EdgeNormals
        {
            get
            {
                Vector2[] points = Points;
                Vector2[] normals = new Vector2[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 edge = points[(i + 1) % points.Length] - points[i];
                    normals[i] = new Vector2(-edge.y, edge.x).Normalized;
                }
                return normals;
            }
        }

        public override float Inertia => 1f;

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
            Vector2 cp = new Vector2(Mathf.Infinity, Mathf.Infinity);
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

            // TODO: Collision response

            return info;
        }
    }
}
