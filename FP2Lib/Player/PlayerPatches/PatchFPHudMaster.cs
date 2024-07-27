using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPHudMaster
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudMaster), "LateUpdate", MethodType.Normal)]
        static void PatchHudMasterLateUpdate(FPHudMaster __instance, FPHudDigit[] ___hudLifeIcon, float ___lifeIconBlinkTimer)
        {
            //Add lives animation for hud and animate them
        }
    }
}
