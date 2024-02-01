using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class CuboidCollider : KonvexCollider
    {
        private static readonly Vector3[] defaultPoints = new Vector3[8]
        {
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f)
        };

        private static readonly int[] defaultIndices = new int[24]
        {
            0, 1, 1, 3, 2, 3, 0, 2, 4, 5, 5, 7, 6, 7, 4, 6, 0, 4, 1, 5, 2, 6, 3, 7
        };

        private static readonly Vector3[] defaultNormals = new Vector3[6]
        {
            new Vector3( 1,  0,  0),
            new Vector3(-1,  0,  0),
            new Vector3( 0,  1,  0),
            new Vector3( 0, -1,  0),
            new Vector3( 0,  0,  1),
            new Vector3( 0,  0, -1)
        };

        public CuboidCollider()
        {
            PointsRaw = defaultPoints;
            Indices = defaultIndices;
            NormalsRaw = defaultNormals;
        }

        public CuboidCollider(Vector3 scale, Rigidbody rigidbody, Vector3 center, float restitution) : base(defaultPoints, defaultIndices, defaultNormals, scale, rigidbody, center, PrimitiveShape.Cube, restitution)
        {

        }

        public override CollisionInfo IsColliding(Collider other)
        {
            CollisionInfo info = other switch
            {
                CuboidCollider cuboidCol => Cuboid_Cuboid_Collision(this, cuboidCol),
                SphereCollider sphereCol => Cuboid_Sphere_Collision(this, sphereCol),
                _ => throw new ArgumentException($"{other.GetType()} is not collidable with {GetType()}"),
            };

            CollisionResponse(other, info);

            return info;
        }
    }
}
