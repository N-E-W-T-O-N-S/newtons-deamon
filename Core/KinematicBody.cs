using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core
{
    public class KinematicBody
    {
        public event Action? UpdatePosition;
        public event Action? UpdateRotation;

        //Active Properties
        private Vector3 position;

        public Vector3 Position
        {
            get => position;
            set 
            { 
                position = value;
                UpdatePosition?.Invoke();
            }
        }
        private Vector3 rotation;

        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                UpdateRotation?.Invoke();
            }
        }

        //Passive Properties
        public Vector3 Velocity { get; set; }
        public Vector3 CenterOfMass { get; set; }
        public float Mass { get; set; }
        public bool UseGravity { get; set; } = true;

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
        public void AddForce(Vector3 newForce)
        {
            throw new NotImplementedException();
        }
    }
}
