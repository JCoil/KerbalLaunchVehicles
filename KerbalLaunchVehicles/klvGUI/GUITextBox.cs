using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    class GUITextBox : IGUIControl
    {
        private string LabelLeft;
        private string LabelRight;
        private string Text;
        private string NewText;
        private int TextWidth;
        private int TotalWidth;
        internal bool isEditing { get; private set; }
        public IGUIControl Parent { get; set; }

        private DoButtonPress EnterPressedAction;

        internal GUITextBox(string _labelLft = "", string _text = "", string _labelRt = "", int _textWidth = 100, int _totalWidth = 200, 
            DoButtonPress _enter = null)
        {
            LabelLeft = _labelLft;
            LabelRight = _labelRt;
            Text = _text;
            NewText = _text;
            TextWidth = _textWidth;
            TotalWidth = _totalWidth;
            EnterPressedAction = _enter;
        }

        public object DoLayout(GUIStyle style = null, object newValue = null, object input = null)
        {
            Text = NewText;
            GUILayout.BeginHorizontal(GUILayout.Width(TotalWidth), GUILayout.ExpandWidth(true));
            GUILayout.Label(LabelLeft, style?? klvGUIStyles.StandardLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            if(Event.current.Equals(Event.KeyboardEvent("return")))
            {
                EnterPressedAction(null, Text.Trim());
            }
            NewText = GUILayout.TextField(Text, klvGUIStyles.StandardTextBox, GUILayout.Width(TextWidth));
            GUILayout.Label(LabelRight, style ?? klvGUIStyles.StandardLabel, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            // Return null if text hasn't changed
            if(Text != NewText)
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

        internal string GetText()
        {
            return NewText;
        }
    }
}
