using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class CuboidCollider : Collider
    {
        public CuboidCollider(Vector3 scale, KinematicBody? kinematicBody, Vector3 center, PrimitiveShape shape) : base(kinematicBody, center, shape)
        {
            Scale = scale;
        }

        public Vector3 Scale { get; set; }
    }
}
