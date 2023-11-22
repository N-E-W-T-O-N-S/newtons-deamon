using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public interface IColliderReference : IDisposable
    {
        public IColliderReference SetCollider(Collider collider);
    }
}
