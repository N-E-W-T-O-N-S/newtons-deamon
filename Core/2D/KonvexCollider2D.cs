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

        public KonvexCollider2D()
        {
            Size = new Vector2(1, 1);
            PointsRaw = _defaultPoints;
        }

        public KonvexCollider2D(Vector2[] points, Vector2 size, Vector2 scale, Rigidbody2D rigidbody, Vector2 center, Vector2 centerOfMass, PrimitiveShape2D shape, bool addToEngine = true) : base(rigidbody, scale, center, centerOfMass, shape, addToEngine)
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

        public override float GetInertia()
        {
            return 1;
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
