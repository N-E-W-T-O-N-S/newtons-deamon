using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KinematicBody : IDisposable
    {
        public event Action? OnUpdatePosition;
        public event Action? OnUpdateRotation;

        private List<IKinematicBodyReference> _references = new List<IKinematicBodyReference>();
        private bool _isDisposed = false;

        /// <summary>
        /// Implements with default values
        /// <br /> <see cref="Mass"/> = 1f
        /// </summary>
        public KinematicBody()
        {
            Mass = 1f;
            Physics.Bodies.Add(this);
        }

        public KinematicBody(Vector3 position, Vector3 rotation, float drag, float mass)
        {
            Position = position;
            Rotation = rotation;
            Mass = mass;
            Drag = drag;
            Physics.Bodies.Add(this);
        }

        //Active Properties

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector3 position;

        public Vector3 Position
        {
            get => position;
            set 
            { 
                position = value;
                OnUpdatePosition?.Invoke();
            }
        }

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector3 rotation;

        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                OnUpdateRotation?.Invoke();
            }
        }

        //Passive Properties

        public Vector3 Velocity;

        public Vector3 CenterOfMass;

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public float mass;

        public float Mass
        {
            get => mass;
            set { mass = Mathf.Max(value, PhysicsInfo.MinMass); }
        }

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public float drag;

        public float Drag
        {
            get => drag;
            set { drag = Mathf.Max(value, PhysicsInfo.MinDrag); }
        }

        public bool UseGravity;

        public void MoveToPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public void MoveToRotation(Quaternion newRotation)
        {
            throw new NotImplementedException();
        }

        //TODO: Look into have deltaTime be a global variable
        public void AddForce(Vector3 force, ForceMode forceMode, float deltaTime)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    Velocity += force / Mass * deltaTime;
                    break;
                case ForceMode.Acceleration:
                    Velocity += force * deltaTime;
                    break;
                case ForceMode.Impulse:
                    Velocity += force * deltaTime / Mass;
                    break;
                case ForceMode.VelocityChange:
                    Velocity += force;
                    break;
            }
        }

        public void AddReference(IKinematicBodyReference reference)
        {
            _references.Add(reference);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _references.Clear();
            _isDisposed = true;
        }
    }
}
