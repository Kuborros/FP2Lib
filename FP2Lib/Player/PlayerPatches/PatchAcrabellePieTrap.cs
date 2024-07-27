using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchAcrabellePieTrap
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AcrabellePieTrap),"Start")]
        private static void PatchAcrabellePieTrapStart(ref Sprite[] ___characterBase,ref Sprite[] ___characterStruggle)
        {
            if (FPSaveManager.character <= (FPCharacterID)5)
            {
            }
        }
    }
}
