using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class Collider : IKinematicBodyReference, IDisposable
    {
        private List<IColliderReference> _references = new List<IColliderReference>();
        private bool _isDsposed = false;

        public Collider()
        {
            //Physics.Collideres.Add(this);
        }

        public Collider(KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape)
        {
            Scale = scale;
            Body = kinematicBody;
            Center = center;
            Shape = shape;
            Physics.Collideres.Add(this);
        }
        
        public KinematicBody Body;
        public Vector3 Center;
        public PrimitiveShape Shape { get; }


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
            if (!_isDsposed)
                return;
            Body = null;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _isDsposed = true;
        }

        public IKinematicBodyReference SetKinematicBody(KinematicBody kinematicBody)
        {
            Body = kinematicBody;
            return this;
        }
    }
}
