using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalLaunchVehicles
{
    internal static class Extensions
    {
        internal static Rect AddPosition(this Rect rectA, Rect rectB)
        {
            rectA.position += rectB.position;
            return rectA;
        }
        internal static Rect AddPosition(this Rect rectA, Vector2 posB)
        {
            rectA.position += posB;
            return rectA;
        }

        internal static int Limit(this int value, int upperInc, int lowerInc)
        {
            return value > upperInc ? upperInc : (value < lowerInc ? lowerInc : value);
        }
    }
}
