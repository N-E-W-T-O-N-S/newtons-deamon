using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider2D : Collider2D
    {

        private readonly Vector2[] _defaultPoints = new Vector2[]
        {
            //Points of a hexagon start right and go clockwise
            new Vector2(0.5f, 0),
            new Vector2(0.25f, 0.433f),
            new Vector2(-0.25f, 0.433f),
            new Vector2(-0.5f, 0),
            new Vector2(-0.25f, -0.433f),
            new Vector2(0.25f, -0.433f)
        };

        [Obsolete]
        public KonvexCollider2D()
        {
            PointsRaw = _defaultPoints;
            Scale = new Vector2(1, 1);
            GlobalScales = new Vector2(1, 1);
        }

        public KonvexCollider2D(Vector2[] points, KinematicBody2D kinematicBody, Vector2 scale, Vector2 center, float rotation, Vector2 centerOfMass, PrimitiveShape2D shape) : base(kinematicBody, scale, center, rotation, centerOfMass, shape)
        {
            PointsRaw = points;
        }

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
                    Vector2 scaledPoints = new Vector2(PointsRaw[i].x * GlobalScales.x, PointsRaw[i].y * GlobalScales.y);
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
                    scaledPoints[i] = new Vector2(PointsRaw[i].x * GlobalScales.x, PointsRaw[i].y * GlobalScales.y);
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

        public bool Collision(KonvexCollider2D other)
        {
            if (Body.Velocity == Vector2.Zero && other.Body.Velocity == Vector2.Zero)
                return false;

            Vector2[] aEdgeNormals = EdgeNormals;
            Vector2[] bEdgeNormals = other.EdgeNormals;
            Vector2[] aScaledPoints = Points;
            Vector2[] bScaledPoints = other.Points;
            // Maybe do not concat
            Vector2[] axisToCheck = aEdgeNormals.Concat(bEdgeNormals).ToArray();

            float depth = Mathf.Infinity;
            Vector2 normal = Vector2.Zero;

            //TODO: optimisation
            for (int i = 0; i < axisToCheck.Length; i++)
            {
                float aMin = Mathf.Infinity;
                float aMax = Mathf.NegativeInfinity;
                float bMin = Mathf.Infinity;
                float bMax = Mathf.NegativeInfinity;
                for (int j = 0; j < aScaledPoints.Length; j++)
                {
                    float dot = Vector2.Dot(axisToCheck[i], aScaledPoints[j] + Body.Position);
                    if (dot < aMin)
                        aMin = dot;
                    if (dot > aMax)
                        aMax = dot;
                }

                for (int j = 0; j < bScaledPoints.Length; j++)
                {
                    float dot = Vector2.Dot(axisToCheck[i], bScaledPoints[j] + other.Body.Position);
                    if (dot < bMin)
                        bMin = dot;
                    if (dot > bMax)
                        bMax = dot;
                }

                if (aMin >= bMax || bMin >= aMax)
                    return false;

                float d = Mathf.Min(aMax - bMin, bMax - aMin);
                if (d < depth)
                {
                    depth = d;
                    normal = axisToCheck[i];
                }

            }

            Vector2 dir = other.GlobalCenter - GlobalCenter;

            if (Vector2.Dot(dir, normal) > 0)
                normal = -normal;

            Body.MoveToPosition(Body.Position + (normal * depth));

            return true;
        }

        public void AddToPhysicsEngine()
        {
            if (!Physics2D.Colliders.Contains(this))
                Physics2D.Colliders.Add(this);
        }

    }
}
