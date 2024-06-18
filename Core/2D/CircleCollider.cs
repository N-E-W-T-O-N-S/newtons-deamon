﻿using System;
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

        public CircleCollider(float radius, Rigidbody2D rigidbody, Vector2 scale, Vector2 center, float restitution, PrimitiveShape2D shape, bool addToEngine = true) : base(rigidbody, scale, center, restitution, shape, addToEngine)
        { 
            Radius = radius;
        }

        private float _radius;

        public float Radius { get => _radius; set => _radius = Mathf.Max(value, 0); }

        public float ScaledRadius => _radius * Mathf.Max(Mathf.Abs(Scale.x), Math.Max(Mathf.Abs(Scale.y), Mathf.Abs(Scale.z)));

        public override float Inertia => 1f;

        public override Bounds2D Bounds
        {
            get
            {
                return new Bounds2D(ScaledRadius, GlobalCenter);
            }
        }

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
