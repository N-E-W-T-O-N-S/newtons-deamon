using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class KonvexCollider : Collider
    {
        public KonvexCollider(Vector3 scale, KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape, Vector3[] points) : base(kinematicBody, center, shape)
        {
            Scale = scale;
            Points = points;
        }

        public Vector3 Scale { get; set; }
        public Vector3[] Points { get; set; }
    }
}
