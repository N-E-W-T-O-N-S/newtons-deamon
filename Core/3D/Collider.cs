using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NEWTONS.Core._3D
{
    [System.Serializable]
    public abstract class Collider : IRigidbodyReference
    {
        private List<IColliderReference> _references = new List<IColliderReference>();
        private bool _isDisposed = false;

        public event Action? OnUpdateScale;

        public Collider()
        {
            Scale = new Vector3(1, 1, 1);
        }

        public Collider(Vector3 scale, Rigidbody rigidbody, Vector3 center, PrimitiveShape shape, float restitution, bool addToEngine = true)
        {
            Body = rigidbody;
            Center = center;
            Shape = shape;
            Restitution = restitution;
            Scale = scale;
            if (addToEngine)
                AddToPhysicsEngine();
        }

        public Rigidbody? Body;
        public Vector3 Center;
        public PrimitiveShape Shape { get; }
        public float Restitution = 0.5f;


        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector3 scale;

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
        public virtual Vector3 GlobalCenter => Center + Body.Position;

        public virtual Quaternion Rotation => Body.Rotation;

        // <---------------------------------->

        public abstract CollisionInfo IsColliding(Collider other);

        public virtual void CollisionResponse(Collider other, CollisionInfo info)
        {
            if (!info.didCollide)
                return;

            Rigidbody firstBody = Body;
            Rigidbody secondBody = other.Body;

            float m1 = firstBody.Mass;
            float m2 = secondBody.Mass;
            Vector3 v1 = firstBody.Velocity;
            Vector3 v2 = secondBody.Velocity;
            float e = (Restitution + other.Restitution) / 2;
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
            List<Vector3> edges1 = new List<Vector3>();
            List<Vector3> edges2 = new List<Vector3>();

            Vector3[] points1 = coll1.Points;
            Vector3[] points2 = coll2.Points;

            Vector3[] normals1 = coll1.Normals;
            Vector3[] normals2 = coll2.Normals;

            List<Vector3> axisToCheck = new List<Vector3>();

            for (int i = 0; i < coll1.Indices.Length; i += 2)
            {
                Vector3 v = (points1[coll1.Indices[i]] - points1[coll1.Indices[i + 1]]).Normalized;
                edges1.Add(v);
            }

            for (int i = 0; i < coll2.Indices.Length; i += 2)
            {
                Vector3 v = (points2[coll2.Indices[i]] - points2[coll2.Indices[i + 1]]).Normalized;
                edges2.Add(v);
            }

            axisToCheck.AddRange(normals1);
            axisToCheck.AddRange(normals2);

            // HELP THIS CREATES A STACK OVERFLOW ERROR :(((((
            // axisToCheck = axisToCheck.Distinct().ToList();

            float depth = Mathf.Infinity;
            Vector3 normal = Vector2.Zero;

            for (int i = 0; i < axisToCheck.Count; i++)
            {
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

        public void AddToPhysicsEngine()
        {
            if (!Physics.Colliders.Contains(this))
                Physics.Colliders.Add(this);
        }

        public void AddReference(IColliderReference reference)
        {
            _references.Add(reference);
        }

        public void Dispose()
        {
            if (!_isDisposed)
                return;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _isDisposed = true;
        }

        public IRigidbodyReference SetRigidbody(Rigidbody rigidbody)
        {
            Body = rigidbody;
            return this;
        }
    }
}
