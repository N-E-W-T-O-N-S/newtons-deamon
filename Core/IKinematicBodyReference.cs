using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    public interface IKinematicBodyReference : IDisposable
    {
        public IKinematicBodyReference SetKinematicBody(KinematicBody kinematicBody);

    }
}
