using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuCredits
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCredits), "Start", MethodType.Normal)]
        static void PatchMenuCreditsStart(ref AudioClip[] ___bgmCredits, ref Sprite[] ___characterSprites)
        {
            // Loop through each custom player character.
            for (int i = 0; i < PlayerHandler.PlayableChars.Values.Count; i++)
            {
                // Get the player value at this index.
                var entry = PlayerHandler.PlayableChars.ElementAt(i).Value;

                // Add their ending track and key art to the approriate arrays.
                ___bgmCredits = ___bgmCredits.AddToArray(entry.endingTrack);
                ___characterSprites = ___characterSprites.AddToArray(entry.endingKeyArtSprite);
            }
        }
    }
}
