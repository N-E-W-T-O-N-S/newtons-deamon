using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NEWTONS.Core
{
    [System.Serializable]
    public struct Quaternion
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



        public Quaternion Add(Quaternion q2)
        {
            return new Quaternion(this.x + q2.x, this.y + q2.y, this.z + q2.z, this.w + q2.w);

        }

        public Quaternion Multiply(Quaternion q2) 
        {
            float x = this.x * q2.w + this.y * q2.z - this.z * q2.y + this.w * q2.x;
            float y = -this.x * q2.z + this.y * q2.w + this.z * q2.x + this.w * q2.y;
            float z = this.x * q2.y - this.y * q2.x + this.z * q2.w + this.w * q2.z;
            float w = -this.x * q2.x - this.y * q2.y - this.z * q2.z + this.w * q2.w;

            return new Quaternion(x, y, z, w);
        }

        public static float Magnitude(Quaternion q)
        {
            return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        }

        public static Quaternion Normalize(Quaternion q)
        {
            float mag = Magnitude(q);

            return new Quaternion(-1 * q.x / mag, -1 * q.y / mag, -1 *  q.z / mag, q.w / mag);
        }

        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, q.w);
        }

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

        public static Vector3 RotateVector(Vector3 v, Quaternion q)
        {
            Quaternion vecQuat = new Quaternion(v.x, v.y, v.z);
            Quaternion resultQuat = q * vecQuat * Conjugate(q);

            return new Vector3(resultQuat.x, resultQuat.y, resultQuat.z);
        }

        public override string ToString()
        {
            return $"{x}, {y}, {z}, {w}";
        }
    }

}
