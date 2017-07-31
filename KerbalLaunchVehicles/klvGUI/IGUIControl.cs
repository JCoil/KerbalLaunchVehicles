using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    interface IGUIControl
    {
        IGUIControl Parent { get; set; }
        object DoLayout(GUIStyle style, object input, object newValue);
    }
}
