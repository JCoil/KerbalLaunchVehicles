using KSP.UI.Screens;
using UnityEngine;
using System;
using System.Linq;

namespace KerbalLaunchVehicles.klvGUI
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    internal class WindowSPC : WindowDefault
    {
        public override void Awake()
        {
            base.Awake();
            visibleInScenes = ApplicationLauncher.AppScenes.SPACECENTER;
            WindowRect = new Rect(GlobalSettings.SPCWindowPos, new Vector2(450, 375));
            KLVCore.SPCWindow = this;
        }

        public override void Start()
        {
            base.Start();
            openTab = ViewTab.Families;
            SaveManager.RefreshPaths();
        }

        // Families Tab
        protected GUIListNode familyList;
        protected GUITextBox textEditFamily;
        protected GUIButton buttonSaveFamily;
        protected GUIButton buttonCancelFamilyEdit;

        // Destinations Tab
        private GUITextBox textAddDestination;
        private GUIButton buttonAddDest;
        private GUIButton buttonRemoveDest;

        // Settings Tab
        private GUITextBox textFolderPath;
        private GUIButton buttonSave;
        private GUIButton buttonLoad;
        private GUIButton buttonUnload;
        private GUIButton buttonIncreaseFont;
        private GUIButton buttonDecreaseFont;

        protected override void CreateControls()
        {
            base.CreateControls();

            // Families Tab
            UpdateFamilies();
            SetFamilyEditState(false);
            buttonCancelFamilyEdit = new GUIButton("Cancel", DoCancelFamilyEdit, new GUILayoutOption[] { GUILayout.Width(70) });

            // Destinations Tab
            textAddDestination = new GUITextBox("Add Destination: ", "", "", 180, 300, DoAddDestination);
            buttonAddDest = new GUIButton("Add", DoAddDestination, new GUILayoutOption[] { GUILayout.Width(120) });
            buttonRemoveDest = new GUIButton("Remove", DoRemoveDestination, new GUILayoutOption[] { GUILayout.Width(100) });

            comboDestination = new klvGUI.DropDown(new Vector2(260, 170), KLVCore.GetAllDestinationName());
            RegisterCombos(comboDestination);

            // Settings Tab
            textFolderPath = new klvGUI.GUITextBox("Config path:", SaveManager.vehiclesPath, "", 300, 400);
            buttonSave = new GUIButton("Save Configurations", DoSave, null);
            buttonLoad = new GUIButton("Load Configurations", DoLoad, null);
            buttonUnload = new GUIButton("Unload All Configurations", DoUnload, null);
            buttonIncreaseFont = new GUIButton("▲", DoIncreaseFont, GUILayout.Width(30));
            buttonDecreaseFont = new GUIButton("▼", DoDecreaseFont, GUILayout.Width(30));
        }

        public override void OnWindow(int windowId)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            buttonTabFamilies.DoLayout(openTab == ViewTab.Families ? klvGUIStyles.TabButtonActive : klvGUIStyles.TabButton, null, ViewTab.Families);
            buttonTabDestinations.DoLayout(openTab == ViewTab.Destinations ? klvGUIStyles.TabButtonActive : klvGUIStyles.TabButton, null, ViewTab.Destinations);
            buttonTabSettings.DoLayout(openTab == ViewTab.Settings ? klvGUIStyles.TabButtonActive : klvGUIStyles.TabButton, null, ViewTab.Settings);
            GUILayout.EndHorizontal();

            tabDivider.DoLayout();

            switch (openTab)
            {
                case ViewTab.Families: { DisplayFamilies(); break; }
                case ViewTab.Destinations: { DisplayDestinations(); break; }
                case ViewTab.Settings: { DisplaySettings(); break; }
                default: { break; }
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
            GlobalSettings.SPCWindowPos = WindowRect.position;
        }

        private void UpdateFamilies()
        {
            if (GlobalSettings.AllowFamilyEdit)
            {
                familyList = GUIListNode.CreateListNode3Lvl(KLVCore.GetFullVehicleSummary(), null, null, DoBeginFamilyEdit);
            }
            else
            {
                familyList = GUIListNode.CreateListNode3Lvl(KLVCore.GetFullVehicleSummary());
            }
        }

        //Displays

        private VehicleFamily editingFamily;

        protected void DisplayFamilies()
        {
            LayoutGUIList(familyList, 425, 270);
            var editFamilyCheckState = GUILayout.Toggle(GlobalSettings.AllowFamilyEdit, "  Edit Family Names");

            if(editFamilyCheckState != GlobalSettings.AllowFamilyEdit)
            {
                GlobalSettings.AllowFamilyEdit = editFamilyCheckState;
                UpdateFamilies();
            }

            GUILayout.BeginHorizontal();
            // Decide whether we're adding or editing, whether to display button with what text
            LayoutTextInput(textEditFamily, buttonSaveFamily, KLVCore.FamilyAvailable, "Already Exists!", editingFamily != null,
                (editingFamily != null && GlobalSettings.AllowFamilyEdit) ? klvGUIStyles.CorrectButton : null);

            if (editingFamily != null)
            {
                buttonCancelFamilyEdit.DoLayout();
            }
            GUILayout.EndHorizontal();
        }

        private void DisplayDestinations()
        {
            LayoutTextInput(textAddDestination, buttonAddDest, KLVCore.DestinationAvailable, "Already Exists!");

            GUILayout.Label("Destinations:", klvGUIStyles.StandardLabel);

            GUILayout.BeginHorizontal();

            comboDestination.DoLayout(null);

            if (comboDestination.isItemSelected)
            {
                buttonRemoveDest.DoLayout(klvGUIStyles.StandardButton, "Remove", comboDestination.selectedItemName);
            }

            GUILayout.EndHorizontal();
        }

        private void DisplaySettings()
        {
            textFolderPath.DoLayout(klvGUIStyles.StandardLabel);
            buttonSave.DoLayout(klvGUIStyles.StandardButton);
            buttonLoad.DoLayout(klvGUIStyles.StandardButton);
            buttonUnload.DoLayout(klvGUIStyles.StandardButton);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal(GUILayout.Width(270));
            GUILayout.Label("Adjust font size", klvGUIStyles.StandardLabel);
            buttonIncreaseFont.DoLayout(klvGUIStyles.fontSizeRelative < klvGUIStyles.fontSizeMax ? klvGUIStyles.StandardButton : klvGUIStyles.DisabledButton);
            buttonDecreaseFont.DoLayout(klvGUIStyles.fontSizeRelative > klvGUIStyles.fontSizeMin ? klvGUIStyles.StandardButton : klvGUIStyles.DisabledButton);
            GUILayout.EndHorizontal();

            if (klvGUIStyles.fontSizeRelative != 0)
            {
                GUILayout.Label("Not default size - some elements may not display correctly", klvGUIStyles.WarningLabel);
            }
            else
            {
                GUILayout.Label("Default size", klvGUIStyles.StandardLabel);
            }
        }

        //Button Actions
        protected override void DoChangeTab(GUIButton sender, object value)
        {
            comboDestination.SetExpanded(false);
            base.DoChangeTab(sender, value);
        }

        //Families

        private void SetFamilyEditState(bool isEditing)
        {
            if (isEditing)
            {
                textEditFamily = new GUITextBox("Edit Family:", "", "", 140, 225, DoSaveFamily);
                buttonSaveFamily = new GUIButton("Save", DoSaveFamily, new GUILayoutOption[] { GUILayout.Width(118) });
            }
            else
            {
                textEditFamily = new GUITextBox("Add Family:", "", "", 140, 225, DoSaveFamily);
                buttonSaveFamily = new GUIButton("Add", DoSaveFamily, new GUILayoutOption[] { GUILayout.Width(118) });
            }
        }

        protected virtual void DoBeginFamilyEdit(GUIButton sender, object value)
        {
            if (value != null && GlobalSettings.AllowFamilyEdit)
            {
                var family = KLVCore.GetVehicleFamily(value.ToString().Trim());

                if (family != null)
                {
                    editingFamily = family;
                    SetFamilyEditState(true);
                    textEditFamily.SetText(editingFamily.Name);
                    textEditFamily.SetEditing(true);
                }
            }
        }

        protected virtual void DoSaveFamily(GUIButton sender, object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()) && KLVCore.FamilyAvailable(value.ToString()))
            {
                if (editingFamily == null)
                {
                    //Adding new family
                    KLVCore.AddVehicleFamily(value.ToString());
                }
                else
                {
                    //Editing existing family
                    editingFamily.SetName(value.ToString());
                    editingFamily = null;
                }

                KLVCore.Save();
                KLVCore.UpdateAllVehicleNameSchemes();
                UpdateFamilies();
                textEditFamily.SetText("");
                textEditFamily.SetEditing(false);

                SetFamilyEditState(false);
            }
        }

        protected virtual void DoCancelFamilyEdit(GUIButton sender, object value)
        {
            editingFamily = null;
            SetFamilyEditState(false);
        }

        //Destinations

        private void DoRemoveDestination(GUIButton sender, object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                KLVCore.RemoveDestination(value.ToString());
                KLVCore.Save();
                KLVCore.UpdateAllVehicleNameSchemes();
                comboDestination.SetItems(KLVCore.GetAllDestinationName());
                comboDestination.Deselect();
            }
        }

        private void DoAddDestination(GUIButton sender, object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()) && KLVCore.DestinationAvailable(value.ToString()))
            {
                textAddDestination.SetEditing(false);
                textAddDestination.SetText("");
                KLVCore.AddDestination(value.ToString());
                KLVCore.Save();
                KLVCore.UpdateAllVehicleNameSchemes();
                comboDestination.SetItems(KLVCore.GetAllDestinationName());
                //Open combo to view new addition
                comboDestination.SetExpanded(true);
            }
        }

        //Settings
        private void DoSave(GUIButton sender, object value)
        {
            KLVCore.Save();
        }

        private void DoLoad(GUIButton sender, object value)
        {
            KLVCore.Load();
            UpdateFamilies();
            comboDestination.SetItems(KLVCore.GetAllDestinationName());
        }
        private void DoUnload(GUIButton sender, object value)
        {
            KLVCore.RefreshAllConfigurations();
            UpdateFamilies();
            comboDestination.SetItems(KLVCore.GetAllDestinationName());
        }

        private void DoIncreaseFont(GUIButton sender, object value)
        {
            klvGUIStyles.AdjustFontSize(1);
        }

        private void DoDecreaseFont(GUIButton sender, object value)
        {
            klvGUIStyles.AdjustFontSize(-1);
        }
    }
}

