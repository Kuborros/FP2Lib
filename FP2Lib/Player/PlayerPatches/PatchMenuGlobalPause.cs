using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuGlobalPause
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause),"Start",MethodType.Normal)]
        static void PatchMenuGlobalPauseStart(ref MenuGlobalPause __instance)
        {
            //Add pause screen character sprite + disable tutorials
        }
    }
}
