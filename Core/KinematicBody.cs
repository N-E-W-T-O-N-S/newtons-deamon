using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core
{
    public class KinematicBody
    {
        public event Action? OnUpdatePosition;
        public event Action? OnUpdateRotation;

        //Active Properties
        private Vector3 position;

        public Vector3 Position
        {
            get => position;
            set 
            { 
                position = value;
                OnUpdatePosition?.Invoke();
            }
        }
        private Vector3 rotation;


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
        public Vector3 Velocity { get; set; }
        public Vector3 CenterOfMass { get; set; }
        private float mass;

        public float Mass
        {
            get => mass;
            set { mass = Mathf.Max(value, PhysicsInfo.MinMass); }
        }

        private float drag;

        public float Drag
        {
            get => drag;
            set { drag = Mathf.Max(value, PhysicsInfo.MinDrag); }
        }

        public bool UseGravity { get; set; }

        public KinematicBody() 
        { 
            Physics.Bodies.Add(this);
        }

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
    }
}
