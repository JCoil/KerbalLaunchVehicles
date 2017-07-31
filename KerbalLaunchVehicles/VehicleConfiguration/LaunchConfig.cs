using System;
using System.Collections.Generic;
using System.Text;

namespace KerbalLaunchVehicles
{
    internal class LaunchConfig
    {
        internal double PayloadMass { get; private set; }
        internal Destination Target { get; private set; }

        private Vehicle Parent;

        internal LaunchConfig()
        {
            PayloadMass = 0;
        }
        internal LaunchConfig(double _payload, Destination _target)
        {
            PayloadMass = _payload;
            Target = _target;
        }

        internal void SetParent(Vehicle _parent)
        {
            Parent = _parent;
        }

        internal void SetPayload(double _payload)
        {
            PayloadMass = _payload;
        }

        internal void SetTarget(Destination _target)
        {
            if(_target !=null)
            {
                Target = _target;
            }
        }
    }
}
