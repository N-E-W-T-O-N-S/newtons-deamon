using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Core
{
    /// <summary>
    /// Defines the method to apply force to a KinematicBody
    /// </summary>
    public enum ForceMode
    {
        /// <summary>
        /// Applies a continuous force to the KinematicBody, using its mass.
        /// </summary>
        Force = 0,
        /// <summary>
        /// Applies a continuous force to the KinematicBody, ignoreing its mass.
        /// </summary>
        Acceleration = 5,
        /// <summary>
        /// Applies a impulse force to the KinematicBody, using its mass 
        /// </summary>
        Impulse = 1,
        /// <summary>
        /// Applies a velocity directly to the KinematicBody, ignoreing its mass.
        /// </summary>
        VelocityChange = 2
    }
}
