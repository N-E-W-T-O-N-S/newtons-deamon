using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider2D : Collider2D
    {

        private readonly Vector2[] _defaultPoints = new Vector2[]
        {
            //Points of a hexagon
            new Vector2(0, 0.5f),
            new Vector2(0.433f, 0.25f),
            new Vector2(0.433f, -0.25f),
            new Vector2(0, -0.5f),
            new Vector2(-0.433f, -0.25f),
            new Vector2(-0.433f, 0.25f)
        };

        public KonvexCollider2D()
        {
            Points = _defaultPoints;
            Scale = new Vector2(1, 1);
            GlobalScales = new Vector2(1, 1);
        }

        public KonvexCollider2D(Vector2 scale, Vector2[] points, KinematicBody2D kinematicBody, Vector2 center, PrimitiveShape2D shape) : base(kinematicBody, center, shape)
        {
            Points = points;
        }

        public Vector2[] Points;

        public Vector2[] ScaledPoints
        {
            get
            {
                Vector2[] scaledPoints = new Vector2[Points.Length];
                for (int i = 0; i < Points.Length; i++)
                {
                    scaledPoints[i] = new Vector2(Points[i].x * GlobalScales.x, Points[i].y * GlobalScales.y);
                }
                return scaledPoints;
            }
        }

        public Vector2[] EdgeNormals
        {
            get
            {
                Vector2[] normals = new Vector2[Points.Length];
                for (int i = 0; i < Points.Length; i++)
                {
                    Vector2 edge = Points[(i + 1) % Points.Length] - Points[i];
                    normals[i] = new Vector2(-edge.y, edge.x).Normalized;
                }
                return normals;
            }
        }

        public bool Collision(KonvexCollider2D other)
        {
            Vector2[] aNormals = EdgeNormals;
            Vector2[] bNormals = other.EdgeNormals;
            Vector2[] aScaledPoints = ScaledPoints;
            Vector2[] bScaledPoints = other.ScaledPoints;

            Vector2[] axisToCheck = aNormals.Concat(bNormals).ToArray();
            float aMin = Mathf.Infinity;
            float aMax = Mathf.NegativeInfinity;
            float bMin = Mathf.Infinity;
            float bMax = Mathf.NegativeInfinity;

            //TODO: optimisation
            for (int i = 0; i < axisToCheck.Length; i++)
            {
                for (int j = 0; j < aScaledPoints.Length; j++)
                {
                    float dot = Vector2.Dot(axisToCheck[i], aScaledPoints[j]);
                    if (dot < aMin)
                        aMin = dot;
                    else if (dot > aMax)
                        aMax = dot;
                }

                for (int j = 0; j < bScaledPoints.Length; j++)
                {
                    float dot = Vector2.Dot(axisToCheck[i], bScaledPoints[j]);
                    if (dot < bMin)
                        bMin = dot;
                    else if (dot > bMax)
                        bMax = dot;
                }
                if ((aMin < bMax && aMin > bMin) || (bMin < aMax && bMin > aMin))
                    continue;
                else
                    return false;
            }

            return true;
        }

        public void AddToPhysicsEngine()
        {
            if (!Physics2D.Colliders.Contains(this))
                Physics2D.Colliders.Add(this);
        }

    }
}
