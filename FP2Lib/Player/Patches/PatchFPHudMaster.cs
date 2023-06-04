using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FP2Lib.Player.Patches
{
    internal class PatchFPHudMaster
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudMaster), "LateUpdate", MethodType.Normal)]
        static void PatchHudMasterLateUpdate(FPHudMaster __instance, FPHudDigit[] ___hudLifeIcon)
        {
            if (__instance.targetPlayer.characterID >= (FPCharacterID)5)
            {
                if (___hudLifeIcon[0].digitFrames.Length < 16)
                {
                    ___hudLifeIcon[0].digitFrames = ___hudLifeIcon[0].digitFrames.AddRangeToArray(FP2Lib.Player.Patches.currentCharacter.livesIconAnim);
                }
                ___hudLifeIcon[0].SetDigitValue(16);
            }
        }
    }
}
