using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core
{
    internal class KinematicBody2D
    {
        public Vector2 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 CenterOfMass { get; set; }
        public float Mass { get; set; }

        public void MoveToPosition(Vector2 newPosition)
        {
            throw new NotImplementedException();
        }
        public void MoveToRotation(Quaternion newRotation)
        {
            throw new NotImplementedException();
        }
        public void AddForce(Vector2 newForce)
        {
            throw new NotImplementedException();
        }
    }
}
