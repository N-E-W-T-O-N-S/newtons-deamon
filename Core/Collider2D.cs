using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class Collider2D : IKinematicBodyReference2D
    {
        private List<IColliderReference2D> _references = new List<IColliderReference2D>();
        private bool _isDsposed = false;

        public KinematicBody2D Body;
        public Vector2 Center;
        //public float Rotation;
        public Vector2 CenterOfMass;
        public PrimitiveShape2D Shape { get; }

        public Collider2D()
        {

        }

        public Collider2D(KinematicBody2D kinematicBody, Vector2 scale, Vector2 center, Vector2 centerOfMass, PrimitiveShape2D shape)
        {
            Scale = scale;
            Body = kinematicBody;
            Center = center;
            //Rotation = rotation;
            CenterOfMass = centerOfMass;
            Shape = shape;
            Physics2D.Colliders.Add(this);
        }

        [Obsolete("Use GlobalScales instead")]
        public Vector2 globalScale;

        /// <summary>
        /// the global scale of the collider
        /// 
        /// <para> returns the set global scale and multiplies it by a collider scale <seealso cref="Scale"/> </para>
        /// 
        /// </summary>
        public Vector2 GlobalScales
        {
            get => Vector2.ComponentMultiply(globalScale, Scale);
            set => globalScale = value;
        }

        [Obsolete("Use Scale instead")]
        public Vector2 scale;

        public Vector2 Scale
        {
            get => scale;
            set
            {
                scale = new Vector2(Mathf.Max(value.x, 0), Mathf.Max(value.y, 0));

            }
        }

        /// <summary>
        /// the global center of the collider
        /// </summary>
        public Vector2 GlobalCenter 
        { 
            get => Center + Body.Position; 
        }

        public void AddReference(IColliderReference2D reference)
        {
            _references.Add(reference);
        }

        public void Dispose()
        {
            if (!_isDsposed)
                return;
            Body = null;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _isDsposed = true;
        }

        public IKinematicBodyReference2D SetKinematicBody(KinematicBody2D kinematicBody)
        {
            Body = kinematicBody;
            return this;
        }

    }
}
