﻿using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchArenaRace
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArenaRace), "Update", MethodType.Normal)]
        static void PatchArenaRaceStart(ArenaRace __instance, ref FPHudDigit ___hudDistanceMarker)
        {
            //if (___hudDistanceMarker.digitFrames.Length < 16)
            //{
            //    ___hudDistanceMarker.digitFrames = ___hudDistanceMarker.digitFrames.AddToArray(Plugin.moddedBundle.LoadAssetWithSubAssets<Sprite>("Spade_Stock")[0]);
            //}
            //if (FPStage.currentStage.GetPlayerInstance_FPPlayer().characterID == (FPCharacterID)5)
            //{
            //    ___hudDistanceMarker.SetDigitValue(15);
            //}
        }
    }
} 
