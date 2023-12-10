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

        private static Quadtree<Collider2D> _quadtree;

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
        public static int Update(float deltaTime)
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                KinematicBody2D body = Bodies[i];
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

            // TODO: think more about checking if two colliders have already been checked
            Dictionary<Collider2D, HashSet<Collider2D>> checkedCollisions = new Dictionary<Collider2D, HashSet<Collider2D>>();
            for (int i = 0; i < Colliders.Count; i++)
            {
                checkedCollisions.Add(Colliders[i], new HashSet<Collider2D>());
                _quadtree.Insert(new QuadtreeData<Collider2D>(Colliders[i].GlobalCenter, Colliders[i]));
            }


            for (int i = 0; i < Colliders.Count; i++)
            {
                // TODO: Scale may not be the best way to determine the size of the collider
                var qtDataToCheck = _quadtree.Receive(Colliders[i].GlobalCenter, Colliders[i].Scale);
                foreach (var qtData in qtDataToCheck)
                {
                    KonvexCollider2D c1 = (KonvexCollider2D)Colliders[i];
                    KonvexCollider2D c2 = (KonvexCollider2D)qtData.Data;
                    if (c1 == c2 || checkedCollisions[c1].Contains(c2) || checkedCollisions[c2].Contains(c1))
                        continue;
                    checks++;
                    bool hit = c1.Collision(c2);

                    checkedCollisions[c1].Add(c2);
                    checkedCollisions[c2].Add(c1);
                }
            }
            return checks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveBody(KinematicBody2D body)
        {
            Bodies.Remove(body);
            body.Dispose();
        }
    }
}
