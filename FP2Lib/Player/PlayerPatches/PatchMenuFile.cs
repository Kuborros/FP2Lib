using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuFile
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuFile), "GetFileInfo", MethodType.Normal)]
        static void PatchMenuFileInfo(int fileSlot, MenuFile __instance, ref FPHudDigit[] ___characterIcons)
        {
            if (___characterIcons[fileSlot - 1].digitFrames.Length < 8)
            {
                Sprite heart = ___characterIcons[fileSlot - 1].digitFrames[6];

                for (int i = 5; i < PlayerHandler.highestID; i++)
                {
                    ___characterIcons[fileSlot - 1].digitFrames = ___characterIcons[fileSlot - 1].digitFrames.AddToArray(null);
                }

                foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                {
                    ___characterIcons[fileSlot - 1].digitFrames[chara.id + 1] = chara.livesIconAnim[0];
                }
                ___characterIcons[fileSlot - 1].digitFrames = ___characterIcons[fileSlot - 1].digitFrames.AddToArray(heart);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuFile),"State_Transition",MethodType.Normal)]
        static void PatchMenuTransition()
        {
            //Set the selected character as current character
            PlayerHandler.currentCharacter = PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character);
            PlayerHandler.WriteToStorage();
        }
    }
}
