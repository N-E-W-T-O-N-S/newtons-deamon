using NEWTONS.Debuger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public abstract class Collider2D : IRigidbodyReference2D
    {
        private List<IColliderReference2D> _references = new List<IColliderReference2D>();
        private bool _isDsposed = false;

        public Action? OnUpdateScale;

        public event Action<CollisionInfo>? OnCollisionEnter;

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

        public float restitution;

        public float Restitution { get => restitution; set => restitution = value; }

        /// <summary>
        /// the global center of the collider
        /// </summary>
        public virtual Vector2 GlobalCenter => Center + Body.Position;

        public virtual float Rotation => Body.Rotation;

        public abstract float GetInertia();

        public abstract CollisionInfo IsColliding(Collider2D other);

        /// <summary>
        /// Applies impulse to the two colliding Rigid Bodies.
        /// </summary>
        public virtual void CollisionResponse(Collider2D other, CollisionInfo info)
        {
            if (!info.didCollide)
                return;

            Rigidbody2D riA = Body;
            Rigidbody2D riB = other.Body;

            Vector2 rAP = (Vector2)info.contactPoints[0] - (riA.CenterOfMass + riA.Position);
            Vector2 rBP = (Vector2)info.contactPoints[0] - (riB.CenterOfMass + riB.Position);

            Vector2 rotatedAP = new Vector2(-rAP.y, rAP.x);
            Vector2 rotatedBP = new Vector2(-rBP.y, rBP.x);

            Vector2 contact = info.contactPoints[0];

            Vector2 vAP = Body.GetVelocityOfPoint(contact);
            Vector2 vBP = other.Body.GetVelocityOfPoint(contact);
            Vector2 vAB = vBP - vAP;

            float combindeRestitution = (Restitution + other.Restitution) / 2;

            float numerator = Vector2.Dot(-(1 + combindeRestitution) * vAB, info.normal);

            float invMassA = !riA.IsStatic ? 1 / riA.Mass : 0;
            float invMassB = !riB.IsStatic ? 1 / riB.Mass : 0;

            float reducedMass = info.normal.sqrMagnitude * (invMassA + invMassB);
            float rotationA = (Mathf.Pow(Vector2.Dot(rotatedAP, info.normal), 2)) / riA.Inertia;
            float rotationB = (Mathf.Pow(Vector2.Dot(rotatedBP, info.normal), 2)) / riB.Inertia;

            float denominator = reducedMass + rotationA + rotationB;

            float j = numerator / denominator;

            riA.Velocity -= !riA.IsStatic ? (j / riA.Mass) * (Vector2)info.normal : Vector2.Zero;
            riB.Velocity += !riB.IsStatic ? (j / riB.Mass) * (Vector2)info.normal : Vector2.Zero;

            riA.AngularVelocity -= !riA.IsStatic ? Vector2.Dot(rotatedAP, j * info.normal) / riA.Inertia : 0;
            riB.AngularVelocity += !riB.IsStatic ? Vector2.Dot(rotatedBP, j * info.normal) / riB.Inertia : 0;
        }

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
            Vector2[] aPoints = coll1.Points;
            Vector2[] bPoints = coll2.Points;
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

                for (int j = 0; j < aPoints.Length; j++)
                {
                    Vector2 point = aPoints[j] + coll1.GlobalCenter;
                    float dot = Vector2.Dot(axisToCheck[i], point);
                    if (dot < aMin)
                        aMin = dot;
                    if (dot > aMax)
                        aMax = dot;
                }

                for (int j = 0; j < bPoints.Length; j++)
                {
                    Vector2 point = bPoints[j] + coll2.GlobalCenter;
                    float dot = Vector2.Dot(axisToCheck[i], point);
                    if (dot < bMin)
                        bMin = dot;
                    if (dot > bMax)
                        bMax = dot;
                }

                if (aMin >= bMax || bMin >= aMax)
                    return info;

                float a = aMax - bMin;
                float b = bMax - aMin;
                float d = Mathf.Min(a, b);

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
            info.contactPoints = coll1.ClosestPointsOnShape(coll2, out _);

            coll1.CollisionResponse(coll2, info);

            // BUFFER THOSE
            coll1.OnCollisionEnter?.Invoke(info);
            coll2.OnCollisionEnter?.Invoke(info);

            return info;
        }

        internal static CollisionInfo Circle_Circle_Collision(CircleCollider coll1, CircleCollider coll2)
        {
            CollisionInfo info = new CollisionInfo()
            {
                didCollide = false
            };

            Vector2 direction = coll1.GlobalCenter - coll2.GlobalCenter;
            Vector2 normal = direction.Normalized;
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

            coll1.Body.MoveToPosition(coll1.Body.Position + (normal * depth1));
            coll2.Body.MoveToPosition(coll2.Body.Position - (normal * depth2));

            info.didCollide = true;
            info.normal = normal;
            info.contactPoints = new Vector3[] { coll2.GlobalCenter + normal * coll2.Radius };

            coll1.CollisionResponse(coll2, info);

            coll1.OnCollisionEnter?.Invoke(info);
            coll2.OnCollisionEnter?.Invoke(info);

            return info;
        }

        internal static CollisionInfo Konvex_Circle_Collision(KonvexCollider2D coll1, CircleCollider coll2)
        {
            CollisionInfo info = default;

            Vector2[] points = coll1.Points;

            Vector2 circleCenter = coll2.GlobalCenter;
            Vector2 konvexCenter = coll1.GlobalCenter;

            float radius = coll2.ScaledRadius;
            float diameter = radius * 2;

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

            List<Vector2> axisToCheck = coll1.EdgeNormals.ToList();
            axisToCheck.Add(circleCornerDir.Normalized);

            float depth = Mathf.Infinity;
            Vector2 normal = Vector2.Zero;

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
                }
            }

            Vector2 kTOc = coll2.GlobalCenter - coll1.GlobalCenter;

            if (Vector2.Dot(kTOc, normal) > 0)
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
            info.contactPoints = new Vector3[] { coll1.ClosestPointOnShape(circleCenter, out _) };

            coll1.CollisionResponse(coll2, info);

            coll1.OnCollisionEnter?.Invoke(info);
            coll2.OnCollisionEnter?.Invoke(info);

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
