using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalLaunchVehicles
{
    internal class VehicleFamily
    {
        internal string Name { get; private set; }

        internal List<Vehicle> AllVehicles { get; private set; }

        internal VehicleFamily(string _name)
        {
            Name = _name;
            AllVehicles = new List<Vehicle>();
        }

        internal void AddVehicle(Vehicle v)
        {
            if(AllVehicles.Where(x => x.Name == v.Name).Count() < 1)
            {
                AllVehicles.Add(v);
                v.SetParent(this);
            }
            else
            {
                //Name already exists
            }
        }

        internal void RemoveVehicle(Vehicle v)
        {
            if(AllVehicles.Contains(v))
            {
                AllVehicles.Remove(v);
            }
        }

        internal Vehicle GetVehicle(string vehicleName)
        {
            return AllVehicles.FirstOrDefault(x => x.Name == vehicleName || x.GetFullName() == vehicleName);
        }

        internal void SetName(string _name)
        {
            Name = _name;
        }
    }
}
