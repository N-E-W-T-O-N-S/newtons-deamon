using System;
using System.Collections.Generic;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider : Collider
    {
        public KonvexCollider()
        {

        }

        public KonvexCollider(Vector3[] points, int[] indices, Vector3[] normals, Vector3 scale, KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape, float restitution) : base(scale, kinematicBody, center, shape, restitution)
        {
            PointsRaw = points;
            Indices = indices;
            NormalsRaw = normals;
        }

        public Vector3[] NormalsRaw;

        /// <summary>
        /// the normals of each face rotated by the collider
        /// </summary>
        public Vector3[] Normals
        {
            get
            {
                Vector3[] normals = new Vector3[NormalsRaw.Length];
                for (int i = 0; i < NormalsRaw.Length; i++)
                {
                    normals[i] = Quaternion.RotateVector(NormalsRaw[i], Body.Rotation);
                }
                return normals;
            }
        }

        public int[] Indices;

        public Vector3[] PointsRaw;

        /// <summary>
        /// scaled and rotated points of the collider
        /// </summary>
        public Vector3[] Points
        {
            get
            {
                Vector3[] points = new Vector3[PointsRaw.Length];
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    points[i] = Quaternion.RotateVector(Vector3.Scale(PointsRaw[i], GlobalScales), Body.Rotation);
                }
                return points;
            }
        }

        public virtual CollisionInfo IsColliding(KonvexCollider other)
        {
            List<Vector3> edges1 = new List<Vector3>();
            List<Vector3> edges2 = new List<Vector3>();

            Vector3[] points1 = Points;
            Vector3[] points2 = other.Points;

            Vector3[] normals1 = Normals;
            Vector3[] normals2 = other.Normals;

            List<Vector3> axisToCheck = new List<Vector3>();

            for (int i = 0; i < Indices.Length; i += 2)
            {
                Vector3 v = (points1[Indices[i]] - points1[Indices[i + 1]]).Normalized;
                edges1.Add(v);
            }

            for (int i = 0; i < other.Indices.Length; i += 2)
            {
                Vector3 v = (points2[other.Indices[i]] - points2[other.Indices[i + 1]]).Normalized;
                edges2.Add(v);
            }

            axisToCheck.AddRange(normals1);
            axisToCheck.AddRange(normals2);


            //foreach (var edge1 in edges1)
            //{
            //    foreach (var edge2 in edges2)
            //    {
            //        axisToCheck.Add(Vector3.Cross(edge1, edge2));
            //    }
            //}

            float depth = Mathf.Infinity;
            Vector3 normal = Vector2.Zero;

            for (int i = 0; i < axisToCheck.Count; i++)
            {
                float aMin = Mathf.Infinity;
                float aMax = Mathf.NegativeInfinity;
                float bMin = Mathf.Infinity;
                float bMax = Mathf.NegativeInfinity;

                for (int j = 0; j < points1.Length; j++)
                {
                    float dot = Vector3.Dot(axisToCheck[i], points1[j] + GlobalCenter);
                    if (dot < aMin)
                        aMin = dot;
                    if (dot > aMax)
                        aMax = dot;
                }

                for (int j = 0; j < points2.Length; j++)
                {
                    float dot = Vector3.Dot(axisToCheck[i], points2[j] + other.GlobalCenter);
                    if (dot < bMin)
                        bMin = dot;
                    if (dot > bMax)
                        bMax = dot;
                }

                if (aMin >= bMax || bMin >= aMax)
                {
                    return new CollisionInfo()
                    {
                        didCollide = false
                    };
                }

                float d = Mathf.Min(aMax - bMin, bMax - aMin);
                if (d < depth)
                {
                    depth = d;
                    normal = axisToCheck[i];
                }
            }

            Vector3 dir = other.GlobalCenter - GlobalCenter;

            if (Vector3.Dot(dir, normal) > 0)
                normal = -normal;

            float velocityB1 = Body.IsStatic ? 0 : Body.Velocity.magnitude;
            float velocityB2 = other.Body.IsStatic ? 0 : other.Body.Velocity.magnitude;
            float combinedVelocity = velocityB1 + velocityB2;


            // TEMP SOLUTION BECAUSE THERE ARE NOR ROTATIONAL FORCES
            // <--------------------------------------------------->
            float depth1;
            float depth2;

            if (combinedVelocity > 0)
            {
                depth1 = (velocityB1 / combinedVelocity) * depth;
                depth2 = (velocityB2 / combinedVelocity) * depth;
            }
            else
            {
                float combinedMass = Body.Mass + other.Body.Mass;
                depth1 = Body.IsStatic ? 0 : (Body.Mass / combinedMass) * depth;
                depth2 = other.Body.IsStatic ? 0 : (other.Body.Mass / combinedMass) * depth;
            }
            // <--------------------------------------------------->

            Body.MoveToPosition(Body.Position + (normal * depth1));
            other.Body.MoveToPosition(other.Body.Position + (-normal * depth2));

            CollisionInfo info = new CollisionInfo()
            {
                didCollide = true,
                Normal = normal
            };

            return info;
        }

    }
}