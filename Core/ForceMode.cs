using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    /// <summary>
    /// Defines the method of applying force to a Rigidbody
    /// </summary>
    public enum ForceMode
    {
        /// <summary>
        /// Applies a continuous force to the Rigidbody, using its mass/inertia.
        /// </summary>
        Force = 0,
        /// <summary>
        /// Applies a velocity directly to the Rigidbody, ignoring its mass/inertia.
        /// </summary>
        VelocityChange = 1
    }
}
