using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public struct Bounds2D
    {
        public Vector2 Min;
        public Vector2 Max;

        public Bounds2D(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public Bounds2D(float radius, Vector2 position)
        {
            Min = new Vector2(-radius, -radius) + position;
            Max = new Vector2(radius, radius) + position;
        }

        public void IncludePoint(Vector2 point)
        {
            if (point.x < Min.x) Min.x = point.x;
            if (point.x > Max.x) Max.x = point.x;

            if (point.y < Min.y) Min.y = point.y;
            if (point.y > Max.y) Max.y = point.y;
        }
    }
}
