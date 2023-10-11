using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NEWTONS.Core
{
    public class KinematicBody
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 CenterOfMass { get; set; }
        public float Mass { get; set; }

        public void MoveToPosition(Vector3 newPosition)
        {
            throw new NotImplementedException();
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
