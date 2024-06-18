using NEWTONS.Debuger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public abstract class Collider2D : IDisposable, IRigidbodyReference2D
    {
        private readonly List<IColliderReference2D> _references = new List<IColliderReference2D>();

        /// <summary>
        /// Is this Collider2D already disposed
        /// </summary>
        public bool Disposed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        } = false;

        /// <summary>
        /// Is this Collider2D already added to the engine
        /// </summary>
        public bool AddedToPhysicsEngine
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Physics2D.Colliders.Contains(this);
        }

        public Action? OnUpdateScale;

        public event Action<CollisionInfo>? OnCollisionEnter;

        public Rigidbody2D Body;

        public Vector2 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public PrimitiveShape2D Shape { get; }

        /// <summary>
        /// serialization constructor
        /// </summary>
        public Collider2D()
        {
            Scale = new Vector2(1, 1);
            Body = new Rigidbody2D();
        }

        public Collider2D(Rigidbody2D rigidbody, Vector2 scale, Vector2 center, float restitution, PrimitiveShape2D shape, bool addToEngine)
        {
            Body = rigidbody;
            Scale = scale;
            Center = center;
            Shape = shape;
            Restitution = restitution;
            Body.AddReference(this);
            if (addToEngine)
                AddToPhysicsEngine();
        }

        private Vector2 _scale;

        /// <summary>
        /// the parent scale of the collider
        /// </summary>
        public virtual Vector3 Scale
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _scale;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _scale = value;
                InformScaleChange();
            }
        }

        public virtual Vector3 ScaleNoNotify
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _scale = value;
        }

        public float Restitution
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        /// <summary>
        /// the global center of the collider
        /// </summary>
        public virtual Vector2 GlobalCenter => Center + Body.Position;

        public abstract float Inertia { get; }

        public virtual float Rotation => Body.Rotation;

        public abstract Bounds2D Bounds { get; }

        public abstract CollisionInfo IsColliding(Collider2D other);


        /// <summary>
        /// Applies impulse to the two colliding Rigid Bodies.
        /// </summary>
        internal virtual void CollisionResponse(Collider2D other, CollisionInfo info)
        {
            if (!info.didCollide)
                return;

            Rigidbody2D riA = Body;
            Rigidbody2D riB = other.Body;

            Vector2 rAP = (Vector2)info.contactPoints[0] - (riA.CenterOfMass + GlobalCenter);
            Vector2 rBP = (Vector2)info.contactPoints[0] - (riB.CenterOfMass + other.GlobalCenter);

            Vector2 rotatedAP = new Vector2(-rAP.y, rAP.x);
            Vector2 rotatedBP = new Vector2(-rBP.y, rBP.x);

            Vector2 contact = info.contactPoints[0];

            Vector2 vAP = Body.GetVelocityAtPoint(contact);
            Vector2 vBP = other.Body.GetVelocityAtPoint(contact);
            Vector2 vAB = vBP - vAP;

            float combindeRestitution = (Restitution + other.Restitution) / 2;

            float numerator = Vector2.Dot(-(1 + combindeRestitution) * vAB, info.normal);

            float invInertiaA = riA.InvInertia;
            float invInertiaB = riB.InvInertia;
            float invMassA = riA.InvMass;
            float invMassB = riB.InvMass;

            float reducedMass = (invMassA + invMassB);
            float rotationA = (Mathf.Pow(Vector2.Dot(rotatedAP, info.normal), 2)) * invInertiaA;
            float rotationB = (Mathf.Pow(Vector2.Dot(rotatedBP, info.normal), 2)) * invInertiaB;

            float denominator = reducedMass + rotationA + rotationB;

            float j = numerator / denominator;

            riA.Velocity -= (j * invMassA) * (Vector2)info.normal;
            riB.Velocity += (j * invMassB) * (Vector2)info.normal;

            riA.AngularVelocity -= Vector2.Dot(rotatedAP, j * info.normal) * invInertiaA;
            riB.AngularVelocity += Vector2.Dot(rotatedBP, j * info.normal) * invInertiaB;
        }
        
        internal static bool BoundsOverlapCheck(Collider2D c1, Collider2D c2)
        {
            Bounds2D b1 = c1.Bounds;
            Bounds2D b2 = c2.Bounds;

            if (b1.Max.x < b2.Min.x || b1.Min.x > b2.Max.x)
                return false;
            if (b1.Max.y < b2.Min.y || b1.Min.y > b2.Max.y)
                return false;

            return true;
        }

        // TODO: custom cuboid konvex collision
        internal static CollisionInfo Konvex_Cuboid_Collision(KonvexCollider2D coll1, CuboidCollider2D coll2) => Konvex_Konvex_Collision(coll1, coll2);

        internal static CollisionInfo Konvex_Konvex_Collision(KonvexCollider2D coll1, KonvexCollider2D coll2)
        {
            CollisionInfo info = default;


            if (coll1.Body.IsStatic && coll2.Body.IsStatic)
                return info;

            if (!BoundsOverlapCheck(coll1, coll2))
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


            coll1.Body.Position += (normal * depth1);
            coll2.Body.Position += (-normal * depth2);

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
            CollisionInfo info = default;

            if (coll1.Body.IsStatic && coll2.Body.IsStatic)
                return info;

            if (!BoundsOverlapCheck(coll1, coll2))
                return info;

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

            coll1.Body.Position += (normal * depth1);
            coll2.Body.Position -= (normal * depth2);

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

            if (coll1.Body.IsStatic && coll2.Body.IsStatic)
                return info;

            if (!BoundsOverlapCheck(coll1, coll2))
                return info;

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


            coll1.Body.Position += (normal * depth1);
            coll2.Body.Position += (normal * -depth2);

            info.didCollide = true;
            info.normal = normal;
            info.contactPoints = new Vector3[] { coll1.ClosestPointOnShape(circleCenter, out _) };

            coll1.CollisionResponse(coll2, info);

            coll1.OnCollisionEnter?.Invoke(info);
            coll2.OnCollisionEnter?.Invoke(info);

            return info;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddToPhysicsEngine()
        {
            Physics2D.Colliders.Add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFromPhysicsEngine()
        {
            Physics2D.Colliders.Remove(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddReference(IColliderReference2D reference)
        {
            _references.Add(reference);
        }

        internal void InformScaleChange() => OnUpdateScale?.Invoke();

        ~Collider2D()
        {
            //Debug.Log($"Collider2D - hash: {GetHashCode()} - has been disposed!");
            Dispose();
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            if (AddedToPhysicsEngine)
                RemoveFromPhysicsEngine();

            OnUpdateScale = null;

            Body.Collider = null;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            Disposed = true;
        }
    }
}
