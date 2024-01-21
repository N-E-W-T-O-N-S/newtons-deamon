using System;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider : Collider
    {
        public KonvexCollider()
        {
            
        }

        public KonvexCollider(Vector3[] points, Vector3 scale, KinematicBody kinematicBody, Vector3 center, Quaternion rotation, PrimitiveShape shape, float restitution) : base(scale, kinematicBody, center, rotation, shape, restitution)
        {
            PointsRaw = points;
        }

        public Vector3[] PointsRaw;

        public Vector3[] Points
        {
            get 
            {
                Vector3[] points = new Vector3[PointsRaw.Length];
                for (int i = 0; i < PointsRaw.Length; i++)
                {
                    points[i] = Quaternion.RotateVector(new Vector3(PointsRaw[i].x * GlobalScales.x, PointsRaw[i].y * GlobalScales.y, PointsRaw[i].z * GlobalScales.z), Rotation);
                }
                return points;
            }
        }

        public bool Collision(KonvexCollider other)
        {

            return false;
        }

    }
}