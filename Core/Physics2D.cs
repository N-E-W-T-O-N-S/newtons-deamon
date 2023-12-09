using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    public class Physics2D
    {
        public static List<Collider2D> Colliders { get; set; } = new List<Collider2D>();
        public static List<KinematicBody2D> Bodies { get; set; } = new List<KinematicBody2D>();

        /// <summary>
        /// Acceleration applied to the Physics World
        /// <br /> Default (0, -9.81f, 0)
        /// </summary>
        public static Vector2 Gravity { get; set; } = new Vector2(0, -9.81f);
        public static bool UseCustomDrag { get; set; } = false;

        private static float density;

        public static float Density
        {
            get => density;
            set { density = Mathf.Max(value, PhysicsInfo.MinDensity); }
        }

        private static float temperature;

        public static float Temperature
        {
            get => temperature;
            set { temperature = Mathf.Max(value, PhysicsInfo.MinTemperature); }
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(float deltaTime)
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                KinematicBody2D body = Bodies[i];
                Vector2 deltaPos = Vector2.Zero;
                if (body.IsStatic)
                    continue;

                if (body.UseGravity)
                    body.Velocity += Gravity * deltaTime;
                //if (!UsePhysicalDrag)
                //    body.Velocity += -body.Velocity.Normalized * (body.Drag / body.Mass * deltaTime);
                if (body.Velocity != Vector2.Zero)
                    deltaPos += body.Velocity * deltaTime;

                if (deltaPos != Vector2.Zero)
                    body.MoveToPosition(body.Position + deltaPos);
            }

            for (int i = 0; i < Colliders.Count; i++)
            {
                for (int j = 0; j < Colliders.Count; j++)
                {
                    KonvexCollider2D c1 = (KonvexCollider2D)Colliders[i];
                    KonvexCollider2D c2 = (KonvexCollider2D)Colliders[j];
                    if (c1 == c2)
                        continue;
                    bool hit = c1.Collision(c2);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveBody(KinematicBody2D body)
        {
            Bodies.Remove(body);
            body.Dispose();
        }
    }
}
