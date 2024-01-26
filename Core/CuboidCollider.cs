using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class CuboidCollider : KonvexCollider
    {
        public static readonly Vector3[] defaultPoints = new Vector3[8]
        {
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f)
        };

        public static readonly int[] defaultIndices = new int[24]
        {
            0, 1, 1, 3, 2, 3, 0, 2, 4, 5, 5, 7, 6, 7, 4, 6, 0, 4, 1, 5, 2, 6, 3, 7
        };

        public static readonly Vector3[] defaultNormals = new Vector3[6]
        {
            new Vector3( 1,  0,  0),
            new Vector3(-1,  0,  0),
            new Vector3( 0,  1,  0),
            new Vector3( 0, -1,  0),
            new Vector3( 0,  0,  1),
            new Vector3( 0,  0, -1)
        };

        [Obsolete]
        public CuboidCollider()
        {
            PointsRaw = defaultPoints;
            Indices = defaultIndices;
            NormalsRaw = defaultNormals;
            Scale = new Vector3(1, 1, 1);
            GlobalScales = new Vector3(1, 1, 1);
        }

        public CuboidCollider(Vector3 scale, KinematicBody kinematicBody, Vector3 center, float restitution) : base(defaultPoints, defaultIndices, defaultNormals, scale, kinematicBody, center, PrimitiveShape.Cube, restitution)
        {

        }

        public override CollisionInfo IsColliding(KonvexCollider other)
        {
            CollisionInfo info = base.IsColliding(other);

            CollisionResponse(this, other, info);

            return info;
        }

        public void AddToPhysicsEngine()
        {
            if (!Physics.Colliders.Contains(this))
                Physics.Colliders.Add(this);
        }

        public void CollisionResponse(KonvexCollider body, KonvexCollider other, CollisionInfo info)
        {
            if (!info.didCollide)
                return;

            KinematicBody firstBody = body.Body;
            KinematicBody secondBody = other.Body;

            float m1 = firstBody.Mass;
            float m2 = secondBody.Mass;
            Vector3 v1 = firstBody.Velocity;
            Vector3 v2 = secondBody.Velocity;
            Vector3 pos1 = firstBody.Position;
            Vector3 pos2 = secondBody.Position;
            float e = (body.Restitution + other.Restitution) / 2;
            float meff;

            //Vector3 n = (pos2 - pos1).Normalized;
            Vector3 n = info.Normal;


            if (secondBody.IsStatic)
            {
                meff = m1;
                v2 = Vector3.Zero;
            }
            else if (firstBody.IsStatic)
            {
                meff = m2;
                v1 = Vector3.Zero;
            }
            else
                meff = 1 / ((1 / m1) + (1 / m2));

            if (v1 == Vector3.Zero && v2 == Vector3.Zero)
                return;

            Vector3 vimp = Vector3.ComponentMultiply(n, (v1 - v2));
            Vector3 j = (1 + e) * meff * vimp;
            Vector3 dv1;
            Vector3 dv2;

            if (firstBody.IsStatic)
            {
                dv1 = new Vector3(0, 0, 0);
                dv2 = Vector3.ComponentMultiply(j / m2, n);
            }
            else if (secondBody.IsStatic)
            {
                dv1 = -Vector3.ComponentMultiply(j / m1, n);
                //dv1.x *= -1;
                dv2 = new Vector3(0, 0, 0);
            }
            else
            {
                dv1 = -Vector3.ComponentMultiply(j / m1, n);
                //dv1.x *= -1;
                dv2 = Vector3.ComponentMultiply(j / m2, n);
            }

            firstBody.Velocity += dv1;
            secondBody.Velocity += dv2;
        }
    }
}
