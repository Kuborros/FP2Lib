using HarmonyLib;
using System;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchArenaSpawner
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(ArenaSpawner), "Start", MethodType.Normal)]
        static void PatchArenaFlash(ArenaSpawner __instance)
        {
            if (__instance.useGlobalChallengeID && FPSaveManager.currentArenaChallenge == 4) //Hero Battle Royale
            {
                if (__instance.challenges[4].roundObjectList[0].objectList.Length == 3)
                {
                    Array.Resize(ref __instance.challenges[4].roundObjectList[0].objectList, 4);
                }
            }
        }
    }
}
