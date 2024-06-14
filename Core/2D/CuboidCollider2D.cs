using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public class CuboidCollider2D : KonvexCollider2D
    {
        private static readonly Vector2[] _defaultPoints = new Vector2[4]
        {
            new Vector2(-0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(-0.5f, -0.5f)
        };

        public CuboidCollider2D()
        {
            PointsRaw = _defaultPoints;
        }

        public CuboidCollider2D(Vector3 scale, Vector3 size, Rigidbody2D rigidbody, Vector3 center, float restitution, bool addToEngine = true) : base(_defaultPoints, size, scale, rigidbody, center, restitution, PrimitiveShape2D.Square, addToEngine)
        {

        }

        public override float Inertia => (Body.Mass / 12f) * (Mathf.Pow(ScaledSize.x, 2) + Mathf.Pow(ScaledSize.y, 2));


        public override CollisionInfo IsColliding(Collider2D other)
        {
            CollisionInfo info = other switch
            {
                CuboidCollider2D cuboidCol => Konvex_Konvex_Collision(this, cuboidCol),
                KonvexCollider2D konvex => Konvex_Konvex_Collision(this, konvex),
                CircleCollider sphereCol => Konvex_Circle_Collision(this, sphereCol),
                _ => throw new ArgumentException($"{other.GetType()} is not collidable with {GetType()}"),
            };

            //CollisionResponse(other, info);

            return info;
        }
    }
}
