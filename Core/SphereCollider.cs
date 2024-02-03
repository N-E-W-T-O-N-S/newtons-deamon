using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class SphereCollider : Collider
    {
        public SphereCollider()
        {
            Radius = 0.5f;
        }

        public SphereCollider(float radius, Vector3 scale, Rigidbody rigidbody, Vector3 center, PrimitiveShape shape, float restitution, bool addToEngine = true) : base(scale, rigidbody, center, shape, restitution, addToEngine)
        {
            Radius = radius;
        }

        public float radius;

        public float Radius { get => radius; set => radius = Mathf.Max(value, 0); }

        public float ScaledRadius => Radius * Mathf.Max(Mathf.Abs(Scale.x), Mathf.Max(Mathf.Abs(Scale.y), Mathf.Abs(Scale.z)));

        public override CollisionInfo IsColliding(Collider other)
        {
            CollisionInfo info = other switch
            {
                SphereCollider sphereCol => Sphere_Sphere_Collision(this, sphereCol),
                CuboidCollider cuboidCol => Cuboid_Sphere_Collision(cuboidCol, this),
                _ => throw new ArgumentException($"{other.GetType()} is not collidable with {GetType()}"),
            };

            CollisionResponse(other, info);

            return info;
        }
    }
}
