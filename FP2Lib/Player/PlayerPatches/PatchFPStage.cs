﻿using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPStage
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
        static void PatchStart(ref FPPlayer[] ___playerList)
        {
            GameObject spadeObject = Plugin.moddedBundle.LoadAsset<GameObject>("Player Spade");
            ___playerList = ___playerList.AddItem(spadeObject.GetComponent<FPPlayer>()).ToArray();

            if (FPStage.stageNameString != "Nalao Lake" || FPStage.stageNameString != "The Battlesphere")
            {
                PatchBFFMicroMissile.BFFActive = false;
            }
            if (FPStage.stageNameString == "Bakunawa Chase") PatchBFFMicroMissile.BFFActive = true;
        }
    }
}
