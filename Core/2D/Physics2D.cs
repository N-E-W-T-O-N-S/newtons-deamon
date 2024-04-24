using NEWTONS.Debuger;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core._2D
{
    public static class Physics2D
    {
        public static HashSet<Collider2D> Colliders { get; set; } = new HashSet<Collider2D>();
        public static HashSet<Rigidbody2D> Bodies { get; set; } = new HashSet<Rigidbody2D>();

        public static float DeltaTime { get; set; }

        private static Quadtree<Collider2D>? _quadtree;

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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update()
        {
            foreach (var body in Bodies)
            {
                if (body.IsStatic)
                    continue;
                
                // Linear velocity
                Vector2 deltaPos = Vector2.Zero;

                if (body.UseGravity)
                    body.Velocity += Gravity * DeltaTime;

                if (body.Velocity != Vector2.Zero)
                {
                    deltaPos += body.Velocity * DeltaTime;
                    body.MoveToPosition(body.Position + deltaPos);
                }

                // Angular Velocity
                float deltaRotation = 0f; // Angle in degrees
                if (body.AngularVelocity != 0f)
                {
                    deltaRotation += body.AngularVelocity * DeltaTime * Mathf.Rad2Deg;
                    body.MoveToRotation(body.Rotation + deltaRotation);
                }
            }
            
            // TODO: infinite quadtree
            Rectangle boundary = new Rectangle(new Vector2(0, 0), new Vector2(100, 100));
            _quadtree = new Quadtree<Collider2D>(boundary, 4);

            HashSet<ValueTuple<Collider2D, Collider2D>> checkd = new HashSet<ValueTuple<Collider2D, Collider2D>>(); // THIS!!!!!!
            foreach (var collider in Colliders)
            {
                _quadtree.Insert(new QuadtreeData<Collider2D>(collider.GlobalCenter, collider));
            }

            foreach (var collider in Colliders)
            {
                Collider2D c1 = collider;
                
                // TODO: Add boundary to get Scale for the Quadtree
                var qtDataToCheck = _quadtree.Receive(c1.GlobalCenter, c1.Scale * 100);
                foreach (var qtData in qtDataToCheck)
                {
                    Collider2D c2 = qtData.Data;
                    ValueTuple<Collider2D, Collider2D> compareTupl = (c1, c2);
                    if (c1 == c2 || checkd.Contains(compareTupl))
                        continue;

                    CollisionInfo info = c1.IsColliding(c2);

                    compareTupl = (c2, c1);
                    checkd.Add(compareTupl);
                }
            }
        }
    }
}
