﻿using System;
using System.Runtime.CompilerServices;

namespace NEWTONS.Core
{
    /// <summary>
    /// A struct to represent rotation in 3D space
    /// </summary>
    [System.Serializable]
    public struct Quaternion : IEquatable<Quaternion>
    {
        public float x, y, z, w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public Quaternion(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = 0f;
        }

        public static Quaternion Identity => new Quaternion(0, 0, 0, 1);

        public static Quaternion operator +(Quaternion q1, Quaternion q2) => new Quaternion(q1.x + q2.x, q1.y + q2.y, q1.z + q2.z, q1.w + q2.w);
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            float x = q1.x * q2.w + q1.y * q2.z - q1.z * q2.y + q1.w * q2.x;
            float y = -q1.x * q2.z + q1.y * q2.w + q1.z * q2.x + q1.w * q2.y;
            float z = q1.x * q2.y - q1.y * q2.x + q1.z * q2.w + q1.w * q2.z;
            float w = -q1.x * q2.x - q1.y * q2.y - q1.z * q2.z + q1.w * q2.w;

            return new Quaternion(x, y, z, w);
        }
        public static Quaternion operator -(Quaternion q1, Quaternion q2) => new Quaternion(q1.x - q2.x, q1.y - q2.y, q1.z - q2.z, q1.w - q2.w);
        public static bool operator ==(Quaternion q1, Quaternion q2) => q1.x == q2.x && q1.y == q2.y && q1.z == q2.z && q1.w == q2.w;
        public static bool operator !=(Quaternion q1, Quaternion q2) => !(q1 == q2);

        /// <summary>
        /// adds two quaternions
        /// </summary>
        /// <param name="q2">Quaternion</param>
        /// <returns>added quaternion</returns>
        public Quaternion Add(Quaternion q2)
        {
            return new Quaternion(this.x + q2.x, this.y + q2.y, this.z + q2.z, this.w + q2.w);

        }
        
        /// <summary>
        /// multiplies two quaternions
        /// </summary>
        /// <param name="q2">Quaternion</param>
        /// <returns>multiplied quaternion</returns>
        public Quaternion Multiply(Quaternion q2) 
        {
            float x2 = this.x * q2.w + this.y * q2.z - this.z * q2.y + this.w * q2.x;
            float y2 = -this.x * q2.z + this.y * q2.w + this.z * q2.x + this.w * q2.y;
            float z2 = this.x * q2.y - this.y * q2.x + this.z * q2.w + this.w * q2.z;
            float w2 = -this.x * q2.x - this.y * q2.y - this.z * q2.z + this.w * q2.w;

            return new Quaternion(x2, y2, z2, w2);
        }
        /// <summary>
        /// zero clue what that does.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static float Magnitude(Quaternion q)
        {
            return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        }
        /// <summary>
        /// normalized a quaternion
        /// </summary>
        /// <param name="q">Quaternion</param>
        /// <returns>a normalized quaternion</returns>
        public static Quaternion Normalize(Quaternion q)
        {
            float mag = Magnitude(q);

            return new Quaternion(1 * q.x / mag, 1 * q.y / mag, 1 *  q.z / mag, q.w / mag);
        }
        
        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, q.w);
        }
        /// <summary>
        /// the inverse of a quaternion
        /// </summary>
        /// <param name="q">Quaternion</param>
        /// <returns>an inversed quaternion</returns>
        public static Quaternion Inverse(Quaternion q)
        {
            float magSquared = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;

            float inverseMag = 1.0f / magSquared;

            float x = -q.x * inverseMag;
            float y = -q.y * inverseMag;
            float z = -q.z * inverseMag;
            float w = q.w * inverseMag;

            return new Quaternion(x, y, z, w);
        }
        /// <summary>
        /// this rotates a vector by a quaternion.
        /// </summary>
        /// <param name="v">Vector3</param>
        /// <param name="q">Quaternion</param>
        /// <returns>a Vector3 rotated by a given quaternion</returns>
        public static Vector3 RotateVector(Vector3 v, Quaternion q)
        {
            Quaternion vecQuat = new Quaternion(v.x, v.y, v.z);
            Quaternion resultQuat = q * vecQuat * Conjugate(q);

            return new Vector3(resultQuat.x, resultQuat.y, resultQuat.z);
        }
        
        /// <summary>
        /// this converts a quaternion to euler angles.
        /// </summary>
        /// <param name="q">Quaternion</param>
        /// <returns>a Vector3 with radiant values</returns>
        public static Vector3 ToEulerAngle(Quaternion q)
        {
            Vector3 angles;
            
            // x-Axis
            float sinr_cosp = 2 * (q.w * q.x + q.y * q.z);
            float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
            angles.x = Mathf.Atan2(sinr_cosp, cosr_cosp);
            
            // y-Axis
            float sinp = Mathf.Sqrt(1 + 2 * (q.w * q.y - q.x * q.z));
            float cosp = Mathf.Sqrt(1 - 2 * (q.w * q.y - q.x * q.z));
            angles.y = 2 * Mathf.Atan2(sinp, cosp) - Mathf.PI / 2;
            
            // z-Axis
            float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
            float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
            angles.z = Mathf.Atan2(siny_cosp, cosy_cosp);
        
            return angles;
        }
        /// <summary>
        /// this converts a euler angle (Vector3) to a quaternion.
        /// </summary>
        /// <param name="v">euler angle</param>
        /// <returns>a quaternion</returns>
        public static Quaternion ToQuaternion(Vector3 v)
        {
            float cr = Mathf.Cos(v.x * 0.5f);
            float sr = Mathf.Sin(v.x * 0.5f);
            float cp = Mathf.Cos(v.y * 0.5f);
            float sp = Mathf.Sin(v.y * 0.5f);
            float cy = Mathf.Cos(v.z * 0.5f);
            float sy = Mathf.Sin(v.z * 0.5f);

            Quaternion q;
            q.w = cr * cp * cy + sr * sp * sy;
            q.x = sr * cp * cy - cr * sp * sy;
            q.y = cr * sp * cy + sr * cp * sy;
            q.z = cr * cp * sy - sr * sp * cy;

            return q;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z}, {w})";
        }

        public override bool Equals(object? obj)
        {
            return obj is Quaternion quaternion && Equals(quaternion);
        }

        public bool Equals(Quaternion other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z, w);
        }
    }
}
