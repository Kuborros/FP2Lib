using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchBossRushManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BossRushManager),"Update",MethodType.Normal)]
        static void PatchBossRushManagerUpdate(ref BossRushManager __instance)
        {
            if (__instance.divisionComplete && FPSaveManager.character > FPCharacterID.NEERA)
            {
                Sprite sprite = __instance.playerPortraits[0];

                if (PlayerHandler.currentCharacter != null)
                {
                    sprite = PlayerHandler.currentCharacter.profilePic;
                }
                __instance.sprite.transform.localPosition = new Vector2(__instance.playerPortraitOffset.x, __instance.playerPortraitOffset.y);
                __instance.sprite.sprite = sprite;
            }
        }
    }
}
