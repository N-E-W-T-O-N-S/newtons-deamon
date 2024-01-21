using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class Collider : IKinematicBodyReference
    {
        private List<IColliderReference> _references = new List<IColliderReference>();
        private bool _isDisposed = false;

        public Collider()
        {
            
        }

        public Collider(KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape, float restitution)
        {
            Scale = scale;
            Body = kinematicBody;
            Center = center;
            Shape = shape;
            Restitution = restitution;
            Physics.Collideres.Add(this);
        }
        
        public KinematicBody Body;
        public Vector3 Center;
        public PrimitiveShape Shape { get; }
        public float Restitution = 0.5f;


        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector3 globalScale;

        /// <summary>
        /// The Global Scale of the Collider
        /// 
        /// <para> returns the set global scale and multiplies it by a collider scale <seealso cref="Scale"/> </para>
        /// 
        /// </summary>
        public Vector3 GlobalScales 
        { 
            get => Vector3.ComponentMultiply(globalScale, Scale);
            set => globalScale = value; 
        }

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector3 scale;

        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = new Vector3(Mathf.Max(value.x, 0), Mathf.Max(value.y, 0), Mathf.Max(value.z, 0));
            }
        }

        public void AddReference(IColliderReference reference)
        {
            _references.Add(reference);
        }

        public void Dispose()
        {
            if (!_isDisposed)
                return;
            Body = null;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _isDisposed = true;
        }

        public IKinematicBodyReference SetKinematicBody(KinematicBody kinematicBody)
        {
            Body = kinematicBody;
            return this;
        }
    }
}
