using KerbalLaunchVehicles.klvGUI;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    internal class KLVCore : MonoBehaviour
    {
        private static List<VehicleFamily> _allVehicleFamilies;
        private static List<Destination> _allDestinations;


        internal static WindowSPC SPCWindow;
        internal static WindowEditor EditWindow;

        internal static List<VehicleFamily> AllVehicleFamilies
        {
            get
            {
                if(_allVehicleFamilies == null)
                {
                    _allVehicleFamilies = SaveManager.LoadConfigurations();
                }
                return _allVehicleFamilies;
            }
            private set
            {
                _allVehicleFamilies = value;
            }
        }
        internal static List<Destination> AllDestinations
        {
            get
            {
                if (_allDestinations == null)
                {
                    _allDestinations = SaveManager.LoadDestinationDefinitions();
                }
                return _allDestinations;
            }
            private set
            {
                _allDestinations = value;
            }
        }

        // Class management
        public void Awake()
        {
            //Called 1st
            SaveManager.ValidatePaths();
            SaveManager.LoadSettings();
        }

        public void Start()
        {
            //Called 2nd
            GUIUtilities.Log("[Kerbal Launch Vehicles] Start");
            RefreshAllConfigurations();
            Load();
        }

        internal static void RefreshAllConfigurations()
        {
            AllVehicleFamilies = new List<VehicleFamily>();
            AllDestinations = new List<Destination>();
            UpdateAllVehicleNameSchemes();
        }

        internal static void Save()
        {
            SaveManager.SaveDestinationDefinitions(AllDestinations);
            SaveManager.SaveConfigurations(AllVehicleFamilies);
        }

        internal static void Load()
        {
            // Need to load destinations before vehicles
            AllDestinations = SaveManager.LoadDestinationDefinitions();
            AllVehicleFamilies = SaveManager.LoadConfigurations();
            UpdateAllVehicleNameSchemes();
        }

        // Vehicle management
        internal static Vehicle AddVehicle(string familyName, string newVehicle, double _mass, string _note = "")
        {
            var family = AllVehicleFamilies.FirstOrDefault(x => x.Name.Trim().ToLowerInvariant() == familyName.Trim().ToLowerInvariant());
            Vehicle v;
            if (family != null)
            {
                v = new Vehicle("", newVehicle, _mass, _note);
                family.AddVehicle(v);
                v.SetSubassemblyName(v.GetFullName());
                return v;
            }
            return null;
        }

        internal static void AddVehicleFamily(string familyName)
        {
            if(AllVehicleFamilies.FirstOrDefault(x => x.Name == familyName) == null)
            {
                AllVehicleFamilies.Add(new VehicleFamily(familyName));
            }
        }

        internal static void AddDestination(string newName)
        {
            AllDestinations.Add(new Destination(GetNewDestinationId(), newName));
        }

        internal static void RemoveDestination(int id)
        {
            var dest = AllDestinations.FirstOrDefault(x => x.Id == id);
            if (dest != null)
            {
                AllDestinations.Remove(dest);
            }
        }

        internal static void RemoveDestination(string destName)
        {
            var dest = AllDestinations.FirstOrDefault(x => x.Name == destName);
            if (dest != null)
            {
                AllDestinations.Remove(dest);
            }
        }

        internal static bool DestinationAvailable(string destName)
        {
            return AllDestinations.FirstOrDefault(x => String.Equals(x.Name.Trim(), destName.Trim(), StringComparison.OrdinalIgnoreCase)) == null;
        }

        internal static bool FamilyAvailable(string familyName)
        {
            return AllVehicleFamilies.FirstOrDefault(x => String.Equals(x.Name.Trim(), familyName.Trim(), StringComparison.OrdinalIgnoreCase)) == null;
        }

        internal static bool VehicleAvailable(string familyName, string vehicleName, Vehicle vehicle = null)
        {
            var family = AllVehicleFamilies.FirstOrDefault(x => String.Equals(x.Name.Trim(), familyName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (GetAllVehicles().Contains(vehicle))
            {
                return true;
            }

            if (family != null)
            {
                return family.AllVehicles.FirstOrDefault(x => String.Equals(x.Name.Trim(), vehicleName.Trim(), StringComparison.OrdinalIgnoreCase)) == null;
            }
            return false;
        }

        internal static Destination GetDestination(string destName)
        {
            return AllDestinations.FirstOrDefault(x => String.Equals(x.Name.Trim(), destName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        internal static Destination GetDestination(int destId)
        {
            return AllDestinations.FirstOrDefault(x => x.Id == destId);
        }

        internal static VehicleFamily GetVehicleFamily(string familyName)
        {
            return AllVehicleFamilies.FirstOrDefault(x => String.Equals(x.Name.Trim(), familyName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        internal static List<Vehicle> GetAllVehicles()
        {
            List<Vehicle> allVehicles = new List<Vehicle>();

            for (int i = 0; i < AllVehicleFamilies.Count; i++)
            {
                allVehicles.AddRange(AllVehicleFamilies[i].AllVehicles);
            }

            return allVehicles;
        }

        internal static Vehicle GetVehicle(string vehicleFullName)
        {
            return GetAllVehicles().FirstOrDefault(x => String.Equals(x.GetFullName().Trim(), vehicleFullName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        // Vehicle name schemes

        internal static void UpdateAllVehicleNameSchemes()
        {
            _allVehicleNames = null;
            _allFamilyNames = null;
            _allDestinationNames = null;
            _allVehicleNamesGroupByFamily = null;
            _fullVehicleSummary = null;
        }

        private static List<string> _allVehicleNames;
        private static List<string> _allFamilyNames;
        private static List<string> _allDestinationNames;
        private static Dictionary<string, List<string>> _allVehicleNamesGroupByFamily;
        private static Dictionary<string[], Dictionary<string[], List<string[]>>> _fullVehicleSummary;

        internal static List<string> GetAllVehicleNames()
        {
            if(_allVehicleNames == null)
            {
                _allVehicleNames = GetAllVehicles().Select(x => x.GetFullName()).ToList();
            }
            return _allVehicleNames;
        }
        internal static List<string> GetAllFamilyNames()
        {
            if (_allFamilyNames == null)
            {
                _allFamilyNames = AllVehicleFamilies.Select(x => x.Name).ToList();
            }
            return _allFamilyNames;
        }

        internal static List<Destination> GetAllDestinations(IEnumerable<Destination> excluding)
        {
            return _allDestinations.Except(excluding).ToList();
        }

        internal static List<string> GetAllDestinationName()
        {
            if (_allDestinationNames == null)
            {
                _allDestinationNames = AllDestinations.Select(x => x.Name).ToList();
            }
            return _allDestinationNames;
        }

        internal static Dictionary<string, List<string>> GetVehicleNameGroupByFamily()
        {
            if (_allVehicleNamesGroupByFamily == null)
            {
                _allVehicleNamesGroupByFamily = AllVehicleFamilies.ToDictionary(g => g.Name, g => g.AllVehicles.Select(v => v.GetFullName()).ToList());
            }
            return _allVehicleNamesGroupByFamily;
        }

        internal static Dictionary<string[], Dictionary<string[], List<string[]>>> GetFullVehicleSummary()
        {
            // This is horrendous, sorry to future me
            if(_fullVehicleSummary == null)
            {
                _fullVehicleSummary = new Dictionary<string[], Dictionary<string[], List<string[]>>>();

                foreach(var family in AllVehicleFamilies)
                {
                    _fullVehicleSummary.Add(new string[] { family.Name }, family.AllVehicles.ToDictionary(v => new string[] { family.Name, v.Name },
                        v => v.AllLaunchConfigs.Select(lc => new string[] { lc.PayloadMass.ToString("0.##") + "t", "to", lc.Target.Name }).ToList()));
                }
            }
            return _fullVehicleSummary;
        }

        internal static int GetNewDestinationId(int currentId = -1)
        {
            if (AllDestinations.Count > 0)
            {
                var allIDs = AllDestinations.Select(x => x.Id).ToList();
                return Enumerable.Range(1, allIDs.Max() + 1).Except(allIDs).FirstOrDefault();
            }
            
            return 0;
        }

        internal static Dictionary<string[], List<string[]>> GetVehicleSuggestions(double payloadMass, bool allowNoConfigs)
        {
            var _allVehicleSuggestions = new Dictionary<string[], List<string[]>>();

            foreach (var v in GetAllVehicles())
            {
                if (allowNoConfigs || v.AllLaunchConfigs.FirstOrDefault(x => x.PayloadMass > payloadMass) != null)
                {
                    _allVehicleSuggestions.Add(new string[] { v.GetFullName()}, 
                        v.AllLaunchConfigs.Where(x => x.PayloadMass > payloadMass && x.Target != null)
                        .Select(lc => new string[] { lc.PayloadMass.ToString("0.##") + "t", "to", lc.Target.Name }).ToList());
                }
            }
            
            return _allVehicleSuggestions;
        }
    }
}
