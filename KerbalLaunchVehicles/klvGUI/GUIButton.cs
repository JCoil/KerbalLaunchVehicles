using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    internal delegate void DoButtonPress(GUIButton sender, object value);

    internal class GUIButton : IGUIControl
    {
        protected bool isPressed;
        internal string Title { get; private set; }
        protected GUILayoutOption[] Options;

        protected DoButtonPress Action;

        public IGUIControl Parent { get; set; }

        internal GUIButton(string _title, DoButtonPress _action, params GUILayoutOption[] _options)
        {
            Title = _title;
            Action = _action ?? EmptyButtonPress;
            Options = _options ?? new GUILayoutOption[0];
        }

        internal void SetText(string text)
        {
            Title = text;
        }

        public object DoLayout(GUIStyle style = null, object newValue = null, object input = null)
        {
            if (GUILayout.Button(newValue == null ? Title : newValue.ToString(), 
                style ?? klvGUIStyles.StandardButton, Options))
            {

                if (!isPressed)
                {
                    Action(this, input);
                    isPressed = true;
                }
            }
            else
            {
                isPressed = false;
            }

            return isPressed;
        }

        private static void EmptyButtonPress(GUIButton sender, object value)
        {

        }
    }
}
