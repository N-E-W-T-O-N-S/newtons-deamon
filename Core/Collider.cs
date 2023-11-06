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

        }

        public Collider(KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape)
        {
            Body = kinematicBody;
            Center = center;
            Shape = shape;
            Physics.Collideres.Add(this);
        }
        
        public KinematicBody Body;
        public Vector3 Center;
        public PrimitiveShape Shape { get; }

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
