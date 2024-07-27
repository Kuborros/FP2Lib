using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchBSAutoscroller
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BSAutoscroller), "Update", MethodType.Normal)]
        static void PatchBSAutoScroller(ref FPHudDigit ___hudDistanceMarker)
        {
            //Add custom chara distance marker
        }
    }
}
