namespace NEWTONS.Core
{
    [System.Serializable]
    public class KonvexCollider : Collider
    {

        public KonvexCollider()
        {
            
        }

        public KonvexCollider(Vector3 scale, Vector3[] points, KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape) : base(kinematicBody, center, shape)
        {
            Scale = scale;
            Points = points;
        }

        /// <summary>
        /// <u><b>WARNING:</b></u> <b>Do NOT use! Only for Serilization</b>
        /// </summary>
        public Vector3 scale;

        public Vector3 Scale 
        { 
            get => scale; 
            set 
            { 
                scale = new Vector3(Mathf.Max(value.x, 0), Mathf.Max(value.y, 0), Mathf.Max(value.z, 0)); 
            } 
        }

        public Vector3[] Points;

        public Vector3[] ScaledPoints
        {
            get 
            {
                Vector3[] scaledPoints = new Vector3[Points.Length];
                for (int i = 0; i < Points.Length; i++)
                {
                    scaledPoints[i] = new Vector3(Points[i].x * Scale.x, Points[i].y * Scale.y, Points[i].z * Scale.z);
                }
                return scaledPoints;
            }
        }


    }
}