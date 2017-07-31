using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    internal class GUIDivider : IGUIControl
    {
        public IGUIControl Parent { get; set; }

        internal GUIDivider()
        {

        }

        public object DoLayout(GUIStyle style = null, object input = null, object newValue = null)
        {
            GUILayout.Box("", klvGUIStyles.Divider, GUILayout.Height(1));
            GUILayout.Space(10);
            return null;
        }
    }
}
