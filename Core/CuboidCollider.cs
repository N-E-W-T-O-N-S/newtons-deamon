using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class CuboidCollider
    {
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public CuboidCollider(float length, float width, float height)
        {
            Length = length;
            Width = width;
            Height = height;
        }
    }
}
