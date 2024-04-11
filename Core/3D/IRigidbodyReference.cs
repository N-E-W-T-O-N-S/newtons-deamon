using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._3D
{
    public interface IRigidbodyReference : IDisposable
    {
        public IRigidbodyReference SetRigidbody(Rigidbody rigidbody);

    }
}
