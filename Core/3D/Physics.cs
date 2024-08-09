using NEWTONS.Core._2D;
using NEWTONS.Debugger;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NEWTONS.Core._3D
{
    public static class Physics
    {
        public static HashSet<Rigidbody> Bodies { get; set; } = new HashSet<Rigidbody>();
        public static HashSet<Collider> Colliders { get; set; } = new HashSet<Collider>();

        private static BVH<Collider> _bvh = new BVH<Collider>();

        public static float DeltaTime { get; set; }

        /// <summary>
        /// Acceleration applied to the Physics World
        /// <br /> Default (0, -9.81f, 0)
        /// </summary>
        public static Vector3 Gravity { get; set; } = new Vector3(0, -9.81f, 0);
        public static bool UseCustomDrag { get; set; } = false;

        public static int Steps { get; set; }

        private static float _density;

        public static float Density
        {
            get => _density;
            set { _density = Mathf.Max(value, PhysicsInfo.MinDensity); }
        }

        private static float _temperature;

        public static float Temperature
        {
            get => _temperature;
            set => _temperature = Mathf.Max(value, PhysicsInfo.MinTemperature);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(float delta)
        {
            DeltaTime = delta;
            float time = delta / Steps;

            BVHData<Collider>[] data = new BVHData<Collider>[Colliders.Count];

            for (int step = 0; step < Steps; step++)
            {
                int i = 0;
                foreach (var collider in Colliders)
                {
                    data[i] = new BVHData<Collider>(collider.GlobalCenter, collider.Bounds, collider);
                    i++;
                }

                _bvh.Build(data);

                int checking = 0;
                HashSet<ValueTuple<Collider, Collider>> checkd = new HashSet<ValueTuple<Collider, Collider>>(); // THIS!!!!!!

                foreach (var body in Bodies)
                {
                    if (!body.IsStatic)
                    {
                        if (body.UseGravity)
                            body.Velocity += Gravity * time;

                        // INFO: Fake Drag
                        //body.Velocity -= body.Velocity / body.Mass * time;

                        Vector3 deltaPos = Vector3.Zero;
                        if (body.Velocity != Vector3.Zero)
                        {
                            deltaPos += body.Velocity * time;
                            body.MoveToPosition(body.Position + deltaPos);
                        }
                    }

                    Collider? c1 = body.Collider;
                    if (c1 == null)
                        continue;

                    List<BVHData<Collider>> bvhDataToCheck = new List<BVHData<Collider>>();
                    _bvh.Receive(c1.Bounds, bvhDataToCheck);

                    checking += bvhDataToCheck.Count;

                    foreach (var bvhData in bvhDataToCheck)
                    {
                        Collider c2 = bvhData.data;
                        ValueTuple<Collider, Collider> compareTuple = (c1, c2);
                        if (c1 == c2 || checkd.Contains(compareTuple))
                            continue;

                        //TODO: optimize
                        var info = c1.IsColliding(c2);
                        Collider.CollisionResponse(c1, c2, info);

                        checkd.Add((c2, c1));
                    }
                }
            }

            foreach (var body in Bodies)
            {
                if (body.IsStatic) continue;
                body.InformPositionChange();

                if (body.FixRotation) continue;
                body.InformRotationChange();
            }
        }
    }
}