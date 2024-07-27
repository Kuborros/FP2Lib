using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuCredits
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCredits), "Start", MethodType.Normal)]
        static void PatchMenuCreditsStart(ref AudioClip[] ___bgmCredits, ref Sprite[] ___characterSprites)
        {
            //Load per-character ending art
        }
    }
}
