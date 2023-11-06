using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public class CuboidCollider : KonvexCollider
    {
        public static readonly Vector3[] points = new Vector3[8]
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

        public CuboidCollider()
        {
            Points = points;
            Scale = new Vector3(1, 1, 1);
        }

        public CuboidCollider(Vector3 scale, KinematicBody kinematicBody, Vector3 center) : base(scale, points, kinematicBody, center, PrimitiveShape.Cube)
        {

        }

        public bool IsColliding(KonvexCollider other)
        {
            float lengthX = Mathf.Abs((other.Center.x + other.Body.Position.x) - (Center.x + Body.Position.x));
            float lengthY = Mathf.Abs((other.Center.y + other.Body.Position.y) - (Center.y + Body.Position.y));
            float lengthZ = Mathf.Abs((other.Center.z + other.Body.Position.z) - (Center.z + Body.Position.z));

            //TODO: scale != width, height and length for all konvex shapes
            float half_w_k1 = Scale.x / 2;
            float half_w_k2 = other.Scale.x / 2;
            float gapX = lengthX - (half_w_k1 + half_w_k2);

            float half_h_k1 = Scale.y / 2;
            float half_h_k2 = other.Scale.y / 2;
            float gapY = lengthY - (half_h_k1 + half_h_k2);

            float half_d_k1 = Scale.z / 2;
            float half_d_k2 = other.Scale.z / 2;
            float gapZ = lengthZ - (half_d_k1 + half_d_k2);

            if (gapX < 0 && gapY < 0 && gapZ < 0)
                return true;
            return false;
        }
    }
}
