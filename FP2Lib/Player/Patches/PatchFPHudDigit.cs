using FP2Lib.Player;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FP2Lib.Player.Patches
{
    internal class PatchFPHudDigit
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudDigit), "SetDigitValue", MethodType.Normal)]
        static void PatchFPHudDigitValue(FPHudDigit __instance, ref Sprite[] ___digitFrames)
        {
            if (__instance.name == "PortraitCharacter")
            {
                if (___digitFrames[5] == null)
                {
                    ___digitFrames = ___digitFrames.Take(5).ToArray();
                    foreach (PlayableChara character in PlayerHandler.PlayableChars.Values)
                    {
                        ___digitFrames = ___digitFrames.AddToArray(character.profilePic);
                    }
                    ___digitFrames = ___digitFrames.AddToArray(null);
                }
            }
        }
    }
}
