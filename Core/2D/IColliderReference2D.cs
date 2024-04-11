using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core._2D
{
    public interface IColliderReference2D : IDisposable
    {
        public IColliderReference2D SetCollider(Collider2D collider);
    }
}
