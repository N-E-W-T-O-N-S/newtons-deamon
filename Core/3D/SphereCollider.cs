using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._3D
{
    [System.Serializable]
    public class SphereCollider : Collider
    {
        public SphereCollider()
        {
            Radius = 0.5f;
        }

        public SphereCollider(float radius, Vector3 scale, Rigidbody rigidbody, Vector3 center, float restitution, bool addToEngine = true) : base(rigidbody, scale, center, restitution, addToEngine)
        {
            Radius = radius;
        }

        public override Vector3 Scale
        {
            get
            {
                return base.Scale;
            }
            set
            {
                base.Scale = value;
                p_scaledRadiusNeedsUpdate = true;
            }
        }

        private float _radius;

        public float Radius
        {
            get => _radius;
            set
            {
                _radius = Mathf.Max(value, 0);
                p_scaledRadiusNeedsUpdate = true;
            }
        }

        protected bool p_scaledRadiusNeedsUpdate = true;

        private float _scaledRadius;

        public float ScaledRadius
        {
            get
            {
                if (p_scaledRadiusNeedsUpdate)
                    _scaledRadius = _radius * Mathf.Max(Mathf.Abs(Scale.x), Mathf.Max(Mathf.Abs(Scale.y), Mathf.Abs(Scale.z)));

                p_scaledRadiusNeedsUpdate = false;
                return _scaledRadius;
            }
        }

        public override Bounds Bounds => new Bounds(ScaledRadius, GlobalCenter);

        public override CollisionInfo IsColliding(Collider other)
        {
            CollisionInfo info = other switch
            {
                SphereCollider sphereCol => Sphere_Sphere_Collision(this, sphereCol),
                CuboidCollider cuboidCol => Cuboid_Sphere_Collision(cuboidCol, this),
                _ => throw new ArgumentException($"{other.GetType()} is not collidable with {GetType()}"),
            };

            return info;
        }
    }
}
