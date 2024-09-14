using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuGlobalPause
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "Start", MethodType.Normal)]
        static void PatchMenuGlobalPauseStart(ref MenuGlobalPause __instance)
        {
            //Just to be 100% sure we got it right.
            if (__instance != null && FPSaveManager.character >= (FPCharacterID)5)
            {
                PlayerHandler.currentCharacter = PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character);
                PlayableChara chara = PlayerHandler.currentCharacter;
                int npcnumber = FPSaveManager.GetNPCNumber(chara.Name);
                if (npcnumber > 0)
                {
                    //Only set it if we _find_ an NPC with their name. They might not exist for all we know.
                    FPSaveManager.npcFlag[npcnumber] = (byte)Mathf.Max(1, FPSaveManager.npcFlag[npcnumber]);
                }
                __instance.playerSprites = __instance.playerSprites.AddToArray(chara.worldMapPauseSprite);
                __instance.playerInfoSprite.sprite = __instance.playerSprites[4];
                __instance.playerInfoName.GetComponent<TextMesh>().text = chara.Name;

                //No touchie for now
                //__instance.menuOptions[2].locked = true;

            }
        }
    }
}
