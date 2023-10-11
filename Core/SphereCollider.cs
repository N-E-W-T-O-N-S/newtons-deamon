using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class SphereCollider
    {
        public float Radius { get; set; }
        public Vector3 Offset { get; set; }

        public SphereCollider(float radius, Vector3 offset)
        {
            Radius = radius;
            Offset = offset;
        }
    }
}
