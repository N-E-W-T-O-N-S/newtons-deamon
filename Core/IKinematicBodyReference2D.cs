using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public interface IKinematicBodyReference2D : IDisposable
    {
        public IKinematicBodyReference2D SetKinematicBody(KinematicBody2D kinematicBody);
    }
}
