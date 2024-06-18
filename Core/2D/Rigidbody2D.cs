using NEWTONS.Debuger;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
            get => Physics2D.Bodies.Contains(this);
        }


        public Collider2D? Collider { get; set; }

        /// <summary>
        /// serialization constructor
        /// </summary>
        public Rigidbody2D()
        {
            Mass = 1f;
        }

        public Rigidbody2D(Vector2 position, float rotation, float mass, Vector2 centerOfMass, float drag, bool addToEngine, Collider2D? coll)
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

        //<----------------------->
        // Active Properties
        //<----------------------->

        public Vector2 Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }


        /// <summary>
        /// Angle in degrees
        /// </summary>
        public float Rotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        //<----------------------->
        // Passive Properties
        //<----------------------->

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

        private Vector2 _velocity;

        public Vector2 Velocity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsStatic ? Vector2.Zero : _velocity;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _velocity = value;
        }

        private float _angularVelocity;

        /// <summary>
        /// Angular velocity in radians per second
        /// </summary>
        public float AngularVelocity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsStatic | FixRotation ? 0f : _angularVelocity;
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

        private float _inertia = 1f;

        public float Inertia
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CustomInertia ? _inertia : Collider?.Inertia ?? 1f;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (!CustomInertia)
                    return;
                _inertia = value;
            }
        }

        public float InvInertia
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => IsStatic | FixRotation ? 0f : 1f / Inertia;
        }

        public Vector2 CenterOfMass
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
            set => _drag = Mathf.Max(value, PhysicsInfo.MinDrag);
        }

        public bool UseGravity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        public Matrix2x2 WorldToLocalMatrix => new Matrix2x2(
            Vector2.Rotate(new Vector3(1, 0), Rotation * Mathf.Deg2Rad),
            Vector2.Rotate(new Vector3(0, 1), Rotation * Mathf.Deg2Rad)
        );

        // TODO: find a purpose
        public void AddCurrentVelocity(Vector2 velocity)
        {
            if (IsStatic) return;
        }

        //TODO: find a purpose
        public void AddCurrentAngularVelocity(float angualrVelocity)
        {
            if (IsStatic) return;
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
        /// Gets the current linear velocity, of the point in world space, on the RigidBody
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 GetVelocityAtPoint(Vector2 p)
        {
            Vector2 cmToP = p - (CenterOfMass + Collider?.GlobalCenter ?? Position);
            return Velocity + AngularVelocity * new Vector2(-cmToP.y, cmToP.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddToPhysicsEngine()
        {
            Physics2D.Bodies.Add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFromPhysicsEngine()
        {
            Physics2D.Bodies.Remove(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddReference(IRigidbodyReference2D reference)
        {
            _references.Add(reference);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InformPositionChange() => OnUpdatePosition?.Invoke();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InformRotationChange() => OnUpdateRotation?.Invoke();

        ~Rigidbody2D()
        {
            //Debug.Log($"RigidBody2D - hash: {GetHashCode()} - has been disposed!");
            Dispose();
        }

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

        //INFO: Just for fun!!!!!! (for now)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeMultiple(params Rigidbody2D[] bodies)
        {
            Task[] disposeTasks = new Task[bodies.Length];
            for (int i = 0; i < bodies.Length; i++)
            {
                disposeTasks[i] = Task.Run(() => bodies[i].Dispose());
            }
            Task.WaitAll(disposeTasks);
        }
    }
}
