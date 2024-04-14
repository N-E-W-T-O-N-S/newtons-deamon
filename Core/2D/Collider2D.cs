﻿using NEWTONS.Debuger;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public abstract class Collider2D : IRigidbodyReference2D
    {
        private List<IColliderReference2D> _references = new List<IColliderReference2D>();
        private bool _isDsposed = false;

        public Action? OnUpdateScale;

        public Rigidbody2D Body;
        public Vector2 Center;
        public Vector2 CenterOfMass;
        public PrimitiveShape2D Shape { get; }

        public Collider2D()
        {
            Scale = new Vector2(1, 1);
        }

        public Collider2D(Rigidbody2D rigidbody, Vector2 scale, Vector2 center, Vector2 centerOfMass, PrimitiveShape2D shape, bool addToEngine = true)
        {
            Body = rigidbody;
            Scale = scale;
            Center = center;
            CenterOfMass = centerOfMass;
            Shape = shape;
            if (addToEngine)
                AddToPhysicsEngine();
        }

        public Vector2 scale;

        /// <summary>
        /// the parent scale of the collider
        /// </summary>
        public virtual Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                OnUpdateScale?.Invoke();
            }
        }

        public virtual Vector3 ScaleNoNotify { set => scale = value; }

        /// <summary>
        /// the global center of the collider
        /// </summary>
        public virtual Vector2 GlobalCenter => Center + Body.Position;

        public virtual float Rotation => Body.Rotation;

        public abstract float GetInertia();

        public abstract CollisionInfo IsColliding(Collider2D other);

        internal static CollisionInfo Konvex_Cuboid_Collision(KonvexCollider2D coll1, CuboidCollider2D coll2) => Konvex_Konvex_Collision(coll1, coll2);

        internal static CollisionInfo Konvex_Konvex_Collision(KonvexCollider2D coll1, KonvexCollider2D coll2)
        {
            CollisionInfo info = new CollisionInfo()
            {
                didCollide = false
            };

            if (coll1.Body.IsStatic && coll2.Body.IsStatic)
                return info;

            Vector2[] aEdgeNormals = coll1.EdgeNormals;
            Vector2[] bEdgeNormals = coll2.EdgeNormals;
            Vector2[] aScaledPoints = coll1.Points;
            Vector2[] bScaledPoints = coll2.Points;
            // Maybe do not concat
            Vector2[] axisToCheck = aEdgeNormals.Concat(bEdgeNormals).ToArray();

            float depth = Mathf.Infinity;
            Vector2 normal = Vector2.Zero;

            for (int i = 0; i < axisToCheck.Length; i++)
            {
                float aMin = Mathf.Infinity;
                float aMax = Mathf.NegativeInfinity;
                float bMin = Mathf.Infinity;
                float bMax = Mathf.NegativeInfinity;

                for (int j = 0; j < aScaledPoints.Length; j++)
                {
                    float dot = Vector2.Dot(axisToCheck[i], aScaledPoints[j] + coll1.GlobalCenter);
                    if (dot < aMin)
                        aMin = dot;
                    if (dot > aMax)
                        aMax = dot;
                }

                for (int j = 0; j < bScaledPoints.Length; j++)
                {
                    float dot = Vector2.Dot(axisToCheck[i], bScaledPoints[j] + coll2.GlobalCenter);
                    if (dot < bMin)
                        bMin = dot;
                    if (dot > bMax)
                        bMax = dot;
                }

                if (aMin >= bMax || bMin >= aMax)
                    return info;

                float d = Mathf.Min(aMax - bMin, bMax - aMin);
                if (d < depth)
                {
                    depth = d;
                    normal = axisToCheck[i];
                }

            }

            Vector2 dir = coll2.GlobalCenter - coll1.GlobalCenter;

            if (Vector2.Dot(dir, normal) > 0)
                normal = -normal;

            float velocityB1 = coll1.Body.Velocity.magnitude;
            float velocityB2 = coll2.Body.Velocity.magnitude;
            float combinedVelocity = velocityB1 + velocityB2;

            if (combinedVelocity == 0)
            {
                combinedVelocity = 1;
                if (coll1.Body.IsStatic)
                    velocityB2 = 1;
                else if (coll2.Body.IsStatic)
                    velocityB1 = 1;
                else
                {
                    velocityB1 = 0.5f;
                    velocityB2 = 0.5f;
                }
            }

            float depth1 = (velocityB1 / combinedVelocity) * depth;
            float depth2 = (velocityB2 / combinedVelocity) * depth;


            coll1.Body.MoveToPosition(coll1.Body.Position + (normal * depth1));
            coll2.Body.MoveToPosition(coll2.Body.Position + (-normal * depth2));

            info.didCollide = true;
            info.normal = normal;

            return info;
        }

        internal static CollisionInfo Circle_Circle_Collision(CircleCollider coll1, CircleCollider coll2)
        {
            CollisionInfo info = new CollisionInfo()
            {
                didCollide = false
            };

            Vector2 direction = coll1.GlobalCenter - coll2.GlobalCenter;
            Vector2 normDir = direction.Normalized;
            float distance = direction.magnitude;
            float combinedRadius = coll1.ScaledRadius + coll2.ScaledRadius;

            if (distance >= combinedRadius)
                return info;

            float depth = combinedRadius - distance;

            float velocityB1 = coll1.Body.IsStatic ? 0 : coll1.Body.Velocity.magnitude;
            float velocityB2 = coll2.Body.IsStatic ? 0 : coll2.Body.Velocity.magnitude;
            float combinedVelocity = velocityB1 + velocityB2;

            float depth1;
            float depth2;

            depth1 = (velocityB1 / combinedVelocity) * depth;
            depth2 = (velocityB2 / combinedVelocity) * depth;

            coll1.Body.MoveToPosition(coll1.Body.Position + (normDir * depth1));
            coll2.Body.MoveToPosition(coll2.Body.Position - (normDir * depth2));

            info.didCollide = true;
            info.normal = normDir;

            return info;
        }

        internal static CollisionInfo Konvex_Circle_Collision(KonvexCollider2D konvex, CircleCollider circle)
        {
            CollisionInfo info = new CollisionInfo()
            {
                didCollide = false,
            };

            List<Vector2> axisToCheck = konvex.EdgeNormals.ToList();

            Vector2[] points = konvex.Points;

            Vector2 circleCenter = circle.GlobalCenter;
            Vector2 konvexCenter = konvex.GlobalCenter;

            float radius = circle.ScaledRadius;
            float diameter = radius * 2;

            Vector2 circleVN = circle.Body.Velocity.Normalized;
            Vector2 konvexVN = konvex.Body.Velocity.Normalized;

            Vector2 circleCornerDir = new Vector2();
            float sqrDist = Mathf.Infinity;

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 dirX = points[i] + konvexCenter - circleCenter;
                float dist = dirX.sqrMagnitude;

                if (dist < sqrDist)
                {
                    circleCornerDir = dirX;
                    sqrDist = dist;
                }
            }

            axisToCheck.Add(circleCornerDir.Normalized);

            Vector2[] normals = konvex.EdgeNormals;

            float depth = Mathf.Infinity;
            Vector2 normal = Vector2.Zero;
            Vector2 minCorner = Vector2.Zero;
            Vector2 maxCorner = Vector2.Zero;

            for (int i = 0; i < axisToCheck.Count; i++)
            {
                float aMin = Mathf.Infinity;
                float aMax = Mathf.NegativeInfinity;

                // Konvex projection
                for (int j = 0; j < points.Length; j++)
                {
                    float dot = Vector2.Dot(axisToCheck[i], points[j] + konvexCenter);
                    if (dot < aMin)
                        aMin = dot;
                    if (dot > aMax)
                        aMax = dot;
                }

                // Circle projection
                // maybe more performant (funny)
                float diff = diameter * (axisToCheck[i].x * axisToCheck[i].x + axisToCheck[i].y * axisToCheck[i].y); // difference between bMin and bMax
                float bMax = Vector2.Dot(axisToCheck[i], circleCenter + axisToCheck[i] * radius);
                float bMin = bMax - diff;

                if (aMin >= bMax || bMin >= aMax)
                    return info;

                float d = Mathf.Min(aMax - bMin, bMax - aMin);
                if (d < depth)
                {
                    depth = d;
                    normal = axisToCheck[i];
                    if (i < points.Length)
                    {
                        minCorner = points[(i + 1) % points.Length] + konvexCenter;
                        maxCorner = points[i] + konvexCenter;
                    }
                    else
                    {
                        minCorner = Vector2.Zero;
                        maxCorner = Vector2.Zero;
                    }
                }
            }

            Vector2 kTOc = circle.GlobalCenter - konvex.GlobalCenter;

            Vector2 directedNormal;

            if (Vector2.Dot(kTOc, normal) > 0)
                directedNormal = -normal;
            else
                directedNormal = normal;


            // TODO: Implement velocity based collision resolution

            Vector2 dp = circleCenter + normal * radius;
            Vector2 lineP = dp - normal * depth;

            // Circle resolution
            // <--------------->
            float circleDepth = 0;
            if (circleVN != Vector2.Zero)
            {
                if (!normals.Contains(normal))
                {
                    Vector2 hitP_0 = directedNormal * radius - directedNormal * depth;
                    float a = circleVN.x * circleVN.x + circleVN.y * circleVN.y;
                    float b = 2 * hitP_0.x * circleVN.x + 2 * hitP_0.y * circleVN.y;
                    float c = hitP_0.x * hitP_0.x + hitP_0.y * hitP_0.y - radius * radius;

                    float determinant = b * b - 4 * a * c;
                    if (determinant >= 0)
                        circleDepth = (-b + Mathf.Sqrt(determinant)) / 2 * a;
                }
                else
                {
                    float lineD = -(lineP.x * directedNormal.x + lineP.y * directedNormal.y);
                    float dividend = -lineD - dp.x * directedNormal.x - dp.y * directedNormal.y;
                    float divisor = -circleVN.x * directedNormal.x - circleVN.y * directedNormal.y;

                    if (divisor != 0)
                    {
                        float d = dividend / divisor;
                        float min = Vector2.Distance(dp, minCorner);
                        float max = Vector2.Distance(dp, maxCorner);
                        Debug.Log(Mathf.Abs(d) + " min: " + Mathf.Min(min, max));
                        Debug.Log("normal: " + normal);
                        Debug.Log("corner1: " + minCorner + min + "corner2: " + maxCorner + max);
                        d = Mathf.Clamp(Mathf.Abs(d), 0, Mathf.Min(min, max));
                        circleDepth = d;
                    }
                }
            }
            // <--------------->

            // Konvex resolution
            // <--------------->
            float konvexDepth = 0;
            if (konvexVN != Vector2.Zero)
            {
                if (!normals.Contains(normal))
                {
                    Vector2 hitP_0 = directedNormal * radius - directedNormal * depth;
                    float a = konvexVN.x * konvexVN.x + konvexVN.y * konvexVN.y;
                    float b = -2 * hitP_0.x * konvexVN.x - 2 * hitP_0.y * konvexVN.y;
                    float c = hitP_0.x * hitP_0.x + hitP_0.y * hitP_0.y - radius * radius;

                    float determinant = b * b - 4 * a * c;
                    if (determinant >= 0)
                        konvexDepth = (-b + Mathf.Sqrt(determinant)) / 2 * a;
                }
                else
                {
                    float df = -(lineP.x * directedNormal.x + lineP.y * directedNormal.y);
                    float dividend = -df - dp.x * directedNormal.x - dp.y * directedNormal.y;
                    float divisor = konvexVN.x * directedNormal.x + konvexVN.y * directedNormal.y;

                    if (divisor != 0)
                        konvexDepth = dividend / divisor;
                }
            }
            // <--------------->


            //float velocityDirectionPercentage = Mathf.Abs(Vector2.Dot(konvexVN, circleVN));

            //float depth1 = depth * (velocityDirectionPercentage / 2);
            //float depth2 = depth * (1 - velocityDirectionPercentage / 2);


            konvex.Body.MoveToPosition(konvex.Body.Position + (-konvexVN * konvexDepth));
            circle.Body.MoveToPosition(circle.Body.Position + (-circleVN * circleDepth));

            info.didCollide = true;
            info.normal = normal;

            return info;
        }

        public void AddToPhysicsEngine()
        {
            if (!Physics2D.Colliders.Contains(this))
                Physics2D.Colliders.Add(this);
        }

        public void AddReference(IColliderReference2D reference)
        {
            _references.Add(reference);
        }

        public void Dispose()
        {
            if (!_isDsposed)
                return;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _isDsposed = true;
        }

        public IRigidbodyReference2D SetRigidbody(Rigidbody2D rigidbody)
        {
            Body = rigidbody;
            return this;
        }

    }
}
