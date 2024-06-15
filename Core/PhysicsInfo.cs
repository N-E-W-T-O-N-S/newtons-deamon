using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    public static class PhysicsInfo
    {
        public static float MinMass
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1E-20f;
        }

        public static float MinInertia
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1E-20f;
        }

        public static float MinTemperature
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => -273.15f;
        }

        public static float MinDrag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0;
        }

        public static float MinDensity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1E-20f;
        }

        /// <summary>
        /// Max distance between collider to be seen as touching
        /// </summary>
        public static float MaxColliderDistance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 1e-6f;
        }
    }
}
