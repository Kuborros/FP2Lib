using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPHudMaster
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPHudMaster), "Start", MethodType.Normal)]
        static void PatchHudMasterStart(ref FPHudDigit ___hudRevive)
        {
            if (___hudRevive.gameObject.GetComponents<FPHudDigit>().Length != 0)
            {
                foreach (FPHudDigit digit in ___hudRevive.gameObject.GetComponentsInChildren<FPHudDigit>())
                {
                    if (digit.name == "Hud Life Icon" && digit.digitFrames.Length <= 6 && PlayerHandler.PlayableChars.Count > 0)
                    {
                        Sprite heart = digit.digitFrames[5];

                        for(int i = 5; i <= PlayerHandler.highestID; i++)
                        {
                            digit.digitFrames = digit.digitFrames.AddToArray(heart);
                        }

                        foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                        {
                            digit.digitFrames[chara.id] = chara.livesIconAnim[0];
                        }
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPHudMaster), "LateUpdate", MethodType.Normal)]
        static void PatchHudMasterLateUpdate(FPHudMaster __instance, FPHudDigit[] ___hudLifeIcon, float ___lifeIconBlinkTimer)
        {
            if (__instance.targetPlayer.characterID == (FPCharacterID)5)
            {
                if (___hudLifeIcon[0].digitFrames.Length < 16)
                {
                    Sprite[] stock = PlayerHandler.currentCharacter.livesIconAnim;

                    ___hudLifeIcon[0].digitFrames = ___hudLifeIcon[0].digitFrames.AddToArray(stock[0]);
                    ___hudLifeIcon[0].digitFrames = ___hudLifeIcon[0].digitFrames.AddToArray(stock[1]);
                    ___hudLifeIcon[0].digitFrames = ___hudLifeIcon[0].digitFrames.AddToArray(stock[2]);
                    ___hudLifeIcon[0].digitFrames = ___hudLifeIcon[0].digitFrames.AddToArray(stock[1]);
                }
                ___hudLifeIcon[0].SetDigitValue(Mathf.Max(15, 15 + (int)___lifeIconBlinkTimer % 3));
            }
        }
    }
}
