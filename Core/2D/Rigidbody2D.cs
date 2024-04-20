using NEWTONS.Debuger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public class Rigidbody2D : IDisposable
    {
        public event Action? OnUpdatePosition;
        public event Action? OnUpdateRotation;

        private List<IRigidbodyReference2D> _references = new List<IRigidbodyReference2D>();

        /// <summary>
        /// Is this RigidBody2D already disposed
        /// </summary>
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// Is this RigidBody2D already added to the engine
        /// </summary>
        public bool AddedToPhysicsEngine => Physics2D.Bodies.Contains(this);

        public Collider2D? Collider { get; set; }

        public Rigidbody2D()
        {
            Mass = 1f;
            Collider = null;
        }

        public Rigidbody2D(Vector2 position, float rotation, float drag, float mass, bool addToEngine, Collider2D? coll)
        {
            Collider = coll;
            Position = position;
            Rotation = rotation;
            Mass = mass;
            Drag = drag;
            if (addToEngine)
                AddToPhysicsEngine();
        }

        //<----------------------->
        // Active Properties
        //<----------------------->

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

        /// <summary>
        /// Angle in degrees
        /// </summary>
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
        // Passive Properties
        //<----------------------->

        public bool IsStatic;

        public Vector2 Velocity;

        /// <summary>
        /// Angular velocity in radians per second
        /// </summary>
        public float AngularVelocity;

        public float Inertia => Collider?.Inertia ?? 1f;

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
            set => drag = Mathf.Max(value, PhysicsInfo.MinDrag);
        }

        public bool UseGravity;

        public Matrix2x2 WorldToLocalMatrix => new Matrix2x2(
            Vector2.Rotate(new Vector3(1, 0), Rotation * Mathf.Deg2Rad),
            Vector2.Rotate(new Vector3(0, 1), Rotation * Mathf.Deg2Rad)
        );

        // TODO: Buffer this
        public void MoveToPosition(Vector2 newPosition)
        {
            Position = newPosition;
        }

        // TODO: Buffer this
        public void MoveToRotation(float newRotation)
        {
            Rotation = newRotation;
        }

        public void AddForce(Vector2 force, ForceMode forceMode)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    Velocity += force / Mass * Physics2D.DeltaTime;
                    break;
                case ForceMode.VelocityChange:
                    Velocity += force;
                    break;
            }
        }

        public void AddTorque(float torque, ForceMode forceMode)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    AngularVelocity += torque / Inertia * Physics2D.DeltaTime;
                    break;
                case ForceMode.VelocityChange:
                    AngularVelocity += torque;
                    break;
            }
        }

        /// <summary>
        /// Gets the current linear velocity of the point on the RigidBody
        /// </summary>
        public Vector2 GetVelocityOfPoint(Vector2 p)
        {
            Vector2 cmToP = p - (CenterOfMass + Position);
            return Velocity + AngularVelocity * new Vector2(-cmToP.y, cmToP.x);
        }

        public void AddToPhysicsEngine()
        {
            if (AddedToPhysicsEngine)
                return;

            Physics2D.Bodies.Add(this);
        }
        
        public void AddReference(IRigidbodyReference2D reference)
        {
            _references.Add(reference);
        }

        /// <summary>
        /// Removes the RgidiBody from the engine and disposes all references
        /// </summary>
        public void Dispose()
        {
            if (Disposed)
                return;

            if (AddedToPhysicsEngine)
                Physics2D.Bodies.Remove(this);

            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _references.Clear();
            Disposed = true;
        }
    }
}
