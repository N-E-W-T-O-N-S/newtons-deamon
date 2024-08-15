using NEWTONS.Core._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core._3D
{
    [System.Serializable]
    public abstract class Collider : IRigidbodyReference
    {
        private List<IColliderReference> _references = new List<IColliderReference>();

        /// <summary>
        /// Is this Collider already disposed
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
            get => Physics.Colliders.Contains(this);
        }

        public event Action? OnUpdateScale;

        public Rigidbody Body;

        private Vector3 _center;

        public virtual Vector3 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _center;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                _center = value;
                p_boundsNeedsUpdate = true;
                p_globalCenterNeedsUpdate = true;
            }
        }

        public Collider()
        {
            Scale = new Vector3(1, 1, 1);
            Body = new Rigidbody();
        }

        public Collider(Rigidbody rigidbody, Vector2 scale, Vector2 center, float restitution, bool addToEngine)
        {
            Body = rigidbody;
            Scale = scale;
            Center = center;
            Restitution = restitution;
            Body.AddReference(this);

            if (addToEngine)
                AddToPhysicsEngine();
        }

        private Vector3 _scale;

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
        } = 0.5f;

        protected bool p_globalCenterNeedsUpdate = true;

        private Vector3 _globalCenter;

        /// <summary>
        /// the global center of the collider
        /// </summary>
        public virtual Vector3 GlobalCenter
        {
            get
            {
                if (p_globalCenterNeedsUpdate)
                    _globalCenter = Body.Position + Center;

                p_globalCenterNeedsUpdate = false;
                return _globalCenter;
            }
        }

        public virtual Quaternion Rotation => Body.Rotation;

        protected bool p_boundsNeedsUpdate = true;

        public abstract Bounds Bounds { get; }

        internal virtual void RotationChanged() { }

        internal virtual void PositionChanged()
        {
            p_globalCenterNeedsUpdate = true;
        }

        // <---------------------------------->

        public abstract CollisionInfo IsColliding(Collider other);

        internal static void CollisionResponse(Collider c1, Collider c2, CollisionInfo info)
        {
            if (!info.didCollide)
                return;

            Rigidbody firstBody = c1.Body;
            Rigidbody secondBody = c2.Body;

            float m1 = firstBody.Mass;
            float m2 = secondBody.Mass;
            Vector3 v1 = firstBody.Velocity;
            Vector3 v2 = secondBody.Velocity;
            float e = (c1.Restitution + c2.Restitution) / 2;
            float meff;

            //Vector3 n = (pos2 - pos1).Normalized;
            Vector3 n = info.normal;


            if (secondBody.IsStatic)
            {
                meff = m1;
                v2 = Vector3.Zero;
            }
            else if (firstBody.IsStatic)
            {
                meff = m2;
                v1 = Vector3.Zero;
            }
            else
                meff = 1 / ((1 / m1) + (1 / m2));

            if (v1 == Vector3.Zero && v2 == Vector3.Zero)
                return;

            float vimp = Vector3.Dot(n, v1 - v2);
            float j = ((1 + e) * meff) * vimp;
            Vector3 dv1;
            Vector3 dv2;

            if (firstBody.IsStatic)
            {
                dv1 = new Vector3(0, 0, 0);
                dv2 = (j / m2) * n;
            }
            else if (secondBody.IsStatic)
            {
                dv1 = -(j / m1) * n;
                dv2 = new Vector3(0, 0, 0);
            }
            else
            {
                dv1 = -(j / m2) * n;
                dv2 = (j / m2) * n;
            }

            firstBody.Velocity += dv1;
            secondBody.Velocity += dv2;
        }

        internal static CollisionInfo Cuboid_Cuboid_Collision(CuboidCollider coll1, CuboidCollider coll2)
        {
            Vector3[] edges1 = new Vector3[coll1.Indices.Length / 2];
            Vector3[] edges2 = new Vector3[coll2.Indices.Length / 2];

            Vector3[] points1 = coll1.Points;
            Vector3[] points2 = coll2.Points;

            Vector3[] axisToCheck = coll1.Normals.Concat(coll2.Normals).ToArray();

            for (int i = 0, k = 0; i < edges1.Length; i++, k += 2)
            {
                Vector3 v = (points1[coll1.Indices[k]] - points1[coll1.Indices[k + 1]]).Normalized;
                edges1[i] = v;
            }

            for (int i = 0, k = 0; i < edges2.Length; i++, k += 2)
            {
                Vector3 v = (points2[coll2.Indices[k]] - points2[coll2.Indices[k + 1]]).Normalized;
                edges2[i] = v;
            }

            // HELP THIS CREATES A STACK OVERFLOW ERROR :(((((
            // axisToCheck = axisToCheck.Distinct().ToList();

            float depth = Mathf.Infinity;
            Vector3 normal = Vector3.Zero;

            //HashSet<Vector3> checkedAxis = new HashSet<Vector3>();

            for (int i = 0; i < axisToCheck.Length; i++)
            {
                //if (checkedAxis.Contains(axisToCheck[i]) || checkedAxis.Contains(-axisToCheck[i]))
                //    continue;

                //checkedAxis.Add(axisToCheck[i]);

                float aMin = Mathf.Infinity;
                float aMax = Mathf.NegativeInfinity;
                float bMin = Mathf.Infinity;
                float bMax = Mathf.NegativeInfinity;

                for (int j = 0; j < points1.Length; j++)
                {
                    float dot = Vector3.Dot(axisToCheck[i], points1[j] + coll1.GlobalCenter);
                    if (dot < aMin)
                        aMin = dot;
                    if (dot > aMax)
                        aMax = dot;
                }

                for (int j = 0; j < points2.Length; j++)
                {
                    float dot = Vector3.Dot(axisToCheck[i], points2[j] + coll2.GlobalCenter);
                    if (dot < bMin)
                        bMin = dot;
                    if (dot > bMax)
                        bMax = dot;
                }

                if (aMin >= bMax || bMin >= aMax)
                {
                    return new CollisionInfo()
                    {
                        didCollide = false
                    };
                }

                float d = Mathf.Min(aMax - bMin, bMax - aMin);
                if (d < depth)
                {
                    depth = d;
                    normal = axisToCheck[i];
                }
            }

            Vector3 dir = coll2.GlobalCenter - coll1.GlobalCenter;

            if (Vector3.Dot(dir, normal) > 0)
                normal = -normal;

            float velocityB1 = coll1.Body.IsStatic ? 0 : coll1.Body.Velocity.magnitude;
            float velocityB2 = coll2.Body.IsStatic ? 0 : coll2.Body.Velocity.magnitude;
            float combinedVelocity = velocityB1 + velocityB2;

            // TEMP SOLUTION BECAUSE THERE ARE NO ROTATIONAL FORCES
            // <--------------------------------------------------->
            float depth1;
            float depth2;

            if (combinedVelocity > 0)
            {
                depth1 = (velocityB1 / combinedVelocity) * depth;
                depth2 = (velocityB2 / combinedVelocity) * depth;
            }
            else
            {
                float combinedMass = coll1.Body.Mass + coll2.Body.Mass;
                depth1 = coll1.Body.IsStatic ? 0 : (coll1.Body.Mass / combinedMass) * depth;
                depth2 = coll2.Body.IsStatic ? 0 : (coll2.Body.Mass / combinedMass) * depth;
            }
            // <--------------------------------------------------->

            coll1.Body.MoveToPosition(coll1.Body.Position + (normal * depth1));
            coll2.Body.MoveToPosition(coll2.Body.Position - (normal * depth2));

            CollisionInfo info = new CollisionInfo()
            {
                didCollide = true,
                normal = normal
            };

            return info;
        }
        
        internal static CollisionInfo Cuboid_Sphere_Collision(CuboidCollider cuboid, SphereCollider sphere)
        {
            CollisionInfo info = new CollisionInfo()
            {
                didCollide = false,
            };

            List<Vector3> axisToCheck = new List<Vector3>();
            
            Vector3[] points = cuboid.Points;
            Vector3[] normals = cuboid.Normals;
            
            // TODO

            return info;
        }

        internal static CollisionInfo Sphere_Sphere_Collision(SphereCollider coll1, SphereCollider coll2)
        {
            CollisionInfo info = new CollisionInfo()
            {
                didCollide = false,
            };

            Vector3 direction = coll1.GlobalCenter - coll2.GlobalCenter;
            Vector3 normDir = direction.Normalized;
            float dist = direction.magnitude;
            float combinedRadius = coll1.ScaledRadius + coll2.ScaledRadius;

            if (dist >= combinedRadius)
                return info;

            float depth = combinedRadius - dist;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddToPhysicsEngine()
        {
            Physics.Colliders.Add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFromPhysicsEngine()
        {
            Physics.Colliders.Remove(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddReference(IColliderReference reference)
        {
            _references.Add(reference);
        }

        internal void InformScaleChange() => OnUpdateScale?.Invoke();

        ~Collider()
        {
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
