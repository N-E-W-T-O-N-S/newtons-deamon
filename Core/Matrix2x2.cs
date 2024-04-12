using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    public struct Matrix2x2 : IEquatable<Matrix2x2>, IFormattable
    {
        public float m00, m01;
        public float m10, m11;

        public Matrix2x2(Vector2 col1, Vector2 col2)
        {
            m00 = col1.x;
            m01 = col2.x;
            m10 = col1.y;
            m11 = col2.y;
        }

        private float this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return i switch
                {
                    0 => m00,
                    1 => m10,
                    2 => m01,
                    3 => m11,
                    _ => throw new IndexOutOfRangeException("Invalid matrix index!"),
                };
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (i)
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m01 = value;
                        break;
                    case 3:
                        m11 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        public float this[int row, int column]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this[row + column * 2];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this[row + column * 2] = value;
            }
        }

        /// <summary>
        /// no side effects
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector2 GetCol(int i) => i switch
        {
            0 => new Vector3(m00, m10),
            1 => new Vector3(m01, m11),
            _ => throw new IndexOutOfRangeException("Invalid column index!")
        };

        /// <summary>
        /// no side effects
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCol(int i, Vector2 col)
        {
            this[0, i] = col.x;
            this[1, i] = col.y;
        }

        /// <summary>
        /// no side effects
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector2 GetRow(int i) => i switch
        {
            0 => new Vector2(m00, m01),
            1 => new Vector2(m10, m11),
            _ => throw new IndexOutOfRangeException("Invalid row index!")
        };

        /// <summary>
        /// no side effects
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRow(int i, Vector3 row)
        {
            this[i, 0] = row.x;
            this[i, 1] = row.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 TransformPoint(Vector2 point) => this * point;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Matrix2x2 a, Matrix2x2 b) => (a.GetCol(0) == b.GetCol(0) && a.GetCol(1) == b.GetCol(1) && a.GetCol(2) == b.GetCol(2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Matrix2x2 a, Matrix2x2 b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator -(Matrix2x2 a, Matrix2x2 b) => new Matrix2x2(a.GetCol(0) - b.GetCol(0), a.GetCol(1) - b.GetCol(1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator -(Matrix2x2 a) => new Matrix2x2(-a.GetCol(0), -a.GetCol(1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator +(Matrix2x2 a, Matrix2x2 b) => new Matrix2x2(a.GetCol(0) + b.GetCol(0), a.GetCol(1) + b.GetCol(1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator *(Matrix2x2 a, float b) => new Matrix2x2(a.GetCol(0) * b, a.GetCol(1) * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator *(float b, Matrix2x2 a) => new Matrix2x2(a.GetCol(0) * b, a.GetCol(1) * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(Matrix2x2 a, Vector2 b) => new Vector2(Vector2.Dot(a.GetRow(0), b), Vector2.Dot(a.GetRow(1), b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator *(Matrix2x2 a, Matrix2x2 b) => new Matrix2x2()
        {
            m00 = Vector3.Dot(a.GetRow(0), b.GetCol(0)),
            m01 = Vector3.Dot(a.GetRow(0), b.GetCol(1)),

            m10 = Vector3.Dot(a.GetRow(1), b.GetCol(0)),
            m11 = Vector3.Dot(a.GetRow(1), b.GetCol(1)),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Matrix2x2 other) => (other.m00 == m00 && other.m01 == m01 &&
                                                        other.m10 == m10 && other.m11 == m11);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Matrix2x2 matrix)
                return Equals(matrix);
            return base.Equals(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly string ToString() => ToString(null, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string format) => ToString(format, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F5";
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return string.Format("({0}, {1} \n {2}, {3})",
                m00.ToString(format!, formatProvider!), m01.ToString(format!, formatProvider!),
                m10.ToString(format!, formatProvider!), m11.ToString(format!, formatProvider!)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(m00);
            hash.Add(m01);
            hash.Add(m10);
            hash.Add(m11);
            return hash.ToHashCode();
        }
    }
}
