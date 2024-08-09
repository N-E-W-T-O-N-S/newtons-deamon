using NEWTONS.Core._2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core._3D
{
    [System.Serializable]
    public class Rigidbody : IDisposable
    {
        public event Action? OnUpdatePosition;
        public event Action? OnUpdateRotation;

        private List<IRigidbodyReference> _references = new List<IRigidbodyReference>();

        public Collider? Collider { get; set; }

        /// <summary>
        /// Is this RigidBody2D already disposed
        /// </summary>
        public bool Disposed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        } = false;

        /// <summary>
        /// Is this RigidBody2D already added to the engine
        /// </summary>
        public bool AddedToPhysicsEngine
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Physics.Bodies.Contains(this);
        }

        public Rigidbody()
        {
            Mass = 1f;
            Rotation = Quaternion.Identity;
        }

        public Rigidbody(Vector3 position, Quaternion rotation, float mass, Vector3 centerOfMass, float drag, bool addToEngine, Collider? coll)
        {
            Position = position;
            Rotation = rotation;
            Mass = mass;
            CenterOfMass = centerOfMass;
            Drag = drag;
            Collider = coll;
            if (addToEngine)
                AddToPhysicsEngine();
        }

        // <----------------------->
        // Active Properties
        // <----------------------->

        private Vector3 _position;

        public Vector3 Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value == _position) return;
                _position = value;
                Collider?.PositionChanged();
            }
        }

        private Quaternion _rotation;

        /// <summary>
        /// Angle in degrees
        /// </summary>
        public Quaternion Rotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _rotation;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value == _rotation) return;
                _rotation = value;
                Collider?.RotationChanged();
            }
        }

        // <----------------------->
        // Passive Properties
        // <----------------------->

        public bool FixRotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public bool IsStatic
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        private Vector3 _velocity;

        public Vector3 Velocity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsStatic ? Vector3.Zero : _velocity;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _velocity = value;
        }

        private Vector3 _angularVelocity;

        /// <summary>
        /// Angular velocity in radians per second
        /// </summary>
        public Vector3 AngularVelocity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsStatic || FixRotation ? Vector3.Zero : _angularVelocity;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _angularVelocity = value;
        }

        public bool CustomInertia
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        } = false;

        //private float _inertia = 1f;

        //public float Inertia
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    get => CustomInertia ? _inertia : Collider?.Inertia ?? 1f;
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    set
        //    {
        //        if (!CustomInertia)
        //            return;
        //        _inertia = value;
        //    }
        //}

        //public float InvInertia
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    get => IsStatic || FixRotation ? 0f : 1f / Inertia;
        //}

        public Vector3 CenterOfMass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        private float _mass;

        public float Mass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _mass;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _mass = Mathf.Max(value, PhysicsInfo.MinMass);
        }

        public float InvMass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsStatic ? 0f : 1f / Mass;
        }

        private float _drag;

        public float Drag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _drag;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _drag = Mathf.Max(value, PhysicsInfo.MinDragCoefficient);
        }

        public bool UseGravity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public Matrix3x3 WorldToLocalMatrix => new Matrix3x3(
            Quaternion.RotateVector(new Vector3(1, 0, 0), Rotation),
            Quaternion.RotateVector(new Vector3(0, 1, 0), Rotation),
            Quaternion.RotateVector(new Vector3(0, 0, 1), Rotation)
        );

        public void MoveToPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public void MoveToRotation(Quaternion newRotation)
        {
            Rotation = newRotation;
        }

        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    Velocity += force / Mass * Physics.DeltaTime;
                    break;
                case ForceMode.VelocityChange:
                    Velocity += force;
                    break;
            }
        }

        public void AddToPhysicsEngine()
        {
            Physics.Bodies.Add(this);
        }

        public void RemoveFromPhysicsEngine()
        {
            Physics.Bodies.Remove(this);
        }

        public void AddReference(IRigidbodyReference reference)
        {
            _references.Add(reference);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InformPositionChange() => OnUpdatePosition?.Invoke();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InformRotationChange() => OnUpdateRotation?.Invoke();

        /// <summary>
        /// Removes the RgidiBody from the engine and disposes all references
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (Disposed)
                return;

            if (AddedToPhysicsEngine)
                RemoveFromPhysicsEngine();

            OnUpdatePosition = null;
            OnUpdateRotation = null;

            for (int i = 0; i < _references.Count; i++)
            {
                _references[i].Dispose();
            }
            _references.Clear();
            Disposed = true;
        }
    }
}
