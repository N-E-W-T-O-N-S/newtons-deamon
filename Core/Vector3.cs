using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NEWTONS
{
    public struct Vector3 : IEquatable<Vector3>, IFormattable
    {
        public float x;
        public float y;
        public float z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Returns new Vector3(0, 0, 0);
        /// </summary>
        private static readonly Vector3 zeroVector = new Vector3(0, 0, 0);
        /// <summary>
        /// Returns <c>new Vector3(1, 0, 0);</c>
        /// </summary>
        private static readonly Vector3 rightVector = new Vector3(1, 0, 0);
        /// <summary>
        /// Returns <c>new Vector3(-1, 0, 0);</c>
        /// </summary>
        private static readonly Vector3 leftVector = new Vector3(-1, 0, 0);
        /// <summary>
        /// Returns <c>new Vector3(0, 1, 0);</c>
        /// </summary>
        private static readonly Vector3 upVector = new Vector3(0, 1, 0);
        /// <summary>
        /// Returns <c>new Vector3(0, -1, 0);</c>
        /// </summary>
        private static readonly Vector3 downVector = new Vector3(0, -1, 0);
        /// <summary>
        /// Returns <c>new Vector3(0, 0, 1);</c>
        /// </summary>
        private static readonly Vector3 forwardVector = new Vector3(0, 0, 1);
        /// <summary>
        /// Returns <c>new Vector3(0, 0, -1);</c>
        /// </summary>
        private static readonly Vector3 backVector = new Vector3(0, 0, -1);

        public const float floatingAcc = 1E-6f;

        public readonly float magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MathF.Sqrt(x * x + y * y + z * z);
        }

        public float sqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => x * x + y * y + z * z;
        }

        public readonly Vector3 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Vector3 result = new Vector3(x, y, z);
                result.Normalize();
                return result;
            }
        }

        /// <summary>
        /// Returns <c>new Vector3(0, 0, 0);</c>
        /// </summary>
        public static Vector3 Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => zeroVector;
        }

        /// <summary>
        /// Returns <c>new Vector3(1, 0, 0);</c>
        /// </summary>
        public static Vector3 Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => rightVector;
        }

        /// <summary>
        /// Returns <c>new Vector3(-1, 0, 0);</c>
        /// </summary>
        public static Vector3 Left
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => leftVector;
        }

        /// <summary>
        /// Returns <c>new Vector3(0, 1, 0);</c>
        /// </summary>
        public static Vector3 Up
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => upVector;
        }

        /// <summary>
        /// Returns <c>new Vector3(0, -1, 0);</c>
        /// </summary>
        public static Vector3 Down
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => downVector;
        }

        /// <summary>
        /// Returns <c>new Vector3(0, 0, 1);</c>
        /// </summary>
        public static Vector3 Forward
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => forwardVector;
        }

        /// <summary>
        /// Returns <c>new Vector3(0, 0, -1);</c>
        /// </summary>
        public static Vector3 Back
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => backVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a) => new Vector3(0f - a.x, 0f - a.y, 0f - a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(float b, Vector3 a) => new Vector3(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, float b) => new Vector3(a.x / b, a.y / b, a.z / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            float numX = a.x - b.x;
            float numY = a.y - b.y;
            float numZ = a.z - b.z;
            float num = numX * numX + numY * numY + numZ * numZ;
            return num < 9.99999944E-11f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(Vector3 v) => new Vector2(v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Magnitude() => MathF.Sqrt(x * x + y * y + z * z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(Vector3 a) => MathF.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float SqrMagnitude() => (x * x + y * y + z * z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(Vector3 a) => (a.x * a.x + a.y * a.y + a.z * a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            float num = magnitude;
            if (num > floatingAcc)
                this /= num;
            else
                this = Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Dot(Vector3 a) => x * a.x + y * a.y + z * a.z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector3 b, Vector3 a) => a.x * b.x + a.y * b.y + a.z * b.z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(Vector3 a)
        {
            x *= a.x; 
            y *= a.y; 
            z *= a.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3
            (
                a.x * b.x,
                a.y * b.y,
                a.z * b.z
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            //TODO: Mathf impl for Clamp
            if (t < 0)
                t = 0;
            else if (t > 1f)
                t = 1f;
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        //TODO: May need to be optimised
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SLerp(Vector3 start, Vector3 end, float t)
        {
            start.Normalize();
            end.Normalize();

            float dot = Dot(start, end);

            //TODO: Clamp in Mathf
            if (dot < -1f)
                dot = -1f;
            else if (dot > 1f)
                dot = 1f;

            float angle = MathF.Acos(dot);

            float sinAngle = MathF.Sin(angle);
            float startWeight = MathF.Sin((1 - t) * angle) / sinAngle;
            float endWeight = MathF.Sin(t * angle) / sinAngle;

            Vector3 result = start * startWeight + end * endWeight;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector3 a, Vector3 b)
        {
            float numX = a.x - b.x;
            float numY = a.y - b.y;
            float numZ = a.z - b.z;
            return MathF.Sqrt(numX * numX + numY * numY + numZ * numZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Reflect(Vector3 direction, Vector3 normal)
        {
            float num = -2f * Dot(normal, direction);
            return new Vector3(num * normal.x + direction.x, num * normal.y + direction.y, num * normal.z + direction.z);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override bool Equals(object other)
        {
            if (!(other is Vector3 v))
                return false;

            return Equals(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Vector3 other) => (other.x == x && other.y == y && other.z == z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly string ToString() => ToString(null, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string format) => ToString(format, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return string.Format("({0}, {1}, {2})", x.ToString(format!, formatProvider!), y.ToString(format!, formatProvider!), z.ToString(format!, formatProvider!));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override int GetHashCode() => HashCode.Combine(x, y, z, magnitude, Normalized);
    }
}
