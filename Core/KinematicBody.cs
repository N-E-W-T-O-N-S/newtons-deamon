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

        [Obsolete]
        public KinematicBody()
        {
            Mass = 1f;
            Rotation = Quaternion.Identity;
        }

        public KinematicBody(Vector3 position, Quaternion rotation, float drag, float mass)
        {
            Position = position;
            Rotation = rotation;
            Mass = mass;
            Drag = drag;
            AddToPhysicsEngine();
        }

        // <----------------------->
        // Active Properties
        // <----------------------->

        [Obsolete("Use Position instead")]
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
        /// Does not invoke <see cref="OnUpdatePosition"/>
        /// </summary>
        public Vector3 PositionNoNotify
        {
            get => position;
            set => position = value;
        }

        [Obsolete("Use Rotation instead")]
        public Quaternion rotation;

        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                OnUpdateRotation?.Invoke();
            }
        }

        /// <summary>
        /// Does not invoke <see cref="OnUpdateRotation"/>
        /// </summary>
        public Quaternion RotationNoNotify 
        { 
            set => rotation = value; 
        }

        // <----------------------->
        // Passive Properties
        // <----------------------->

        public bool IsStatic;

        public Vector3 Velocity;

        public Vector3 CenterOfMass;

        [Obsolete("Use Mass instead")]
        public float mass;

        public float Mass
        {
            get => mass;
            set { mass = Mathf.Max(value, PhysicsInfo.MinMass); }
        }

        [Obsolete("Use Drag instead")]
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
            Rotation = newRotation;
        }

        //TODO: Look into have deltaTime be a global variable
        public void AddForce(Vector3 force, ForceMode forceMode, float deltaTime)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    Velocity += force / Mass * deltaTime;
                    break;
                case ForceMode.VelocityChange:
                    Velocity += force;
                    break;
            }
        }

        public void AddToPhysicsEngine()
        {
            if (!Physics.Bodies.Contains(this))
                Physics.Bodies.Add(this);
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
