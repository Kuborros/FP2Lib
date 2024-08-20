using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPHudDigit
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudDigit), "SetDigitValue", MethodType.Normal)]
        static void PatchFPHudDigitValue(FPHudDigit __instance, ref Sprite[] ___digitFrames)
        {
            if (__instance.name == "PortraitCharacter")
            {
                if (___digitFrames.Length <= 6)
                {
                    //Extend array
                    for (int i = 4; i <= PlayerHandler.highestID +1; i++)
                    {
                        ___digitFrames = ___digitFrames.AddToArray(null);
                    }
                    
                    //Load profile pic
                    foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                    {
                        ___digitFrames[chara.id] = chara.profilePic;
                    }
                }
            }
        }
    }
}
