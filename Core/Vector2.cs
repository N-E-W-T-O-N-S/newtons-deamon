using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NEWTONS
{
    public struct Vector2 : IEquatable<Vector2>, IFormattable
    {
        public float x;
        public float y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Returns new <c>Vector2(0, 0);</c>
        /// </summary>
        private static readonly Vector2 zeroVector = new Vector2(0, 0);
        /// <summary>
        /// Returns <c>new Vector2(1, 0);</c>
        /// </summary>
        private static readonly Vector2 rightVector = new Vector2(1, 0);
        /// <summary>
        /// Returns <c>new Vector2(-1, 0);</c>
        /// </summary>
        private static readonly Vector2 leftVector = new Vector2(-1, 0);
        /// <summary>
        /// Returns <c>new Vector2(0, 1);</c>
        /// </summary>
        private static readonly Vector2 upVector = new Vector2(0, 1);
        /// <summary>
        /// Returns <c>new Vector2(0, -1);</c>
        /// </summary>
        private static readonly Vector2 downVector = new Vector2(0, -1);

        public const float floatingAcc = 1E-6f;

        public readonly float magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MathF.Sqrt(x * x + y * y);
        }

        public readonly Vector2 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Vector2 result = new Vector2(x, y);
                result.Normalize();
                return result;
            }
        }

        /// <summary>
        /// Returns <c>new Vector2(0, 0);</c>
        /// </summary>
        public static Vector2 Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => zeroVector;
        }

        /// <summary>
        /// Returns <c>new Vector2(1, 0);</c>
        /// </summary>
        public static Vector2 Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => rightVector;
        }

        /// <summary>
        /// Returns <c>new Vector2(-1, 0);</c>
        /// </summary>
        public static Vector2 Left
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => leftVector;
        }

        /// <summary>
        /// Returns <c>new Vector2(0, 1);</c>
        /// </summary>
        public static Vector2 Up
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => upVector;
        }

        /// <summary>
        /// Returns <c>new Vector2(0, -1);</c>
        /// </summary>
        public static Vector2 Down
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => downVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator -(Vector2 a) => new Vector2(0f - a.x, 0f - a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b, a.y * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(float b, Vector2 a) => new Vector2(a.x * b, a.y * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.x / b, a.y / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            float numX = a.x - b.x;
            float numY = a.y - b.y;
            float num = numX * numX + numY * numY;
            return num < 9.99999944E-11f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(Vector2 v) => new Vector3(v.x, v.y, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Magnitude() => MathF.Sqrt(x * x + y * y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(Vector2 a) => MathF.Sqrt(a.x * a.x + a.y * a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float SqrMagnitude() => (x * x + y * y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SqrMagnitude(Vector2 a) => (a.x * a.x + a.y * a.y);

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
        public readonly float Dot(Vector2 a) => x * a.x + y * a.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(Vector2 a)
        {
            x *= a.x;
            y *= a.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Scale(Vector2 a, Vector2 b)
        {
            return new Vector2
            (
                a.x * b.x,
                a.y * b.y
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            //TODO: Mathf impl for Clamp
            if (t < 0)
                t = 0;
            else if (t > 1f)
                t = 1f;
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        //TODO: May need to be optimised
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 SLerp(Vector2 start, Vector2 end, float t)
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

            Vector2 result = start * startWeight + end * endWeight;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector2 a, Vector2 b)
        {
            float numX = a.x - b.x;
            float numY = a.y - b.y;
            return MathF.Sqrt(numX * numX + numY * numY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Reflect(Vector2 direction, Vector2 normal)
        {
            float num = -2f * Dot(normal, direction);
            return new Vector2(num * normal.x + direction.x, num * normal.y + direction.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override bool Equals(object other)
        {
            if (!(other is Vector2 v))
                return false;

            return Equals(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Vector2 other) => (other.x == x && other.y == y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString() => ToString(null, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string format) => ToString(format, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return string.Format("({0}, {1})", x.ToString(format!, formatProvider!), y.ToString(format!, formatProvider!));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override int GetHashCode() => HashCode.Combine(x, y, magnitude, Normalized);
    }
}
