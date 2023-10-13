using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    public struct Mathf
    {
        /// <summary>
        /// The well-known 3.14159265358979... value.
        /// </summary>
        public const float PI = MathF.PI;

        /// <summary>
        /// A representation of postitive infinity.
        /// </summary>
        public const float Infinity = float.PositiveInfinity;

        /// <summary>
        /// A representation of negative infinity.
        /// </summary>
        public const float NegativeInfinity = float.NegativeInfinity;

        /// <summary>
        /// Degrees-to-radians conversion
        /// </summary>
        public const float Deg2Rad = PI / 180f;

        /// <summary>
        /// Radians-to-degrees conversion
        /// </summary>
        public const float Rad2Deg = 57.29578f;

        //Trigonometry


        /// <summary>
        /// Returns the cosine of angle f.
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>(float) The cosine of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float f)
        {
            return (float)Math.Cos(f);
        }
        /// <summary>
        /// Returns the sine of angle f.
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>(float) The sine of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float f)
        {
            return (float)Math.Sin(f);
        }
        /// <summary>
        /// Returns the tangent of angle f.
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>(float) The tangent of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float f)
        {
            return (float)Math.Tan(f);
        }
        /// <summary>
        /// Returns the Arc-sine of angle f.
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>(float) The Arc-sine of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Asin(float f)
        {
            return (float)Math.Asin(f);
        }
        /// <summary>
        /// Returns the Arc-cosine of angle f.
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>(float) The Arc-cosine of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float f)
        {
            return (float)Math.Acos(f);
        }
        /// <summary>
        /// Returns the Arc-tangent of angle f.
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>(float) The Arc-tangent of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan(float f)
        {
            return (float)Math.Atan(f);
        }
        /// <summary>
        /// Returns the angle in radians whose tangent is y and x.
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>(float) The angle of radians of y and x</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        //Numeric Calculation

        /// <summary>
        /// Returns the square root of value f.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns>(float) Returns the square root of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt(f);
        }
        /// <summary>
        /// Returns the absolute value of value f.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns>(float) Returns the absolute value of f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float f)
        {
            return (float)Math.Abs(f);
        }
        /// <summary>
        /// Returns the absolute value of value i.
        /// </summary>
        /// <param name="i">The input value, in int.</param>
        /// <returns>(float) Absolute value of given parameter.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int i)
        {
            return Math.Abs(i);
        }
        /// <summary>
        /// Loops the value t, so that it is never larger than length and never smaller than
        /// 0.
        /// </summary>
        /// <param name="t">The input value, in float.</param>
        /// <param name="length">The input value, in float.</param>
        /// <returns>(float) A float value larger than 0 and smaller than the given length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(float t, float length)
        {
            return Clamp(t - Floor(t / length) * length, 0f, length);
        }

        /// <summary>
        /// Returns the smallest of two values.
        /// </summary>
        /// <param name="a">The input value, in float.</param>
        /// <param name="b">The input value, in float.</param>
        /// <returns>(float) Smallest value in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }
        /// <summary>
        /// Returns the smallest of two or more values.
        /// </summary>
        /// <param name="values">The input values, in float.</param>
        /// <returns>(float) Smallest value in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(params float[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0f;
            }

            float num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }
        /// <summary>
        /// Returns the smallest of two values in int.
        /// </summary>
        /// <param name="a">The input value, in int.</param>
        /// <param name="b">The input value, in int.</param>
        /// <returns>(int) Smallest value in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }
        /// <summary>
        /// Returns the smallest of two or more values in int.
        /// </summary>
        /// <param name="values">The input values, in int.</param>
        /// <returns>(int) Smallest values in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(params int[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0;
            }

            int num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }
        /// <summary>
        /// Returns the biggest of two values.
        /// </summary>
        /// <param name="a">The input value, in float.</param>
        /// <param name="b">The input value, in float.</param>
        /// <returns>(float) Biggest value in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }
        /// <summary>
        /// Returns the biggest of two or more values.
        /// </summary>
        /// <param name="a">The input value, in float.</param>
        /// <param name="b">The input value, in float.</param>
        /// <returns>(float) Biggest value in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(params float[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0f;
            }

            float num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }
        /// <summary>
        /// Returns the biggest of two values.
        /// </summary>
        /// <param name="a">The input value, in int.</param>
        /// <param name="b">The input value, in int.</param>
        /// <returns>(int) Biggest value in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }
        /// <summary>
        /// Returns the biggest of two or more values.
        /// </summary>
        /// <param name="a">The input value, in int.</param>
        /// <param name="b">The input value, in int.</param>
        /// <returns>(int) Biggest value in given parameters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(params int[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0;
            }

            int num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }
        /// <summary>
        /// Returns the value f raised to the power of p.
        /// </summary>
        /// <param name="f">The input value raised to the power, in float.</param>
        /// <param name="p">The input value raising to the power, in float.</param>
        /// <returns>(float) Given value raised to the power of given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float f, float p)
        {
            return (float)Math.Pow(f, p);
        }
        /// <summary>
        /// Returns the constant e raised to the power of p
        /// </summary>
        /// <param name="power">The input raising to the power, in float.</param>
        /// <returns>(float) Constant e raised to the power of given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(float p)
        {
            return (float)Math.Exp(p);
        }
        /// <summary>
        /// Returns the logarithm of a the value f in the base p.
        /// </summary>
        /// <param name="f">The value of the number, in float.</param>
        /// <param name="p">The value of the base, in float.</param>
        /// <returns>(float) Logarithm of given number in base of given base.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Log(float f, float p)
        {
            return (float)Math.Log(f, p);
        }
        /// <summary>
        /// Returns the natural logarithm of the value f.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Log(float f)
        {
            return (float)Math.Log(f);
        }
        /// <summary>
        /// Returns the base 10 logarithm of the value f.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns>(float) Base 10 logarithm of the given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Log10(float f)
        {
            return (float)Math.Log10(f);
        }
        /// <summary>
        /// Returns the smallest integer greater to or equal to value f.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns>(float) Smallest integer greater or equal to given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(float f)
        {
            return (float)Math.Ceiling(f);
        }
        /// <summary>
        /// Returns the largest integer smaller to or equal to value f.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns>(float) Largest integer smaller or equal to given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float f)
        {
            return (float)Math.Floor(f);
        }
        /// <summary>
        /// Returns value f rounded to the nearest integer.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns>(float) Given Value rounded to the nearest integer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float f)
        {
            return (float)Math.Round(f);
        }
        /// <summary>
        /// Returns the sign of value f.
        /// </summary>
        /// <param name="f">The input value, in float.</param>
        /// <returns> Returns -1 when given value is negative.
        /// Returns 1 when given value is positive.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sign(float f)
        {
            return (f >= 0f) ? 1f : (-1f);
        }
        /// <summary>
        /// Clamps the given value f between the given minimum and maximum values 'min' and 'max'.
        /// Returns the given value if it is within the minimum and maximum range.
        /// </summary>
        /// <param name="value">The input value to restrict inside the range, in float.</param>
        /// <param name="min">The minimum value, in float.</param>
        /// <param name="max">The maximum value, in float.</param>
        /// <returns>(float) Returns the given value if it is between the min and max.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float f, float min, float max)
        {
            if (f < min)
            {
                f = min;
            }
            else if (f > max)
            {
                f = max;
            }

            return f;
        }
        /// <summary>
        /// Clamps the given value i between the given minimum and maximum values 'min' and 'max'.
        /// Returns the given value if it is within the minimum and maximum range.
        /// </summary>
        /// <param name="value">The input value to restrict inside the range, in int.</param>
        /// <param name="min">The minimum value, in int.</param>
        /// <param name="max">The maximum value, in int.</param>
        /// <returns>(int) Returns the given value if it is between the min and max.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int i, int min, int max)
        {
            if (i < min)
            {
                i = min;
            }
            else if (i > max)
            {
                i = max;
            }

            return i;
        }
        /// <summary>
        /// Clamps value f between 0 and 1.
        /// </summary>
        /// <param name="value">The input value, in float.</param>
        /// <returns>(float) Returns the given value when it is between 0 and 1.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return 1f;
            }

            return value;
        }
        /// <summary>
        /// Linearly interpolates between value a and value b by value t.
        /// </summary>
        /// <param name="a">The start value, in float.</param>
        /// <param name="b">The end value, in float.</param>
        /// <param name="t">(float) The interpolated float result between the given two float values.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }
        /// <summary>
        /// Linearly interpolates between valuea and value b with no limit to value t.
        /// </summary>
        /// <param name="a">The start value, in float.</param>
        /// <param name="b">The end value, in float.</param>
        /// <param name="t">(float) The float value as a result from the linear interpolation.</param>
        /// <returns>(float) The float value as result from the linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        /// <summary>
        /// Same as Lerp but makes sure the values interpolate correctly when 
        /// they wrap around 360 degrees.
        /// </summary>
        /// <param name="a">The start angle, in float.</param>
        /// <param name="b">The end angle, in float.</param>
        /// <param name="t">
        ///     The interpolation value between the start and end angles.
        ///     This value is clamped to the range [0,1].
        /// </param>
        /// <returns>(float) The interpolated float value between given angle a and angle b, based on the given interpolation value t.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpAngle(float a, float b, float t)
        {
            float num = Repeat(b - a, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }

            return a + num * Clamp01(t);
        }
        /// <summary>
        /// Determines where a value lies between two points.
        /// </summary>
        /// <param name="a">The start of the range, in float.</param>
        /// <param name="b">The end of the range, in float.</param>
        /// <param name="value">The point within the range.</param>
        /// <returns>(float) A value between 0 and 1, representing where the "value"
        /// parameters falls within the range defined by the given values a and b.
        /// If the values a and b are the same, the function returns 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
            {
                return Clamp01((value - a) / (b - a));
            }

            return 0f;
        }

        /// <summary>
        /// Calculates the shortest difference between two given angles
        /// given in degrees.
        /// </summary>
        /// <param name="current">The current angle, in float.</param>
        /// <param name="target">The current angle, in float.</param>
        /// <returns>(float) The shortest difference between two given values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngle(float current, float target)
        {
            float num = Repeat(target - current, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }

            return num;
        }

        /// <summary>
        /// Moves the value current towards the value target.
        /// </summary>
        /// <param name="current">The current value, in float.</param>
        /// <param name="target">The value to move towards, in float.</param>
        /// <param name="maxDelta">The maximum change that should be applied to the value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Sign(target - current) * maxDelta;
        }
        /// <summary>
        /// Same as MoveTowards but makes sure the values interpolate correctly when they
        /// wrap around 360 degrees.
        /// </summary>
        /// <param name="current">The current value, in float.</param>
        /// <param name="target">The value to move towards, in float.</param>
        /// <param name="maxDelta">The maximum change that should be applied to the value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = DeltaAngle(current, target);
            if (0f - maxDelta < num && num < maxDelta)
            {
                return target;
            }

            target = current + num;
            return MoveTowards(current, target, maxDelta);
        }
    }
}
