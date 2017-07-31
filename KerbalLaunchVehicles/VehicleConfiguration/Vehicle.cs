using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalLaunchVehicles
{
    internal class Vehicle
    {
        internal string SubassemblyName { get; private set; }
        internal string Name { get; private set; }
        internal string Note { get; private set; }
        internal double Mass { get; private set; }
        internal List<LaunchConfig> AllLaunchConfigs { get; private set; }

        internal VehicleFamily Parent { get; private set; }

        internal Vehicle(string _subName, string _name, double _mass, string _subNote = "")
        {
            SubassemblyName = _subName;
            Name = _name;
            Mass = _mass;
            Note = _subNote;
            AllLaunchConfigs = new List<LaunchConfig>();
        }

        internal void SetParent(VehicleFamily _parent)
        {
            Parent = _parent;
        }

        internal void SetLaunchConfigs(List<LaunchConfig> newConfigs)
        {
            if (newConfigs != null)
            {
                AllLaunchConfigs = new List<LaunchConfig>();
                for (int i = 0; i < newConfigs.Count; i++)
                {                    
                    AddLaunchConfig(newConfigs[i]);
                }
            }
        }

        internal void AddLaunchConfig(LaunchConfig lc)
        {
            if (AllLaunchConfigs.Where(x => x.Target == lc.Target).Count() < 1)
            {
                AllLaunchConfigs.Add(lc);
                lc.SetParent(this);
            }
            else
            {
                //Already exists
            }
        }

        internal void AddLaunchConfig(double _payloadMass, Destination _target)
        {
            AddLaunchConfig(new LaunchConfig(_payloadMass, _target));
        }

        internal void RemoveLaunchConfig(LaunchConfig lc)
        {
            if (AllLaunchConfigs.Contains(lc))
            {
                AllLaunchConfigs.Remove(lc);
            }
        }

        internal string GetFullName()
        {
            return Parent.Name + " " + Name;
        }

        internal bool HasDestination(string destName)
        {
            if (AllLaunchConfigs != null && AllLaunchConfigs.Count() > 0)
            {
                return AllLaunchConfigs.FirstOrDefault(x => x.Target.Name.Trim().ToLowerInvariant() == destName.Trim().ToLowerInvariant()) != null;
            }
            return false;
        }

        internal LaunchConfig GetLaunchConfig(string lcName)
        {
            return AllLaunchConfigs.FirstOrDefault(x => x.Target != null && x.Target.Name.Trim().ToLowerInvariant() == lcName.Trim().ToLowerInvariant());
        }

        internal void SetSubassemblyName(string _subName)
        {
            SubassemblyName = _subName;
        }

        internal void SetName(string _name)
        {
            Name = _name;
        }

        internal void SetNote(string _note)
        {
            Note = _note;
        }
    }
}
