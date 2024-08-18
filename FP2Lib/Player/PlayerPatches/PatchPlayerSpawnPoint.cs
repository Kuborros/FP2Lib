using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchPlayerSpawnPoint
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSpawnPoint), "Start", MethodType.Normal)]
        static void PatchSpawnPointStart(ref AudioClip[] ___characterMusic)
        {
            //Unused, but game *will* explode if this is missing...
            //Mod can add their own tracks there
            for (int i = 0; i < PlayerHandler.highestID;i++)
            ___characterMusic = ___characterMusic.AddToArray(null);
        }
    }
}
