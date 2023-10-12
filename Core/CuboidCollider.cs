using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public class CuboidCollider
    {
        public Vector3 Center { get; set; } = new Vector3(0, 0, 0);
        public Vector3 Size { get; set; } = new Vector3(1, 1, 1);
    }
}
