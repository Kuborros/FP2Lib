using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuFile
    {
        private static bool clearReplaced = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuFile), "State_Main", MethodType.Normal)]
        static void PatchMenuFileStateMain(MenuFile __instance)
        {
                
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuFile), "GetFileInfo", MethodType.Normal)]
        static void PatchMenuFileInfo(int fileSlot, MenuFile __instance, ref FPHudDigit[] ___characterIcons)
        {
            if (___characterIcons[fileSlot - 1].digitFrames.Length < 8)
            {
                
            }
        }
    }
}
