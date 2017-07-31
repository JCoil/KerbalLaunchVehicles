using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KerbalLaunchVehicles.klvGUI
{
    internal class WindowDefault : MonoBehaviour
    {
        private Texture2D KLVButtonImage;
        private ApplicationLauncherButton Button;
        protected ApplicationLauncher.AppScenes visibleInScenes;
        protected bool isWindowOpen;

        protected Vector2 scrollPos;
        protected HashSet<DropDown> ActiveCombos;
        protected string Title;
        protected ViewTab openTab = ViewTab.Families;
        private bool firstRun = true;

        public virtual void Awake()
        {
            visibleInScenes = ApplicationLauncher.AppScenes.NEVER;
            isWindowOpen = false;
            Title = "Kerbal Launch Vehicles";
            GUIUtilities.Log("[MOD] KLV window awake");
        }

        public virtual void Start()
        {
            KLVCore.Load();
            var App = ApplicationLauncher.Instance;
            KLVButtonImage = GameDatabase.Instance.GetTexture("KerbalLaunchVehicles/Assets/launcherIcon", false);
            Button = App.AddModApplication(OnLauncherButtonPress, OnLauncherButtonPress, null, null, null, null, visibleInScenes, KLVButtonImage);
            GUIUtilities.Log("[MOD] KLV window start");
            ActiveCombos = new HashSet<klvGUI.DropDown>();
            CreateControls();
        }

        public virtual void OnGUI()
        {
            if (firstRun)
            {
                // Things that need to be initialised in OnGui()
                klvGUIStyles.Initialise();
                firstRun = false;
            }

            if (isWindowOpen)
            {
                WindowRect = GUILayout.Window(GetType().FullName.GetHashCode(), WindowRect, OnWindow, Title, klvGUIStyles.StandardWindow, new GUILayoutOption[0]);

                // Need to call popup draws from here as the are new windows   
                int idCounter = 0;
                foreach (var cbo in ActiveCombos)
                {
                    if (cbo.isExpanded)
                    {
                        GUILayout.Window(idCounter, cbo.GetDrawRect().AddPosition(WindowRect), cbo.OnWindow, "", klvGUIStyles.PopupWindow);
                        GUI.BringWindowToFront(idCounter);
                    }
                    idCounter++;
                }
            }
        }

        private Rect _windowRect;

        protected Rect WindowRect
        {
            get
            {
                if (_windowRect == default(Rect))
                {
                    return new Rect(200, 200, 200, 200);
                }
                return _windowRect;
            }
            set
            {
                _windowRect = value;
            }
        }

        public void OnLauncherButtonPress()
        {
            isWindowOpen = !isWindowOpen;
        }


        public virtual void OnWindow(int windowId)
        {

        }

        public void OnDisable()
        {
            isWindowOpen = false;
            if (Button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(Button);
            }
            SaveManager.SaveSettings();
        }

        // Comboboxes

        protected void RegisterCombos(params DropDown[] _combos)
        {
            foreach (var cbo in _combos)
            {
                ActiveCombos.Add(cbo);
            }
        }

        protected void DerigisterCombos(List<DropDown> _combos)
        {
            ActiveCombos.RemoveWhere(x => _combos.Contains(x));
        }

        protected DropDown GetComboFromParent(IGUIControl _parent)
        {
            return ActiveCombos.FirstOrDefault(x => x.Parent == _parent);
        }

        // Tabs
        protected GUIDivider tabDivider;
        protected GUIButton buttonTabVehicles;
        protected GUIButton buttonTabFamilies;
        protected GUIButton buttonTabDestinations;
        protected GUIButton buttonTabSettings;
        protected GUIButton buttonTabSubassemblies;

        protected DropDown comboDestination;

        protected virtual void CreateControls()
        {
            // Tabs
            tabDivider = new klvGUI.GUIDivider();
            buttonTabFamilies = new GUIButton("Families", DoChangeTab, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            buttonTabDestinations = new GUIButton("Destinations", DoChangeTab, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            buttonTabSettings = new GUIButton("Settings", DoChangeTab, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            buttonTabVehicles = new GUIButton("Add Launch Vehicle", DoChangeTab, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
        }

        // Layout

        protected void LayoutGUIList(GUIListNode listNode, int width, int height)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(width), GUILayout.Height(height));
            scrollPos = (Vector2)listNode.DoLayout(null, null, scrollPos);
            GUILayout.EndScrollView();
        }

        private object textOutput;
        protected void LayoutTextInput(GUITextBox textBox, GUIButton button, Func<string, bool> isValid, string invalidMessage, bool showButtonIfEmpty = false,
            GUIStyle validStyle = null, GUIStyle invalidStyle = null)
        {
            GUILayout.BeginHorizontal();
            textOutput = textBox.DoLayout();

            if (!string.IsNullOrEmpty(textOutput.ToString()))
            {
                if (isValid(textOutput.ToString()))
                {
                    button.DoLayout(validStyle ?? klvGUIStyles.StandardButton, null, textOutput.ToString());
                }
                else
                {
                    button.DoLayout(invalidStyle ?? klvGUIStyles.WarningButton, invalidMessage, "");
                }
            }
            else if(showButtonIfEmpty)
            {
                button.DoLayout(invalidStyle ?? klvGUIStyles.WarningButton, "No Name!", "");
            }
            GUILayout.EndHorizontal();
        }

        // Button Actions
        protected virtual void DoChangeTab(GUIButton sender, object value)
        {
            if (value != null && value is ViewTab)
            {
                openTab = (ViewTab)value;
            }
        }

        //protected void DoSelectLaunchConfig(GUIButton sender, object value)
        //{
        //    if (sender == null) { return; } // Button
        //    if (sender.Parent == null) { return; } // Launch config node
        //    if (sender.Parent.Parent == null) { return; } // Vehicle node
        //    if (sender.Parent.Parent.Parent == null) { return; } // Family node

        //    var configNode = (GUIListNode)sender.Parent;
        //    var vehicleNode = (GUIListNode)configNode.Parent;
        //    var familyNode = (GUIListNode)vehicleNode.Parent;

        //    var family = KLVCore.GetVehicleFamily(familyNode.ActionButton.Title);
        //    var vehicle = family.GetVehicle(vehicleNode.ActionButton.Title);
        //    var launchConfig = vehicle.GetLaunchConfig(configNode.ActionButton.Title);

        //    if (family != null && vehicleNode != null && familyNode != null)
        //    {
        //        selectedFamily = family;
        //        selectedVehicle = vehicle;
        //        selectedConfig = launchConfig;

        //        textEditFamily.SetText(selectedFamily.Name);
        //        textEditVehicle.SetText(selectedVehicle.Name);
        //        textEditPayload.SetText(selectedConfig.PayloadMass.ToString("0.##"));
        //        textEditPayload.SetEditing(true);
        //        textEditVehicle.SetEditing(true);
        //        textEditFamily.SetEditing(true);
        //    }
        //}

        //protected void DoSaveConfigEdit(GUIButton sender, object value)
        //{
        //    if(value == null || string.IsNullOrEmpty(value.ToString()))
        //    {
        //        return;
        //    }

        //    selectedConfig.SetPayload(payloadValue);
        //    selectedVehicle.SetName(vehicleOutput.ToString());
        //    selectedFamily.SetName(familyOutput.ToString());

        //    DoCancelConfigEdit(sender, null);

        //    KLVCore.Save();
        //    KLVCore.UpdateAllVehicleNameSchemes();
        //    familyList = GUIListNode.CreateListNode3Lvl(KLVCore.GetFullVehicleSummary());
        //}

        //protected void DoCancelConfigEdit(GUIButton sender, object value)
        //{
        //    selectedFamily = null;
        //    selectedConfig = null;
        //    selectedVehicle = null;

        //    textEditPayload.SetEditing(false);
        //    textEditVehicle.SetEditing(false);
        //    textEditFamily.SetEditing(false);

        //    textEditFamily.SetText("");
        //    textEditVehicle.SetText("");
        //    textEditPayload.SetText("");
        //}
    }
    internal enum ViewTab
    {
        Vehicles,
        Families,
        Destinations,
        Settings,
        Suggestions
    }
}
