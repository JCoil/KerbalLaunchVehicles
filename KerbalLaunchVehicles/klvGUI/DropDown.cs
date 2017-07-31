using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    internal class DropDown
    {
        private GUIButton Button;
        private GUIListNode listNode;
        private Rect drawRect;

        internal string selectedItemName { get; private set; }
        internal bool isItemSelected { get; private set; }
        internal bool isExpanded { get; private set; }
        internal bool canExpand { get; private set; }
        private static Vector2 scrollPos = Vector2.zero;

        private string defaultText = "Select";
        private string emptyText = "None";
        
        private string arrowDownIcon = "▼";
        private string arrowUpIcon = "▲";

        private readonly Vector2 buttonOffset = new Vector2(0, 21);
        internal IGUIControl Parent { get; private set; }

        public DropDown(Vector2  _dropdownSize, List<string> items, IGUIControl _parent = null, string titleText = "Please select")
        {
            isExpanded = false;

            defaultText = titleText;
            Button = new GUIButton(arrowDownIcon + defaultText + arrowDownIcon, DoButtonPress, new GUILayoutOption[] { GUILayout.Width(_dropdownSize.x + 7) });
            SetItems(items);
            
            drawRect = new Rect(buttonOffset, _dropdownSize);
            Parent = _parent;
        }

        public void DoLayout(GUIStyle style, bool isEnabled = true)
        {
            canExpand = isEnabled;

            Button.DoLayout(style, null, selectedItemName);

            if (Event.current.type == EventType.Repaint)
            {
                drawRect.position = GUILayoutUtility.GetLastRect().position;
                drawRect = drawRect.AddPosition(buttonOffset);
            }
        }

        internal void OnWindow(int windowId)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(drawRect.width), GUILayout.Height(drawRect.height));
            scrollPos = (Vector2)listNode.DoLayout(null, null, scrollPos);
            GUILayout.EndScrollView();
        }

        private void DoItemSelected(GUIButton sender, object value)
        {
            selectedItemName = value.ToString();
            isItemSelected = true;
            SetExpanded(false);
            UpdateButtonText();
        }
        internal void SetExpanded(bool _expanded)
        {
            if(!canExpand)
            {
                return;
            }

            isExpanded = _expanded;

            if(isExpanded && expandAction != null)
            {
                expandAction(this);
            }
            else if(collapseAction != null)
            {
                collapseAction(this);
            }

            UpdateButtonText();
        }

        private void DoButtonPress(GUIButton sender, object value)
        {
            SetExpanded(!isExpanded);
            UpdateButtonText();
        }

        internal void SetItems(IEnumerable<string> newItems)
        {
            listNode = GUIListNode.CreateListNode1Lvl(newItems.Select(x => new string[] { x }).ToList(), DoItemSelected);
            listNode.SetIcons(" ", " ");
            selectedItemName = "";
            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            if (listNode.Count < 1)
            {
                Button.SetText(emptyText);
            }
            else
            {
                // Add arrows to text
                Button.SetText(String.Format("{1}   {0}", (isExpanded ? arrowUpIcon : arrowDownIcon), (isItemSelected ? selectedItemName : defaultText)));
            }
        }

        internal void Deselect()
        {
            isItemSelected = false;
            selectedItemName = "";
            UpdateButtonText();
        }

        internal void SetSelection(string text)
        {
            if (!string.IsNullOrEmpty(text) && listNode.HasItem(text))
            {
                selectedItemName = text;
                isItemSelected = true;
                UpdateButtonText();
            }
        }

        internal Rect GetDrawRect()
        {
            return drawRect;
        }

        // Events

        private doCollapse collapseAction;
        internal delegate void doCollapse(DropDown sender);
        private doExpand expandAction;
        internal delegate void doExpand(DropDown sender);

        internal void SetCollapseAction(doCollapse _collapse)
        {
            collapseAction = _collapse;
        }
        internal void SetExpandAction(doExpand _expand)
        {
            expandAction = _expand;
        }
    }
}
