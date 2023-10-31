﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class Collider
    {
        public Collider(KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape)
        {
            KinematicBody = kinematicBody;
            Center = center;
            Shape = shape;
            Physics.Collideres.Add(this);
        }

        //TODO: Needs to be notified when the kinematic body is removed from the scene
        public KinematicBody KinematicBody { get; set; }
        public Vector3 Center { get; set; }
        public PrimitiveShape Shape { get; }
    }
}
