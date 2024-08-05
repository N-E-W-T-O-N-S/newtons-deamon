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

        public CircleCollider(float radius, Rigidbody2D rigidbody, Vector2 scale, Vector2 center, float restitution, bool addToEngine = true) : base(rigidbody, scale, center, restitution, addToEngine)
        {
            Radius = radius;
        }

        public override Vector2 Scale
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
                    _scaledRadius = _radius * Mathf.Max(Mathf.Abs(Scale.x), Mathf.Abs(Scale.y));

                p_scaledRadiusNeedsUpdate = false;
                return _scaledRadius;
            }
        }

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
