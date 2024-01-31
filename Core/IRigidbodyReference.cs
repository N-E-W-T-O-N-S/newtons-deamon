using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public interface IRigidbodyReference : IDisposable
    {
        public IRigidbodyReference SetRigidbody(Rigidbody rigidbody);

    }
}
