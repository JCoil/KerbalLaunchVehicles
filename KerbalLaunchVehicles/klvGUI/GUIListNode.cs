using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    internal class GUIListNode : IGUIControl
    {
        private string[] Titles;
        private string FullText;
        private bool isRoot;
        private bool isCollapsed;
        private bool hasAction;
        private int levelDepth;

        private string expIcon;
        private string colIcon;

        private GUIStyle levelStyle;

        internal int Count { get { return Nodes.Count; } }

        private List<GUIListNode> Nodes;
        internal GUIButton ActionButton { get; private set; }
        public IGUIControl Parent { get; set; }

        internal GUIListNode(string[] _titles, bool _hasAction, DoButtonPress buttonAction, bool _isCollapsed, int _depth, bool _isRoot = false, int _width = 100)
        {
            Nodes = new List<GUIListNode>();
            SetIcons("+ ", "-  ");
            Titles = _titles;
            isRoot = _isRoot;
            isCollapsed = _isCollapsed;
            hasAction = _hasAction;
            levelDepth = _depth;
            FullText = string.Join(" ", Titles);
            ActionButton = new GUIButton(Titles[Titles.Length - 1], buttonAction ?? ToggleCollapsed, new GUILayoutOption[] { GUILayout.Width(_width), GUILayout.ExpandWidth(true) });
            ActionButton.Parent = this;
            levelStyle = null;
        }

        internal void AddNode(GUIListNode newNode)
        {
            if (newNode != null)
            {
                Nodes.Add(newNode);
                newNode.Parent = this;
            }
        }

        public object DoLayout(GUIStyle style = null, object newValue = null, object scrollPos = null)
        {
            if (!isRoot)
            {
                if (hasAction)
                {
                    ActionButton.DoLayout(style ?? klvGUIStyles.HighlightHeader, NestOffset(levelDepth - 1) + (isCollapsed ? expIcon : colIcon) + FullText, FullText);
                }
                else
                {
                    GUILayout.Label(NestOffset(levelDepth - 1) + FullText, levelStyle ?? klvGUIStyles.HighlightBody, GUILayout.Height(22), GUILayout.ExpandWidth(false));
                }
            }

            if (!isCollapsed)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    scrollPos = (Vector2)Nodes[i].DoLayout(klvGUIStyles.HighlightBody, null, scrollPos);
                }
            }

            return scrollPos;
        }

        internal void ToggleCollapsed(GUIButton sender, object input)
        {
            isCollapsed = !isCollapsed;
        }

        internal void SetCollapsed(bool collapse)
        {
            isCollapsed = collapse;
        }

        internal void SetCollapsedAll(bool collapse)
        {
            SetCollapsed(collapse);
            if (Nodes != null)
            {
                foreach (var child in Nodes)
                {
                    child.SetCollapsedAll(collapse);
                }
            }
        }

        internal bool HasItem(string item)
        {
            bool hasItem = false;

            if(Titles.Contains(item))
            {
                hasItem = true;
            }

            foreach(var child in Nodes)
            {
                if(child.HasItem(item))
                {
                    hasItem = true;
                }
            }

            return hasItem;
        }

        internal void SetIcons(string _expandIcon, string _collapseIcon)
        {
            expIcon = _expandIcon;
            colIcon = _collapseIcon;
        }

        internal void SetLevelStyle(int lvl, GUIStyle  style)
        {
            if(lvl == levelDepth)
            {
                levelStyle = style;
            }
            else if(Nodes != null && Nodes.Count > 0)
            {
                foreach(var child in Nodes)
                {
                    child.SetLevelStyle(lvl, style);
                }
            }
        }

        //Factory

        internal static GUIListNode CreateListNode1Lvl(List<string[]> allNodes, DoButtonPress Action = null)
        {
            GUIListNode rootNode = new GUIListNode(new string[] { "" }, false, null, false, 0, true);

            foreach (var value in allNodes)
            {
                rootNode.AddNode(new GUIListNode(value, Action != null, Action, false, 1));
            }

            return rootNode;
        }
        internal static GUIListNode CreateListNode2Lvl(Dictionary<string[], List<string[]>> allNodes, 
            DoButtonPress lvl2Action = null, DoButtonPress lvl1Action = null)
        {
            GUIListNode rootNode = new GUIListNode(new string[] { "" }, false, null, false, 0, true);

            foreach (var lvl1Pair in allNodes)
            {
                var lvl1Node = new GUIListNode(lvl1Pair.Key, true, lvl1Action, true, 1);

                foreach (var value in lvl1Pair.Value)
                {
                    lvl1Node.AddNode(new GUIListNode(value, lvl2Action != null, lvl2Action, false, 2));
                }
                rootNode.AddNode(lvl1Node);
            }
            return rootNode;
        }

        internal static GUIListNode CreateListNode3Lvl(Dictionary<string[], Dictionary<string[], List<string[]>>> allNodes, 
            DoButtonPress lvl3Action = null, DoButtonPress lvl2Action = null, DoButtonPress lvl1Action = null)
        {
            GUIListNode rootNode = new GUIListNode(new string[] { "" }, false, null, false, 0, true);

            foreach (var lvl1Pair in allNodes)
            {
                var lvl1Node = new GUIListNode(lvl1Pair.Key, true, lvl1Action, true, 1);

                foreach (var lvl2Pair in lvl1Pair.Value)
                {
                    var lvl2Node = new GUIListNode(lvl2Pair.Key, true, lvl2Action, true, 2);

                    foreach (var value in lvl2Pair.Value)
                    {
                        lvl2Node.AddNode(new GUIListNode(value, lvl3Action != null, lvl3Action, false, 3));
                    }

                    lvl1Node.AddNode(lvl2Node);
                }
                rootNode.AddNode(lvl1Node);
            }
            return rootNode;
        }

        private static string NestOffset(int number)
        {
            return new string(' ', number * 4);
        }
    }
}
