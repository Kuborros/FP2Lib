using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FP2Lib.Player.Patches
{
    internal class PatchPlayerSpawnPoint
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSpawnPoint), "Start", MethodType.Normal)]
        static void PatchSpawnPointStart(ref AudioClip[] ___characterMusic)
        {
            for (int i = 1; i <= FP2Lib.Player.Patches.PlayableChars.Count; i++)
                ___characterMusic = ___characterMusic.AddToArray(null);
        }
    }
}
