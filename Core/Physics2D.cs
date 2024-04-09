using NEWTONS.Debuger;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    public sealed class Physics2D
    {
        public static List<Collider2D> Colliders { get; set; } = new List<Collider2D>();
        public static List<Rigidbody2D> Bodies { get; set; } = new List<Rigidbody2D>();

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
        public static int Update(float deltaTime)
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                Rigidbody2D body = Bodies[i];
                if (body.IsStatic)
                    continue;
                
                Vector2 deltaPos = Vector2.Zero;

                if (body.UseGravity)
                    body.Velocity += Gravity * deltaTime;

                if (body.Velocity != Vector2.Zero)
                    deltaPos += body.Velocity * deltaTime;

                if (deltaPos != Vector2.Zero)
                    body.MoveToPosition(body.Position + deltaPos);
            }
            
            int checks = 0;

            // TODO: infinite quadtree
            Rectangle boundary = new Rectangle(new Vector2(0, 0), new Vector2(100, 100));
            _quadtree = new Quadtree<Collider2D>(boundary, 4);

            HashSet<ValueTuple<Collider2D, Collider2D>> checkd = new HashSet<ValueTuple<Collider2D, Collider2D>>(); // THIS!!!!!!
            for (int i = 0; i < Colliders.Count; i++)
            {
                _quadtree.Insert(new QuadtreeData<Collider2D>(Colliders[i].GlobalCenter, Colliders[i]));
            }

            for (int i = 0; i < Colliders.Count; i++)
            {
                // TODO: Add boundary to get Scale for the Quadtree
                var qtDataToCheck = _quadtree.Receive(Colliders[i].GlobalCenter, Colliders[i].Scale);

                Collider2D c1 = Colliders[i];
                foreach (var qtData in qtDataToCheck)
                {
                    Collider2D c2 = qtData.Data;
                    ValueTuple<Collider2D, Collider2D> compareTupl = (c1, c2);
                    if (c1 == c2 || checkd.Contains(compareTupl))
                        continue;

                    checks++;
                    CollisionInfo info = c1.IsColliding(c2);

                    compareTupl = (c2, c1);
                    checkd.Add(compareTupl);
                }
            }
            return checks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveBody(Rigidbody2D body)
        {
            Bodies.Remove(body);
            body.Dispose();
        }
    }
}
