using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class Rigidbody2D : IDisposable
    {
        public event Action? OnUpdatePosition;
        public event Action? OnUpdateRotation;

        private List<IRigidbodyReference2D> _references = new List<IRigidbodyReference2D>();
        private bool _isDisposed = false;


        public Rigidbody2D()
        {
            Mass = 1f;
        }

        public Rigidbody2D(Vector2 position, float rotation, float drag, float mass, bool addToEngine)
        {
            Position = position;
            Rotation = rotation;
            Mass = mass;
            Drag = drag;
            if (addToEngine)
                AddToPhysicsEngine();
        }

        //Active Properties

        [Obsolete("Use Position instead")]
        public Vector2 position;

        public Vector2 Position
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
        public Vector2 PositionNoNotify
        {
            set => position = value;
        }

        [Obsolete("Use Rotation instead")]
        public float rotation;

        public float Rotation
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
        public float RotationNoNotify
        {
            set => rotation = value;
        }

        //<----------------------->
        //Passive Properties
        //<----------------------->

        public bool IsStatic;

        public Vector2 Velocity;

        public Vector2 CenterOfMass;

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

        public void MoveToPosition(Vector2 newPosition)
        {
            Position = newPosition;
        }

        public void MoveToRotation(Quaternion newRotation)
        {
            throw new NotImplementedException();
        }

        //TODO: Look into have deltaTime be a global variable
        public void AddForce(Vector2 force, ForceMode forceMode, float deltaTime)
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
            if (!Physics2D.Bodies.Contains(this))
                Physics2D.Bodies.Add(this);
        }

        public void AddReference(IRigidbodyReference2D reference)
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
