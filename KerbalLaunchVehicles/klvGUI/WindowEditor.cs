using KSP.UI.Screens;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace KerbalLaunchVehicles.klvGUI
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    internal class WindowEditor : WindowDefault
    {
        public override void Awake()
        {
            base.Awake();

            visibleInScenes = ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB;
            WindowRect = new Rect(GlobalSettings.EditorWindowPos, new Vector2(500, 400));
            KLVCore.EditWindow = this;
        }

        public override void Start()
        {
            base.Start();
            openTab = ViewTab.Vehicles;
            newConfigs = new List<LaunchConfig>();
            textAllPayloads = new List<GUITextBox>();
            comboAllDestinations = new List<DropDown>();
            buttonAllRemoveConfigs = new List<GUIButton>();
            SaveManager.RefreshPaths();
        }

        // Vehicles Tab

        private GUIButton dropZone;
        private GUITextBox textVehicleName;
        private DropDown comboFamily;
        private GUIFreeText textVehicleNote;

        private GUIButton buttonSaveVehicle;
        private GUIButton buttonCancelVehicleEdit;
        private GUIButton buttonAddLaunchConfig;

        private object outputVehicleName;
        private object outputVehicleNote;

        private double payloadMass;
        private double vehicleFuelMass;
        private double vehicleDryMass;
        private double vehicleWetMass { get { return vehicleDryMass + vehicleFuelMass; } }

        private Part selectedRootPart = null;
        private bool isAddingConfig;
        private bool launchConfigsGood;
        private string configErrorReason;
        private bool isRootPartDecoupler;
        private Vehicle editingVehicle = null;
        private Vehicle suggestedVehicle = null;
        private const int maxNewConfigs = 5;

        private List<LaunchConfig> newConfigs;
        private List<GUITextBox> textAllPayloads;
        private static List<DropDown> comboAllDestinations;
        private List<GUIButton> buttonAllRemoveConfigs;

        private object outputPayload;
        private double payload;

        // Suggestions Tab

        protected static GUIListNode familyList;
        private GUIButton buttonUpdateSuggestions;
        private GUIListNode listSuggestions;
        private GUIButton buttonGetSuggestion;

        private bool prevIgnoreMass = false;
        private bool ignoreMass = false;

        protected override void CreateControls()
        {
            base.CreateControls();

            //Tabs
            buttonTabSubassemblies = new GUIButton("Get Launch Vehicle", DoChangeTab, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });

            //Edit Vehicle
            textVehicleName = new GUITextBox("Vehicle Name: ", "", "", 150, 250);
            textVehicleNote = new GUIFreeText("Notes...", 450, 60);
            buttonSaveVehicle = new GUIButton("Save Vehicle", DoSaveVehicle, new GUILayoutOption[] { GUILayout.Width(165) });
            buttonCancelVehicleEdit = new GUIButton("Cancel", EndVehicleEdit, new GUILayoutOption[] { GUILayout.Width(85) });
            comboFamily = new DropDown(new Vector2(150, 100), KLVCore.GetAllFamilyNames());
            RegisterCombos(comboFamily);
            familyList = GUIListNode.CreateListNode3Lvl(KLVCore.GetFullVehicleSummary(), null, DoEditVehicle);
            dropZone = new GUIButton("ADD VESSEL AS LAUNCH VEHICLE", DoAddSubassembly, new GUILayoutOption[] { GUILayout.Height(60) });
            buttonAddLaunchConfig = new GUIButton("Add Launch Config", DoAddLaunchConfig, new GUILayoutOption[] { GUILayout.Width(175) });

            //Suggestions
            buttonUpdateSuggestions = new GUIButton("Update Suggestions", DoUpdateLists, new GUILayoutOption[] { GUILayout.Width(180) });
            buttonGetSuggestion = new GUIButton("Load", DoLoadSubassembly, new GUILayoutOption[] { GUILayout.Width(80) });

            DoUpdateLists();
        }

        public override void OnWindow(int windowId)
        {
            if (EditorLogic.fetch.ship != null)
            {
                payloadMass = ignoreMass ? 0 : EditorLogic.fetch.ship.GetTotalMass();
            }
            else
            {
                EndVehicleEdit();
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

            //Change tab styles based on editing
            buttonTabVehicles.DoLayout(openTab == ViewTab.Vehicles ? klvGUIStyles.TabButtonActive : klvGUIStyles.TabButton,
                editingVehicle != null || EditorLogic.RootPart == null ? "Edit Launch Vehicle" : "Add Launch Vehicle", ViewTab.Vehicles);
            buttonTabSubassemblies.DoLayout(editingVehicle != null ? klvGUIStyles.TabButtonDisabled :
                (openTab == ViewTab.Suggestions ? klvGUIStyles.TabButtonActive : klvGUIStyles.TabButton), null, ViewTab.Suggestions);
            GUILayout.EndHorizontal();

            tabDivider.DoLayout();

            switch (openTab)
            {
                case ViewTab.Vehicles: { DisplayVehicles(); break; }
                case ViewTab.Suggestions: { DisplaySuggestions(); break; }
                default: { break; }
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
            GlobalSettings.EditorWindowPos = WindowRect.position;
        }

        private void DisplayVehicles()
        {
            if (selectedRootPart == null)
            {
                GUILayout.Space(10);

                if (EditorLogic.RootPart == null)
                {
                    // If building is empty, allow select vehicle to edit
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Edit existing Launch Vehicle", klvGUIStyles.HeaderLabel);
                    GUILayout.EndHorizontal();
                    LayoutGUIList(familyList, 340, 350);
                }
                else
                {
                    // If building occupied only allow add new vehicle
                    dropZone.DoLayout(klvGUIStyles.HighlightBox);
                    GUILayout.Space(5);

                    isRootPartDecoupler = EditorLogic.RootPart.partInfo == null || EditorLogic.RootPart.partInfo.category == PartCategories.Coupling;
                    GUILayout.Label(isRootPartDecoupler ? "" : "It is recommended to set a decoupler as the root part to detach the payload", klvGUIStyles.WarningLabel);
                    GUILayout.Space(10);

                    GUILayout.Label("To edit an existing vehicle, clear the building", klvGUIStyles.HeaderLabel);
                }
            }
            else
            {
                GUILayout.Label(editingVehicle == null ? "Add New Launch Vehicle" : "Edit Launch Vehicle", klvGUIStyles.HeaderLabel);
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                outputVehicleName = textVehicleName.DoLayout();
                comboFamily.DoLayout(editingVehicle == null ? null : klvGUIStyles.DisabledButton, editingVehicle == null ? true : false);
                GUILayout.EndHorizontal();

                outputVehicleNote = textVehicleNote.DoLayout();

                GUILayout.Label("Vehicle dry mass: " + vehicleDryMass.ToString("0.###") + "t", klvGUIStyles.StandardLabel);
                GUILayout.Label("Vehicle wet mass: " + vehicleWetMass.ToString("0.###") + "t", klvGUIStyles.StandardLabel);


                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                buttonCancelVehicleEdit.DoLayout(klvGUIStyles.StandardButton);

                if (!string.IsNullOrEmpty(outputVehicleName.ToString()))
                {
                    if (string.IsNullOrEmpty(comboFamily.selectedItemName))
                    {
                        buttonSaveVehicle.DoLayout(klvGUIStyles.WarningButton, "Select family", null);
                    }
                    else if (!KLVCore.VehicleAvailable(comboFamily.selectedItemName, outputVehicleName.ToString(), editingVehicle))
                    {
                        buttonSaveVehicle.DoLayout(klvGUIStyles.WarningButton, "Vehicle exists!", null);
                    }
                    else if (isAddingConfig && !launchConfigsGood)
                    {
                        buttonSaveVehicle.DoLayout(klvGUIStyles.WarningButton, configErrorReason, null);
                    }
                    else
                    {
                        buttonSaveVehicle.DoLayout(klvGUIStyles.CorrectButton, "Save Vehicle", outputVehicleName.ToString());
                    }

                    if (maxNewConfigs > newConfigs.Count())
                    {
                        buttonAddLaunchConfig.DoLayout(klvGUIStyles.StandardButton, "Add Launch Config", true);
                    }
                    else
                    {
                        buttonAddLaunchConfig.DoLayout(klvGUIStyles.WarningButton, "Max new Configs", null);
                    }
                }
                GUILayout.EndHorizontal();

                launchConfigsGood = true;
                configErrorReason = "";
                for (int i = 0; i < newConfigs.Count(); i++)
                {
                    launchConfigsGood = LayoutLaunchConfigEdit(i) && launchConfigsGood;
                }
            }
        }

        private bool LayoutLaunchConfigEdit(int index)
        {
            bool correct = true;

            GUILayout.BeginHorizontal();

            outputPayload = textAllPayloads[index].GetText();

            // Display any invalid data warnings
            if (outputPayload == null || !double.TryParse(outputPayload.ToString(), out payload))
            {
                //GUILayout.Label("Invalid payload!", klvGUIStyles.WarningLabel);
                configErrorReason = String.IsNullOrEmpty(configErrorReason) ? "Invalid payload!" : configErrorReason;
                correct = false;
            }
            else if (!comboAllDestinations[index].isItemSelected)
            {
                //GUILayout.Label("Destination missing!", klvGUIStyles.WarningLabel);
                configErrorReason = String.IsNullOrEmpty(configErrorReason) ? "Destination missing!" : configErrorReason;
                correct = false;
            }
            else
            {
                newConfigs[index].SetPayload(payload);
                newConfigs[index].SetTarget(KLVCore.GetDestination(comboAllDestinations[index].selectedItemName));
            }

            textAllPayloads[index].DoLayout(correct ? klvGUIStyles.StandardLabel : klvGUIStyles.WarningLabel);
            comboAllDestinations[index].DoLayout(null);
            buttonAllRemoveConfigs[index].DoLayout(klvGUIStyles.StandardButton, "Remove", true);

            GUILayout.EndHorizontal();

            payload = 0;

            return correct;
        }
        
        private void DisplaySuggestions()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(250), GUILayout.ExpandWidth(false));
            GUILayout.Label("Payload mass: " + payloadMass.ToString("0.###") + "t", klvGUIStyles.StandardLabel);
            GUILayout.Space(5);
            ignoreMass = GUILayout.Toggle(ignoreMass, " Ignore?");
            if(ignoreMass != prevIgnoreMass)
            {
                //Check changed
                DoUpdateLists();
            }
            prevIgnoreMass = ignoreMass;
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            buttonUpdateSuggestions.DoLayout(klvGUIStyles.StandardButton);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            LayoutGUIList(listSuggestions, 250, 310);
            GUILayout.Space(5);

            if (suggestedVehicle != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(suggestedVehicle.GetFullName(), klvGUIStyles.TitleLabel);
                GUILayout.Label("Launch Mass: " + suggestedVehicle.Mass.ToString("0.00") + "t", klvGUIStyles.StandardLabel);
                GUILayout.Space(5);
                GUILayout.Label(suggestedVehicle.Note, klvGUIStyles.PanelLabel, GUILayout.Width(210), GUILayout.Height(220));
                buttonGetSuggestion.DoLayout(klvGUIStyles.StandardButton);
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        protected override void DoChangeTab(GUIButton sender, object value)
        {
            // Don't allow change tab if editing a vehicle
            if (editingVehicle == null)
            {
                EndVehicleEdit();
                DoUpdateLists(sender, value);
                base.DoChangeTab(sender, value);
            }
        }

        private void DoEditVehicle(GUIButton sender, object value)
        {
            if (value != null && EditorLogic.RootPart == null)
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    var vehicle = KLVCore.GetVehicle(value.ToString());

                    if (vehicle != null)
                    {
                        StageManager.Instance.DeleteEmptyStages();

                        Debug.Log(vehicle.SubassemblyName);
                        DoLoadSubassembly(sender, vehicle.SubassemblyName);
                        DoAddSubassembly(sender, vehicle.SubassemblyName);

                        textVehicleName.SetText(vehicle.Name);
                        textVehicleName.SetEditing(true);
                        outputVehicleName = vehicle.Name;

                        textVehicleNote.SetText(String.IsNullOrEmpty(vehicle.Note) ? "Note..." : vehicle.Note);
                        textVehicleNote.SetEditing(true);
                        outputVehicleNote = vehicle.Note;

                        comboFamily.SetSelection(vehicle.Parent.Name);
                        comboFamily.SetExpanded(false);

                        for (int i = 0; i < vehicle.AllLaunchConfigs.Count; i++)
                        {
                            AddConfig(vehicle.AllLaunchConfigs[i]);
                            comboAllDestinations[i].SetExpanded(false);
                        }

                        editingVehicle = vehicle;
                    }
                }
            }
        }

        protected void DoSaveVehicle(GUIButton sender, object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(comboFamily.selectedItemName))
                {
                    Vehicle newVehicle = null;

                    if (editingVehicle == null)
                    {
                        newVehicle = KLVCore.AddVehicle(comboFamily.selectedItemName, outputVehicleName.ToString(), vehicleWetMass, outputVehicleNote.ToString());
                    }
                    else
                    {
                        newVehicle = editingVehicle;
                        newVehicle.SetName(outputVehicleName.ToString());
                        newVehicle.SetNote(outputVehicleNote.ToString());
                    }

                    newVehicle.SetLaunchConfigs(newConfigs);

                    KLVCore.Save();

                    if (SubassemblyDropZone.Instance != null && SubassemblyDropZone.Instance.enabled)
                    {
                        try
                        {
                            EditorPartList.Instance.subassemblyButtonTransform.enabled = true;
                            SaveManager.SaveSubassembly(SubassemblyDropZone.Instance, selectedRootPart, newVehicle.GetFullName(), newVehicle.Note);
                        }
                        catch (NullReferenceException)
                        {
                            GUIUtilities.Log("Saving subassembly button error");
                        }
                    }
                    EndVehicleEdit();
                }
            }
        }

        private void EndVehicleEdit(GUIButton sender = null, object value = null)
        {
            textVehicleName.SetEditing(false);
            textVehicleNote.SetEditing(false);

            textVehicleName.SetText("");
            textVehicleNote.SetText("");
            comboFamily.Deselect();
            isAddingConfig = false;

            newConfigs = new List<LaunchConfig>();
            textAllPayloads = new List<GUITextBox>();
            comboAllDestinations = new List<DropDown>();
            buttonAllRemoveConfigs = new List<GUIButton>();

            selectedRootPart = null;
            editingVehicle = null;

            KLVCore.UpdateAllVehicleNameSchemes();
            DoUpdateLists();
        }

        protected void DoAddLaunchConfig(GUIButton sender = null, object value = null)
        {
            AddConfig(null);
        }

        protected void AddConfig(LaunchConfig config)
        {
            comboFamily.SetExpanded(false);

            if (maxNewConfigs <= newConfigs.Count())
            {
                return;
            }

            isAddingConfig = true;

            // Add config and all editing controls
            var newRemoveButton = new GUIButton("Remove", DoRemoveLaunchConfig, new GUILayoutOption[] { GUILayout.Width(80) });
            var newTextPayload = new GUITextBox("Max payload: ", "", "t   to ", 50, 180);
            var newDestCombo = new klvGUI.DropDown(new Vector2(200, 130), KLVCore.GetAllDestinationName(), newRemoveButton);

            if (config != null)
            {
                newDestCombo.SetSelection(config.Target.Name);
            }

            newDestCombo.SetExpandAction(DoExpandCombo);
            RegisterCombos(newDestCombo);

            if (config != null)
            {
                newTextPayload.SetText(config.PayloadMass.ToString("0.###"));
                newDestCombo.SetSelection(config.Target == null ? "" : config.Target.Name);
                newConfigs.Add(config);
            }
            else
            {
                newConfigs.Add(new LaunchConfig());
            }

            newTextPayload.SetEditing(true);

            buttonAllRemoveConfigs.Add(newRemoveButton);
            comboAllDestinations.Add(newDestCombo);
            textAllPayloads.Add(newTextPayload);
        }

        protected void DoRemoveLaunchConfig(GUIButton sender, object value)
        {
            if (buttonAllRemoveConfigs.Count > 0 && buttonAllRemoveConfigs.Contains(sender))
            {
                var cbo = GetComboFromParent(sender);
                if (cbo != null)
                {
                    cbo.SetExpanded(false);
                }

                int index = buttonAllRemoveConfigs.IndexOf(sender);

                newConfigs.RemoveAt(index);
                buttonAllRemoveConfigs.RemoveAt(index);
                comboAllDestinations.RemoveAt(index);
                textAllPayloads.RemoveAt(index);
            }
        }

        private void DoAddSubassembly(GUIButton sender, object value)
        {
            if (EditorLogic.SelectedPart == null)
            {
                // End any previous edit
                EndVehicleEdit();
                selectedRootPart = EditorLogic.RootPart;
                GetSubassemblyTotalMass(selectedRootPart, out vehicleDryMass, out vehicleFuelMass);
            }
        }

        private void DoExpandCombo(DropDown sender)
        {
            foreach (var cbo in comboAllDestinations)
            {
                if (cbo != sender)
                {
                    cbo.SetExpanded(false);
                }
            }
        }

        private void GetSubassemblyTotalMass(Part rootPart, out double dryMass, out double fuelMass)
        {
            ShipConstruct subassembly = new ShipConstruct();
            AddChildToConstruct(subassembly, rootPart);

            float fltDrymass;
            float fltFuelMass;

            subassembly.GetShipMass(out fltDrymass, out fltFuelMass);

            dryMass = fltDrymass;
            fuelMass = fltFuelMass;
        }

        private void AddChildToConstruct(ShipConstruct construct, Part childPart)
        {
            construct.Add(childPart);
            for (int i = 0; i < childPart.FindChildParts<Part>().Count(); i++)
            {
                AddChildToConstruct(construct, childPart.FindChildParts<Part>()[i]);
            }
        }

        protected void DoUpdateLists(GUIButton sender = null, object value = null)
        {
            listSuggestions = GUIListNode.CreateListNode2Lvl(KLVCore.GetVehicleSuggestions(payloadMass, ignoreMass), null, DoShowSuggestionInfo);
            listSuggestions.SetCollapsedAll(false);
            listSuggestions.SetLevelStyle(2, klvGUIStyles.PanelLabel);
            familyList = GUIListNode.CreateListNode3Lvl(KLVCore.GetFullVehicleSummary(), null, DoEditVehicle);
        }

        protected void DoLoadSubassembly(GUIButton sender, object value)
        {
            if (suggestedVehicle != null)
            {
                ShipTemplate subassembly = null;

                subassembly = SaveManager.LoadSubAssembly(suggestedVehicle.SubassemblyName);

                if (subassembly != null)
                {
                    EditorLogic.fetch.SpawnTemplate(subassembly);
                }
                suggestedVehicle = null;
            }
            else if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                ShipTemplate subassembly = null;
                subassembly = SaveManager.LoadSubAssembly(value.ToString());

                if (subassembly != null)
                {
                    EditorLogic.fetch.SpawnTemplate(subassembly);
                }
            }
        }

        protected void DoShowSuggestionInfo(GUIButton sender, object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                suggestedVehicle = KLVCore.GetVehicle(value.ToString());
            }
            else
            {
                suggestedVehicle = null;
            }
        }
    }
}
