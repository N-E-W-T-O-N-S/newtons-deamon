using NEWTONS.Core._2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._3D
{
    public struct Bounds
    {
        public Vector3 Min;
        public Vector3 Max;

        public Bounds(Vector3 min, Vector3 max)
        {
            Min = min; 
            Max = max;
        }

        public Bounds(float radius, Vector3 position)
        {
            Min = new Vector3(-radius, -radius, -radius) + position;
            Max = new Vector3(radius, radius, radius) + position;
        }

        public void IncludePoint(Vector3 point)
        {
            if (point.x < Min.x) Min.x = point.x;
            if (point.x > Max.x) Max.x = point.x;

            if (point.y < Min.y) Min.y = point.y;
            if (point.y > Max.y) Max.y = point.y;

            if (point.z < Min.z) Min.z = point.z;
            if (point.z > Max.z) Max.z = point.z;
        }

        public readonly bool InBounds(Vector3 point)
        {
            return point.x > Min.x && point.x < Max.x && point.y > Min.y && point.y < Max.y && point.z > Min.z && point.z < Max.z;
        }

        public readonly bool Intersect(Bounds bounds)
        {
            if (Max.x < bounds.Min.x || Min.x > bounds.Max.x)
                return false;
            if (Max.y < bounds.Min.y || Min.y > bounds.Max.y)
                return false;
            if (Max.z < bounds.Min.z || Min.z > bounds.Max.z)
                return false;

            return true;
        }

        public static bool Intersect(Bounds b1, Bounds b2)
        {
            if (b1.Max.x < b2.Min.x || b1.Min.x > b2.Max.x)
                return false;
            if (b1.Max.y < b2.Min.y || b1.Min.y > b2.Max.y)
                return false;
            if (b1.Max.z < b2.Min.z || b1.Min.z > b2.Max.z)
                return false;

            return true;
        }

        public readonly bool Encapsulates(Bounds bounds)
        {
            if (bounds.Min.x < Min.x || bounds.Max.x > Max.x)
                return false;
            if (bounds.Min.y < Min.y || bounds.Max.y > Max.y)
                return false;
            if (bounds.Min.z < Min.z || bounds.Max.z > Max.z)
                return false;

            return true;
        }

        public static Bounds InvertedBounds => new Bounds(Vector3.Infinity, Vector3.NegativeInfinity);

        public override string ToString()
        {
            return $"Min: {Min} Max: {Max}";
        }
    }
}
