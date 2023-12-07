﻿using System;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider : Collider
    {
        public KonvexCollider()
        {
            
        }

        public KonvexCollider(Vector3[] points, KinematicBody kinematicBody, Vector3 scale, Vector3 center, PrimitiveShape shape) : base(kinematicBody, scale, center, shape)
        {
            Points = points;
        }

        public Vector3[] Points;

        public Vector3[] ScaledPoints
        {
            get 
            {
                Vector3[] scaledPoints = new Vector3[Points.Length];
                for (int i = 0; i < Points.Length; i++)
                {
                    scaledPoints[i] = new Vector3(Points[i].x * GlobalScales.x, Points[i].y * GlobalScales.y, Points[i].z * GlobalScales.z);
                }
                return scaledPoints;
            }
        }

        public bool Collision(KonvexCollider other)
        {

            return false;
        }

    }
}