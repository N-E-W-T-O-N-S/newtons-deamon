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
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// Is this RigidBody2D already added to the engine
        /// </summary>
        public bool AddedToPhysicsEngine => Physics2D.Bodies.Contains(this);


        public Collider2D? _collider;

        public Collider2D? Collider { get => _collider; set => _collider = value; }

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

        [Obsolete("Use Position instead")]
        public Vector2 position;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public bool fixRotation;

        public bool FixRotation
        {
            get => fixRotation;
            set
            {
                fixRotation = value;
                AngularVelocity = fixRotation ? 0f : AngularVelocity;
            }
        }

        [Obsolete("Use Rotation instead")]
        public float rotation;

        /// <summary>
        /// Angle in degrees
        /// </summary>
        public float Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        //<----------------------->
        // Passive Properties
        //<----------------------->

        public bool isStatic;

        public bool IsStatic
        {
            get => isStatic;
            set
            {
                isStatic = value;
                InvMass = isStatic ? 0f : 1f / Mass;
                Velocity = isStatic ? Vector2.Zero : Velocity;
                AngularVelocity = isStatic ? 0f : AngularVelocity;
            }
        }

        public Vector2 velocity;

        public Vector2 Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        public float angularVelocity;

        /// <summary>
        /// Angular velocity in radians per second
        /// </summary>
        public float AngularVelocity
        {
            get => angularVelocity;
            set => angularVelocity = value;
        }

        public float Inertia => Collider?.Inertia ?? 1f;

        public float InvInertia => IsStatic | FixRotation ? 0f : 1f / Inertia;

        public Vector2 CenterOfMass;

        [Obsolete("Use Mass instead")]
        public float mass;

        public float Mass
        {
            get => mass;
            set
            {
                mass = Mathf.Max(value, PhysicsInfo.MinMass);
                InvMass = IsStatic ? 0f : 1f / mass;
            }
        }

        public float InvMass;



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
        public Vector2 GetVelocityAtPoint(Vector2 p)
        {
            Vector2 cmToP = p - (CenterOfMass + Position);
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

        internal void InformPositionChange() => OnUpdatePosition?.Invoke();
        internal void InformRotationChange() => OnUpdateRotation?.Invoke();

        ~Rigidbody2D()
        {
            //Debug.Log($"RigidBody2D - hash: {GetHashCode()} - has been disposed!");
            Dispose();
        }

        /// <summary>
        /// Removes the RgidiBody from the engine and disposes all references
        /// </summary>
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
