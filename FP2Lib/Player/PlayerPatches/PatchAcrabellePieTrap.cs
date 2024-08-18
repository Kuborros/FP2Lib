using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchAcrabellePieTrap
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AcrabellePieTrap), "Start")]
        private static void PatchAcrabellePieTrapStart(ref Sprite[] ___characterBase, ref Sprite[] ___characterStruggle)
        {
            if (___characterBase.Length <= PlayerHandler.highestID)
            {
                for (int i = 4; i < PlayerHandler.PlayableChars.Count + 4; i++)
                {
                    ___characterBase = ___characterBase.AddToArray(null).ToArray();
                    ___characterStruggle = ___characterStruggle.AddToArray(null).ToArray();
                }
            }

            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {

                Sprite Pied = chara.piedSprite;
                Sprite PiedHurt = chara.piedHurtSprite;

                ___characterBase[chara.id] = Pied;
                ___characterStruggle[chara.id] = PiedHurt;
            }
        }
    }
}
