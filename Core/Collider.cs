using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class Collider
    {
        public Collider(KinematicBody? kinematicBody, Vector3 center, PrimitiveShape shape)
        {
            KinematicBody = kinematicBody;
            Center = center;
            Shape = shape;
        }

        public KinematicBody? KinematicBody { get; set; }
        public Vector3 Center { get; set; }
        public PrimitiveShape Shape { get; }
    }
}
