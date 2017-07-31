using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles
{
    internal static class SaveManager
    {
        private static readonly string modFolder = KSPUtil.ApplicationRootPath + "GameData/KerbalLaunchVehicles/";
        private static string saveFolder;
        private static string configPath;
        internal static string vehiclesPath { get; private set; }
        internal static string settingsPath { get; private set; }

        private static ConfigNode rootNode;
        private static ConfigNode settingsNode;

        //Settings

        public static void RefreshPaths()
        {
            saveFolder = "saves/" + HighLogic.SaveFolder;
            vehiclesPath = saveFolder + "/Subassemblies";
            configPath = vehiclesPath + "/klvConfig.cfg";
            settingsPath = modFolder + "/settings.cfg";
        }

        public static void ValidatePaths()
        {
            RefreshPaths();
            if (!Directory.Exists(vehiclesPath))
            {
                Directory.CreateDirectory(vehiclesPath);
            }

            if (!File.Exists(configPath))
            {
                RefreshPaths();
                File.WriteAllText(configPath, " ");
                SaveDestinationDefinitions(new List<Destination>());
                SaveConfigurations(new List<VehicleFamily>());
            }

            if (!File.Exists(settingsPath))
            {
                RefreshPaths();
                File.WriteAllText(settingsPath, " ");
            }
        }

        // Save file node/key names
        private const string node_families = "LAUNCHFAMILIES";
        private const string node_launchDestinations = "LAUNCHDESTINATIONS";

        private const string node_family = "FAMILY";
        private const string key_familyName = "NAME";

        private const string node_vehicle = "VEHICLE";
        private const string key_subName = "SUBASSEMBLYNAME";
        private const string key_vehicleName = "NAME";
        private const string key_vehicleMass = "MASS";
        private const string key_vehicleNote = "NOTE";

        private const string node_launchConfig = "LAUNCHCONFIG";
        private const string key_payloadMass = "PAYLOADMASS";

        private const string node_destination = "DESTINATION";
        private const string key_destId = "ID";
        private const string key_destDesc = "NAME";

        private const string node_settings = "KLVSETTINGS";
        private const string key_fontSize = "FONTSIZE";
        private const string key_SPCWindowPos = "SPCWINDOW";
        private const string key_EditorWindowPos = "EDITWINDOW";

        private static void LoadRootNode()
        {
            RefreshPaths();
            rootNode = ConfigNode.Load(configPath);
        }

        // Destinations

        internal static List<Destination> LoadDestinationDefinitions()
        {
            ValidatePaths();
            LoadRootNode();
            List<Destination> destinations = new List<Destination>();

            try
            {
                if (rootNode != null && rootNode.HasNode(node_launchDestinations))
                {
                    foreach (var destNode in rootNode.GetNode(node_launchDestinations).GetNodes(node_destination))
                    {
                        destinations.Add(new Destination(int.Parse(destNode.GetValue(key_destId)), destNode.GetValue(key_destDesc))); ;
                    }
                }
            }
            catch(ArgumentNullException)
            {
                Debug.Log("[Kerbal Launch Vehicles] Failed to load Destination configs");
            }

            return destinations;
        }

        internal static void SaveDestinationDefinitions(List<Destination> destinations)
        {
            ValidatePaths();
            ConfigNode destinationsNode = new ConfigNode(node_launchDestinations);

            foreach (var d in destinations)
            {
                ConfigNode destNode = new ConfigNode(node_destination);
                destNode.AddValue(key_destId, d.Id);
                destNode.AddValue(key_destDesc, d.Name);
                destinationsNode.AddNode(destNode);
            }

            if (rootNode == null)
            {
                rootNode = new ConfigNode();
            }

            rootNode.SetNode(node_launchDestinations, destinationsNode, true);

            rootNode.Save(configPath);
        }

        // Vehicle Families

        internal static List<VehicleFamily> LoadConfigurations()
        {
            ValidatePaths();
            LoadRootNode();
            List<VehicleFamily> allFamilies = new List<VehicleFamily>();

            try
            {

                if (rootNode != null && rootNode.HasNode(node_families))
                {
                    foreach (var familyNode in rootNode.GetNode(node_families).GetNodes(node_family))
                    {
                        VehicleFamily g = new VehicleFamily(familyNode.GetValue(key_familyName));

                        foreach (var vehicleNode in familyNode.GetNodes(node_vehicle))
                        {
                            string subId = vehicleNode.GetValue(key_subName);
                            string vName = vehicleNode.GetValue(key_vehicleName);
                            double vMass = double.Parse(vehicleNode.GetValue(key_vehicleMass));
                            string vNote = vehicleNode.GetValue(key_vehicleNote);

                            Vehicle v = new Vehicle(subId, vName, vMass, vNote);

                            foreach (var launchConfigNode in vehicleNode.GetNodes(node_launchConfig))
                            {
                                Destination dest = KLVCore.GetDestination(int.Parse(launchConfigNode.GetValue(node_destination)));

                                if (dest != null)
                                {
                                    double payMass = double.Parse(launchConfigNode.GetValue(key_payloadMass));
                                    v.AddLaunchConfig(payMass, dest);
                                }
                            }
                            g.AddVehicle(v);
                        }
                        allFamilies.Add(g);
                    }
                }
            }
            catch(ArgumentNullException)
            {
                Debug.Log("[Kerbal Launch Vehicles] Failed to load Launch configs");
            }

            return allFamilies;
        }

        internal static void SaveConfigurations(List<VehicleFamily> families)
        {
            ValidatePaths();
            ConfigNode familiesNode = new ConfigNode(node_families);

            foreach (var g in families)
            {
                ConfigNode familyNode = new ConfigNode(node_family);

                familyNode.AddValue(key_familyName, g.Name);

                foreach (var v in g.AllVehicles)
                {
                    ConfigNode vehicleNode = new ConfigNode(node_vehicle);

                    vehicleNode.AddValue(key_subName, v.SubassemblyName);
                    vehicleNode.AddValue(key_vehicleName, v.Name);
                    vehicleNode.AddValue(key_vehicleMass, v.Mass);
                    vehicleNode.AddValue(key_vehicleNote, v.Note);

                    foreach (var lc in v.AllLaunchConfigs)
                    {
                        ConfigNode launchConfigNode = new ConfigNode(node_launchConfig);

                        launchConfigNode.AddValue(node_destination, lc.Target.Id);
                        launchConfigNode.AddValue(key_payloadMass, lc.PayloadMass);

                        vehicleNode.AddNode(launchConfigNode);
                    }
                    familyNode.AddNode(vehicleNode);
                }
                familiesNode.AddNode(familyNode);
            }

            if (rootNode == null)
            {
                rootNode = new ConfigNode();
            }

            rootNode.SetNode(node_families, familiesNode, true);

            rootNode.Save(configPath);
        }

        // Subassemblies


        internal static ShipTemplate LoadSubAssembly(Vehicle vehicle)
        {
            return LoadSubAssembly(vehicle.SubassemblyName);
        }

        internal static ShipTemplate LoadSubAssembly(string subassemblyName)
        {
            if (ShipConstruction.SubassemblyExists(subassemblyName))
            {
                ConfigNode node = ConfigNode.Load(vehiclesPath + "/" + subassemblyName + ".craft");
                ShipTemplate subassembly = new ShipTemplate();
                subassembly.LoadShip(node);
                return subassembly;
            }
            else
            {
                klvGUI.GUIUtilities.Log("Could not find subassembly file: " + subassemblyName);
                return null;
            }
        }

        internal static void SaveSubassembly(SubassemblyDropZone dropzone, Part subAssemblyRootPart, string subassemblyName, string desc)
        {
            if (!string.IsNullOrEmpty(subassemblyName))
            {
                dropzone.SaveSubassembly(subassemblyName, desc, subAssemblyRootPart);
            }
        }

        // Settings

        internal static void LoadSettings()
        {
            ValidatePaths();
            settingsNode = ConfigNode.Load(settingsPath);

            if (settingsNode != null)
            {
                // Just do these individually, not very many

                int fontSizeRelative = 0;
                if (settingsNode.HasValue(key_fontSize))
                {
                    fontSizeRelative = int.Parse(settingsNode.GetValue(key_fontSize));
                }
                klvGUI.klvGUIStyles.SetInitialFontSize(fontSizeRelative);

                if (settingsNode.HasValue(key_SPCWindowPos))
                {
                    GlobalSettings.SPCWindowPos = ConfigNode.ParseVector2(settingsNode.GetValue(key_SPCWindowPos));
                }

                if (settingsNode.HasValue(key_EditorWindowPos))
                {
                    GlobalSettings.EditorWindowPos = ConfigNode.ParseVector2(settingsNode.GetValue(key_EditorWindowPos));
                }
            }
        }

        internal static void SaveSettings()
        {
            ValidatePaths();
            ConfigNode settingsNode = new ConfigNode(node_settings);

            settingsNode.AddValue(key_fontSize, klvGUI.klvGUIStyles.fontSizeRelative, "Font size adjustment between -2 and 4");
            settingsNode.AddValue(key_SPCWindowPos, GlobalSettings.SPCWindowPos);
            settingsNode.AddValue(key_EditorWindowPos, GlobalSettings.EditorWindowPos);

            settingsNode.Save(settingsPath);
        }
    }
}
