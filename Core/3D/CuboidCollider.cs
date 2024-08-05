using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace NEWTONS.Core._3D
{
    [System.Serializable]
    public class CuboidCollider : KonvexCollider
    {
        private static readonly Vector3[] _defaultPoints = new Vector3[8]
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

        private static readonly int[] _defaultIndices = new int[24]
        {
            0, 1, 1, 3, 2, 3, 0, 2, 4, 5, 5, 7, 6, 7, 4, 6, 0, 4, 1, 5, 2, 6, 3, 7
        };

        private static readonly Vector3[] _defaultNormals = new Vector3[3]
        {
            new Vector3( 1,  0,  0),
            new Vector3( 0,  1,  0),
            new Vector3( 0,  0,  1),
        };

        public CuboidCollider()
        {
            PointsRaw = _defaultPoints;
            Indices = _defaultIndices;
            NormalsRaw = _defaultNormals;
        }

        public CuboidCollider(Rigidbody rigidbody, Vector3 scale, Vector3 size, Vector3 center, float restitution, bool addToEngine = true) : base(_defaultPoints, _defaultIndices, _defaultNormals, rigidbody, scale, size, center, restitution, addToEngine)
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

            return info;
        }
    }
}
