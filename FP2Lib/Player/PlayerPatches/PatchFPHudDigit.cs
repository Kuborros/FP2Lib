using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPHudDigit
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudDigit), "SetDigitValue", MethodType.Normal)]
        static void PatchFPHudDigitValue(FPHudDigit __instance, ref Sprite[] ___digitFrames)
        {
            //Add portrait for saves
        }
    }
}
