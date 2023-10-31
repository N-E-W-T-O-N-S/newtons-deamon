namespace NEWTONS.Core
{
    public class KonvexCollider : Collider
    {
        public KonvexCollider(Vector3 scale, Vector3[] points, KinematicBody kinematicBody, Vector3 center, PrimitiveShape shape) : base(kinematicBody, center, shape)
        {
            Scale = scale;
            Points = points;
        }

        public Vector3 Scale { get; set; }
        Vector3[] Points { get; set; }
    }
}