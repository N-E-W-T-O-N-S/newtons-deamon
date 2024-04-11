using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    public struct Matrix3x3 : IEquatable<Matrix3x3>, IFormattable
    {
        public float m00, m01, m02;
        public float m10, m11, m12;
        public float m20, m21, m22;

        public Matrix3x3(Vector3 col1, Vector3 col2, Vector3 col3)
        {
            m00 = col1.x;
            m01 = col2.x;
            m02 = col3.x;
            m10 = col1.y;
            m11 = col2.y;
            m12 = col3.y;
            m20 = col1.z;
            m21 = col2.z;
            m22 = col3.z;
        }

        public static readonly Matrix3x3 identity = new Matrix3x3()
        {
            m00 = 1,
            m01 = 0,
            m02 = 0,
            m10 = 0,
            m11 = 1,
            m12 = 0,
            m20 = 0,
            m21 = 0,
            m22 = 1,
        };

        private float this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return i switch
                {
                    0 => m00,
                    1 => m10,
                    2 => m20,
                    3 => m01,
                    4 => m11,
                    5 => m21,
                    6 => m02,
                    7 => m12,
                    8 => m22,
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
                        m20 = value;
                        break;
                    case 3:
                        m01 = value;
                        break;
                    case 4:
                        m11 = value;
                        break;
                    case 5:
                        m21 = value;
                        break;
                    case 6:
                        m02 = value;
                        break;
                    case 7:
                        m12 = value;
                        break;
                    case 8:
                        m22 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public float this[int row, int column]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this[row + column * 3];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this[row + column * 3] = value;
            }
        }

        /// <summary>
        /// no side effects
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector3 GetCol(int i) => i switch
        {
            0 => new Vector3(m00, m10, m20),
            1 => new Vector3(m01, m11, m21),
            2 => new Vector3(m02, m12, m22),
            _ => throw new IndexOutOfRangeException()
        };

        /// <summary>
        /// no side effects
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCol(int i, Vector3 col)
        {
            this[0, i] = col.x;
            this[1, i] = col.y;
            this[2, i] = col.z;
        }

        /// <summary>
        /// no side effects
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector3 GetRow(int i) => i switch
        {
            0 => new Vector3(m00, m01, m02),
            1 => new Vector3(m10, m11, m12),
            2 => new Vector3(m20, m21, m22),
            _ => throw new IndexOutOfRangeException()
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
            this[i, 2] = row.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Matrix3x3 a, Matrix3x3 b) => (a.GetCol(0) == b.GetCol(0) && a.GetCol(1) == b.GetCol(1) && a.GetCol(2) == b.GetCol(2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Matrix3x3 a, Matrix3x3 b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator -(Matrix3x3 a, Matrix3x3 b) => new Matrix3x3(a.GetCol(0) - b.GetCol(0), a.GetCol(1) - b.GetCol(1), a.GetCol(2) - b.GetCol(2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator +(Matrix3x3 a, Matrix3x3 b) => new Matrix3x3(a.GetCol(0) + b.GetCol(0), a.GetCol(1) + b.GetCol(1), a.GetCol(2) + b.GetCol(2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator *(Matrix3x3 a, float b) => new Matrix3x3(a.GetCol(0) * b, a.GetCol(1) * b, a.GetCol(2) * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator *(float b, Matrix3x3 a) => new Matrix3x3(a.GetCol(0) * b, a.GetCol(1) * b, a.GetCol(2) * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Matrix3x3 a, Vector3 b) => new Vector3(Vector3.Dot(a.GetRow(0), b), Vector3.Dot(a.GetRow(1), b), Vector3.Dot(a.GetRow(2), b));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b) => new Matrix3x3()
        {
            m00 = Vector3.Dot(a.GetRow(0), b.GetCol(0)),
            m01 = Vector3.Dot(a.GetRow(0), b.GetCol(1)),
            m02 = Vector3.Dot(a.GetRow(0), b.GetCol(2)),

            m10 = Vector3.Dot(a.GetRow(1), b.GetCol(0)),
            m11 = Vector3.Dot(a.GetRow(1), b.GetCol(1)),
            m12 = Vector3.Dot(a.GetRow(1), b.GetCol(2)),

            m20 = Vector3.Dot(a.GetRow(2), b.GetCol(0)),
            m21 = Vector3.Dot(a.GetRow(2), b.GetCol(1)),
            m22 = Vector3.Dot(a.GetRow(2), b.GetCol(2)),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Matrix3x3 other) => (other.m00 == m00 && other.m01 == m01 && other.m02 == m02 &&
                                                        other.m10 == m10 && other.m11 == m11 && other.m12 == m12 &&
                                                        other.m20 == m20 && other.m21 == m21 && other.m22 == m22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
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

            return string.Format("({0}, {1}, {2} \n {3}, {4}, {5} \n {6}, {7}, {8})",
                m00.ToString(format!, formatProvider!), m01.ToString(format!, formatProvider!), m02.ToString(format!, formatProvider!),
                m10.ToString(format!, formatProvider!), m11.ToString(format!, formatProvider!), m12.ToString(format!, formatProvider!),
                m20.ToString(format!, formatProvider!), m21.ToString(format!, formatProvider!), m22.ToString(format!, formatProvider!)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(m00);
            hash.Add(m01);
            hash.Add(m02);
            hash.Add(m10);
            hash.Add(m11);
            hash.Add(m12);
            hash.Add(m20);
            hash.Add(m21);
            hash.Add(m22);
            return hash.ToHashCode();
        }

    }
}
