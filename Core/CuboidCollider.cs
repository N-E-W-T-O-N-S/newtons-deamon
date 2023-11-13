using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        /// <summary>
        ///  <b><u>WARNING:</u> only used for Serialization</b>
        /// </summary>
        public CuboidCollider()
        {
            Points = defaultPoints;
            Scale = new Vector3(1, 1, 1);
            GlobalScales = new Vector3(1, 1, 1);
        }

        public CuboidCollider(Vector3 scale, KinematicBody kinematicBody, Vector3 center) : base(scale, defaultPoints, kinematicBody, center, PrimitiveShape.Cube)
        {

        }

        public bool IsColliding(KonvexCollider other)
        {
            bool colliding = false;
            float lengthX = Mathf.Abs((other.Center.x + other.Body.Position.x) - (Center.x + Body.Position.x));
            float lengthY = Mathf.Abs((other.Center.y + other.Body.Position.y) - (Center.y + Body.Position.y));
            float lengthZ = Mathf.Abs((other.Center.z + other.Body.Position.z) - (Center.z + Body.Position.z));

            //TODO: scale != width, height and length for all konvex shapes
            float half_w_k1 = GlobalScales.x / 2;
            float half_w_k2 = other.GlobalScales.x / 2;
            float gapX = lengthX - (half_w_k1 + half_w_k2);

            float half_h_k1 = GlobalScales.y / 2;
            float half_h_k2 = other.GlobalScales.y / 2;
            float gapY = lengthY - (half_h_k1 + half_h_k2);

            float half_d_k1 = GlobalScales.z / 2;
            float half_d_k2 = other.GlobalScales.z / 2;
            float gapZ = lengthZ - (half_d_k1 + half_d_k2);

            if (gapX < 0 && gapY < 0 && gapZ < 0)
            {
                if (Body.Velocity == Vector3.Zero || Body.IsStatic)
                    return true;

                //TODO: Not working properly with different scales

                colliding = true;
                Vector3 dir = -Body.Velocity.Normalized;
                Vector3 bodyDir = other.Body.Position - Body.Position;

                float absX = Mathf.Abs(gapX);
                float absY = Mathf.Abs(gapY);
                float absZ = Mathf.Abs(gapZ);
                float xDistSquare = bodyDir.x * bodyDir.x;
                float yDistSquare = bodyDir.y * bodyDir.y;
                float zDistSquare = bodyDir.z * bodyDir.z;

                float gap = 0f;
                if (((xDistSquare > yDistSquare && xDistSquare > zDistSquare) || xDistSquare == yDistSquare || (dir.y == 0 && dir.z == 0)) && dir.x != 0)
                {
                    gap = absX / Mathf.Abs(dir.x);
                }
                else if (((yDistSquare > xDistSquare && yDistSquare > zDistSquare) || yDistSquare == zDistSquare || (dir.x == 0 && dir.z == 0)) && dir.y != 0)
                {
                    gap = absY / Mathf.Abs(dir.y);
                }
                else if (dir.z != 0)
                {
                    gap = absZ / Mathf.Abs(dir.z);
                }

                Vector3 backDist = dir * gap;
                //Body.Velocity = Vector3.Zero;
                Body.MoveToPosition(Body.Position + backDist);
            }
            return colliding;
        }

        public void AddToPhysicsEngine()
        {
            if (!Physics.Collideres.Contains(this))
                Physics.Collideres.Add(this);
        }
    }
}
