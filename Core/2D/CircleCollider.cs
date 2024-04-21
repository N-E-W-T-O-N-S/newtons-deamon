using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public class CircleCollider : Collider2D
    {
        public CircleCollider() 
        { 
            Radius = 0.5f;
        }

        public CircleCollider(float radius, Rigidbody2D rigidbody, Vector2 scale, Vector2 center, Vector2 centerOfMass, PrimitiveShape2D shape, bool addToEngine = true) : base(rigidbody, scale, center, centerOfMass, shape, addToEngine)
        { 
            Radius = radius;
        }

        public float radius;

        public float Radius { get => radius; set => radius = Mathf.Max(value, 0); }

        public float ScaledRadius => radius * Mathf.Max(Mathf.Abs(Scale.x), Math.Max(Mathf.Abs(Scale.y), Mathf.Abs(Scale.z)));

        public override float Inertia => 1f;

        public override CollisionInfo IsColliding(Collider2D other)
        {
            CollisionInfo info = other switch
            {
                CircleCollider circle => Circle_Circle_Collision(this, circle),
                KonvexCollider2D konvex => Konvex_Circle_Collision(konvex, this),
                _ => throw new ArgumentException($"{other.GetType()} is not collidable with {GetType()}"),
            };

            return info;
        }
    }
}
