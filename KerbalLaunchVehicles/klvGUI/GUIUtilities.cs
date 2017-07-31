using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles.klvGUI
{
    internal static class GUIUtilities
    {
        internal static Vector2 CreateScrollBox(Vector2 scrollPos, int width, int height, List<string> items)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(width), GUILayout.Height(height));
            for (int i = 0; i < items.Count; i++)
            {
                GUILayout.Label(items[i]);
            }
            GUILayout.EndScrollView();
            return scrollPos;
        }

        internal static void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
