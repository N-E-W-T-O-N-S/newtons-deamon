﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NEWTONS.Core
{
    public sealed class Physics
    {
        public static List<Rigidbody> Bodies { get; set; } = new List<Rigidbody>();
        public static List<Collider> Colliders { get; set; } = new List<Collider>();

        /// <summary>
        /// Acceleration applied to the Physics World
        /// <br /> Default (0, -9.81f, 0)
        /// </summary>
        public static Vector3 Gravity { get; set; } = new Vector3(0, -9.81f, 0);
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
        public static void Update(float deltaTime)
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                Rigidbody body = Bodies[i];
                Vector3 deltaPos = Vector3.Zero;
                if (body.IsStatic)
                    continue;

                if (body.UseGravity)
                    body.Velocity += Gravity * deltaTime;
                //if (!UsePhysicalDrag)
                //    body.Velocity += -body.Velocity.Normalized * (body.Drag / body.Mass * deltaTime);
                if (body.Velocity != Vector3.Zero)
                    deltaPos += body.Velocity * deltaTime;

                if (deltaPos != Vector3.Zero)
                    body.MoveToPosition(body.Position + deltaPos);
            }

            for (int i = 0; i < Colliders.Count; i++)
            {
                for (int j = 0; j < Colliders.Count; j++)
                {
                    Collider c1 = Colliders[i];
                    Collider c2 = Colliders[j];
                    if (c1 == c2 || (c1.Body.IsStatic && c2.Body.IsStatic))
                        continue;
                    c1.IsColliding(c2);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveBody(Rigidbody body)
        {
            Bodies.Remove(body);
            body.Dispose();
        }
    }
}