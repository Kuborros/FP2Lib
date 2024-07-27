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
            for (int i = 0; i < PlayerHandler.PlayableChars.Count;i++)
            ___characterMusic = ___characterMusic.AddToArray(null);
        }
    }
}
