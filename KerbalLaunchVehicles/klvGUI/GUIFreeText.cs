using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    class GUIFreeText
    {
        private string Text;
        private string NewText;
        private int Width;
        private int Height;
        internal bool isEditing { get; private set; }
        public IGUIControl Parent { get; set; }

        internal GUIFreeText(string _text = "", int _width = 100, int _height = 40)
        {
            Text = _text;
            NewText = _text;
            Width = _width;
            Height = _height;
        }

        public object DoLayout(GUIStyle style = null, object newValue = null, object input = null)
        {
            Text = NewText;
            NewText = GUILayout.TextArea(Text, klvGUIStyles.FreeTextBox, GUILayout.Width(Width), GUILayout.Height(Height));

            // Return null if text hasn't changed
            if (Text != NewText)
            {
                isEditing = true;
            }

            return isEditing ? NewText : "";
        }

        internal void SetEditing(bool _edit)
        {
            isEditing = _edit;
        }

        internal void SetText(string _text)
        {
            Text = _text;
            NewText = _text;
        }
    }
}
