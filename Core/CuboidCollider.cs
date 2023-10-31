using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class CuboidCollider : KonvexCollider
    {
        private static readonly Vector3[] points = new Vector3[8]
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

        public CuboidCollider(Vector3 scale, KinematicBody kinematicBody, Vector3 center) : base(scale, kinematicBody, center, PrimitiveShape.Cube, points)
        {

        }

        public bool IsColliding(KonvexCollider other)
        {
            float length = Mathf.Abs((other.Center.x + other.KinematicBody.Position.x) - (Center.x + KinematicBody.Position.x));
            //TODO: scale != width, height and length for all konvex shapes
            float half_w_k1 = Scale.x / 2;
            float half_w_k2 = other.Scale.x / 2;
            float gap = length - (half_w_k1 + half_w_k2);

            if (gap < 0)
                return true;
            return false;
        }
    }
}
