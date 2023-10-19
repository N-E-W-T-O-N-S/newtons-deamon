using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NEWTONS.Core
{
    public class Physics
    {
        public static List<KinematicBody> Bodies { get; set; } = new List<KinematicBody>();



        /// <summary>
        /// Acceleration applied to the Physics World
        /// <br /> Default (0, -9.81f, 0)
        /// </summary>
        public static Vector3 Gravity { get; set; } = new Vector3(0, -9.81f, 0);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(float deltaTime)
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                KinematicBody body = Bodies[i];
                Vector3 deltaPos = Vector3.Zero;

                if (body.UseGravity)
                    body.Velocity += Gravity * deltaTime;
                if (body.Velocity != Vector3.Zero)
                    deltaPos += body.Velocity * deltaTime;

                if (deltaPos != Vector3.Zero)
                    body.MoveToPosition(body.Position + deltaPos);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveBody(KinematicBody body)
        {
            Bodies.Remove(body);
        }
    }
}