using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KinematicBody2D : IDisposable
    {
        public event Action? OnUpdatePosition;
        public event Action? OnUpdateRotation;

        private List<IKinematicBodyReference2D> _references = new List<IKinematicBodyReference2D>();
        private bool _isDisposed = false;

        /// <summary>
        /// Implements with default values
        /// <br /> <see cref="Mass"/> = 1f
        /// </summary>
        public KinematicBody2D()
        {
            Mass = 1f;
        }

        public KinematicBody2D(Vector2 position, Vector2 rotation, float drag, float mass)
        {
            Position = position;
            Rotation = rotation;
            Mass = mass;
            Drag = drag;
            AddToPhysicsEngine();
        }

        //Active Properties

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
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
            get => position;
            set => position = value;
        }

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector2 rotation;

        public Vector2 Rotation
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
        public Vector2 RotationNoNotify
        {
            get => position;
            set => position = value;
        }

        //<----------------------->
        //Passive Properties
        //<----------------------->

        public bool IsStatic;

        public Vector2 Velocity;

        public Vector2 CenterOfMass;

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

        public void AddReference(IKinematicBodyReference2D reference)
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
