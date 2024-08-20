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
            for (int i = 5; i < PlayerHandler.highestID; i++)
            {
                ___bgmCredits = ___bgmCredits.AddToArray(null);
                ___characterSprites = ___characterSprites.AddToArray(null);
            }

            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                if (chara.registered)
                {
                    ___bgmCredits[chara.id] = chara.endingTrack;
                    ___characterSprites[chara.id] = chara.endingKeyArtSprite;
                }
            }
        }
    }
}
