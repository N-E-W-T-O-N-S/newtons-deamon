using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class SphereCollider : Collider
    {
        public SphereCollider(Vector3 scale, float radius, KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape, float restitution) : base(scale, kinematicBody, center, shape, restitution)
        {
            Radius = radius;
        }

        public float Radius { get; set; }
    }
}
