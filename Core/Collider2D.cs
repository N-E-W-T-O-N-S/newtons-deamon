using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class Collider2D : IRigidbodyReference2D
    {
        private List<IColliderReference2D> _references = new List<IColliderReference2D>();
        private bool _isDsposed = false;

        public Action? OnUpdateScale;

        public Rigidbody2D Body;
        public Vector2 Center;
        public Vector2 CenterOfMass;
        public PrimitiveShape2D Shape { get; }

        public Collider2D()
        {
            Size = new Vector2(1, 1);
            Scale = new Vector2(1, 1);
        }

        public Collider2D(Rigidbody2D rigidbody, Vector2 scale, Vector2 center, Vector2 centerOfMass, PrimitiveShape2D shape, bool addToEngine = true)
        {
            Size = scale;
            Body = rigidbody;
            Center = center;
            CenterOfMass = centerOfMass;
            Shape = shape;
            if (addToEngine)
                AddToPhysicsEngine();
        }

        public Vector2 scale;

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

        public Vector2 size;

        public virtual Vector2 Size { get => size; set => size = new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y)); }

        public virtual Vector2 ScaledSize => Vector2.Scale(Size, Scale);

        /// <summary>
        /// the global center of the collider
        /// </summary>
        public virtual Vector2 GlobalCenter => Center + Body.Position; 

        public virtual float Rotation => Body.Rotation;

        public void AddToPhysicsEngine()
        {
            if (!Physics2D.Colliders.Contains(this))
                Physics2D.Colliders.Add(this);
        }

        public void AddReference(IColliderReference2D reference)
        {
            _references.Add(reference);
        }

        public void Dispose()
        {
            if (!_isDsposed)
                return;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _isDsposed = true;
        }

        public IRigidbodyReference2D SetRigidbody(Rigidbody2D rigidbody)
        {
            Body = rigidbody;
            return this;
        }

    }
}
