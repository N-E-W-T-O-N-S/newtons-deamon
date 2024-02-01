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
            Radius = 1;
        }

        public SphereCollider(float radius, Vector3 scale, Rigidbody rigidbody, Vector3 center, PrimitiveShape shape, float restitution) : base(scale, rigidbody, center, shape, restitution)
        {
            Radius = radius;
        }

        /// <summary>
        /// radius stored in z component
        /// </summary>
        public override Vector3 Size { get => base.Size; set => base.Size = new Vector3(0, 0, Mathf.Max(value.z)); }

        public float Radius { get => Size.z; set => Size = new Vector3(0, 0, Mathf.Max(value, 0)); }

        public float ScaledRadius => Size.z * Mathf.Max(Mathf.Abs(Scale.x), Mathf.Max(Mathf.Abs(Scale.y), Mathf.Abs(Scale.z)));

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
