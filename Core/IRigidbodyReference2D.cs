using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public interface IRigidbodyReference2D : IDisposable
    {
        public IRigidbodyReference2D SetRigidbody(Rigidbody2D rigidbody);
    }
}
