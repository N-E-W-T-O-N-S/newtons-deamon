using Microsoft.Win32.SafeHandles;
using NEWTONS.Debugger;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core._2D
{
    [System.Serializable]
    public static class Physics2D
    {
        public static HashSet<Collider2D> Colliders { get; set; } = new HashSet<Collider2D>();
        public static HashSet<Rigidbody2D> Bodies { get; set; } = new HashSet<Rigidbody2D>();

        public static float DeltaTime { get; set; }

        public static Broadphase broadphaseAlgorithm = Broadphase.Quadtree;

        //private static Quadtree<Collider2D>? _quadtree;
        public static readonly BVH2D<Collider2D> _bvh = new BVH2D<Collider2D>();

        /// <summary>
        /// Acceleration applied to the Physics World
        /// <br /> Default (0, -9.81f)
        /// </summary>
        public static Vector2 Gravity { get; set; } = new Vector2(0, -9.81f);

        public static bool UseCustomDrag { get; set; } = false;

        public static int Steps { get; set; }

        public const float airDensity = 1.2041f;

        private static float _density = airDensity;

        public static float Density
        {
            get => UseCustomDrag ? _density : airDensity;
            set
            {
                if (UseCustomDrag)
                    _density = Mathf.Max(value, PhysicsInfo.MinDensity);
            }
        }

        private static float _temperature;

        public static float Temperature
        {
            get => _temperature;
            set => _temperature = Mathf.Max(value, PhysicsInfo.MinTemperature);
        }

        public static List<AirComposition> AirCompositions { get; set; } = new List<AirComposition>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(float delta)
        {
            DeltaTime = delta;
            float time = delta / Steps;

            BVHData2D<Collider2D>[] data = new BVHData2D<Collider2D>[Colliders.Count];
            for (int step = 0; step < Steps; step++)
            {

                HashSet<ValueTuple<Collider2D, Collider2D>> checkd = new HashSet<ValueTuple<Collider2D, Collider2D>>(); // THIS!!!!!!

                foreach (var body in Bodies)
                {
                    if (!body.IsStatic)
                    {
                        // Linear velocity

                        if (body.UseGravity)
                            body.Velocity += Gravity * time;

                        body.Velocity += (body.Drag * time) * -body.Velocity.Normalized;

                        Vector2 deltaPos = Vector2.Zero;
                        if (body.Velocity != Vector2.Zero)
                        {
                            deltaPos += body.Velocity * time;
                            body.Position += deltaPos;
                        }

                        // Angular Velocity
                        float deltaRotation = 0f; // Angle in degrees
                        if (body.AngularVelocity != 0f && !body.FixRotation)
                        {
                            deltaRotation += body.AngularVelocity * time * Mathf.Rad2Deg;
                            body.Rotation += deltaRotation;
                        }
                    }

                    Collider2D? c1 = body.Collider;
                    if (c1 == null)
                        continue;

                    int i = 0;
                    foreach (var collider in Colliders)
                    {
                        data[i] = new BVHData2D<Collider2D>(collider.GlobalCenter, collider.Bounds, collider);
                        i++;
                    }

                    _bvh.Build(data);

                    List<BVHData2D<Collider2D>> bvhDataToCheck = new List<BVHData2D<Collider2D>>();
                    _bvh.Receive(c1.Bounds, bvhDataToCheck);

                    foreach (var bvhData in bvhDataToCheck)
                    {
                        Collider2D c2 = bvhData.data;
                        ValueTuple<Collider2D, Collider2D> compareTuple = (c1, c2);
                        if (c1 == c2 || checkd.Contains(compareTuple))
                            continue;

                        //TODO: optimize
                        var info = c1.IsColliding(c2);
                        Collider2D.CollisionResponse(c1, c2, info);

                        checkd.Add((c2, c1));
                    }
                }
            }

            // INFO: Inform the frontend about active changes
            foreach (var body in Bodies)
            {
                if (body.IsStatic) continue;
                body.InformPositionChange();

                if (body.FixRotation) continue;
                body.InformRotationChange();
            }
        }

        // JUST FOR TESTING
        //private static void BVH()
        //{
        //    BVHData2D<Collider2D>[] data = new BVHData2D<Collider2D>[Colliders.Count];

        //    int i = 0;
        //    foreach (var collider in Colliders)
        //    {
        //        data[i] = new BVHData2D<Collider2D>(collider.GlobalCenter, collider.Bounds, collider);
        //        i++;
        //    }

        //    _bvh.Build(data);

        //    int checking = 0;
        //    HashSet<ValueTuple<Collider2D, Collider2D>> checkd = new HashSet<ValueTuple<Collider2D, Collider2D>>(); // THIS!!!!!!
        //    foreach (var c1 in Colliders)
        //    {

        //        List<BVHData2D<Collider2D>> bvhDataToCheck = new List<BVHData2D<Collider2D>>();
        //        _bvh.Receive(c1.Bounds, bvhDataToCheck);

        //        checking += bvhDataToCheck.Count;

        //        foreach (var bvhData in bvhDataToCheck)
        //        {
        //            Collider2D c2 = bvhData.data;
        //            ValueTuple<Collider2D, Collider2D> compareTuple = (c1, c2);
        //            if (c1 == c2 || checkd.Contains(compareTuple))
        //                continue;

        //            //TODO: optimize
        //            var info = c1.IsColliding(c2);
        //            Collider2D.CollisionResponse(c1, c2, info);

        //            checkd.Add((c2, c1));
        //        }
        //    }
        //    //Debug.Log(checking);
        //}

        // JUST FOR TESTING
        //private static void Quadtree()
        //{
        //    // TODO: infinite quadtree
        //    Bounds2D boundary = Bounds2D.InvertedBounds;

        //    foreach (var collider in Colliders)
        //    {
        //        boundary.IncludePoint(collider.Bounds.Min);
        //        boundary.IncludePoint(collider.Bounds.Max);
        //    }

        //    _quadtree = new Quadtree<Collider2D>(boundary.ToRectangle(), 4);

        //    foreach (var collider in Colliders)
        //    {
        //        _quadtree.Insert(new QuadtreeData<Collider2D>(collider.Bounds.ToRectangle(), collider));
        //    }

        //    int checking = 0;
        //    HashSet<ValueTuple<Collider2D, Collider2D>> checkd = new HashSet<ValueTuple<Collider2D, Collider2D>>(); // THIS!!!!!!
        //    foreach (var c1 in Colliders)
        //    {

        //        List<QuadtreeData<Collider2D>> qtDataToCheck = new List<QuadtreeData<Collider2D>>();
        //        _quadtree.Receive(c1.Bounds.ToRectangle(), qtDataToCheck);

        //        checking += qtDataToCheck.Count;

        //        foreach (var qtData in qtDataToCheck)
        //        {
        //            Collider2D c2 = qtData.Data;
        //            ValueTuple<Collider2D, Collider2D> compareTupl = (c1, c2);
        //            if (c1 == c2 || checkd.Contains(compareTupl))
        //                continue;

        //            //TODO: optimize
        //            var info = c1.IsColliding(c2);
        //            Collider2D.CollisionResponse(c1, c2, info);

        //            checkd.Add((c2, c1));
        //        }
        //    }
        //    //Debug.Log(checking);
        //}

        public enum Broadphase
        {
            Quadtree = 0,
            BVH = 1
        }

        [System.Serializable]
        public struct AirComposition
        {
            public Bounds2D Bounds;

            private float _density;
            public float Density 
            { 
                get => _density; 
                set => _density = Mathf.Max(value, PhysicsInfo.MinDensity);
            }
        }

    }
}
