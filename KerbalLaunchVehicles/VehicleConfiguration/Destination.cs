using System;
using System.Collections.Generic;
using System.Text;

namespace KerbalLaunchVehicles
{
    internal class Destination
    {
        internal int Id { get; private set; }
        internal string Name { get; private set; }

        internal Destination(int _id, string _name)
        {
            Id = _id;
            Name = _name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
