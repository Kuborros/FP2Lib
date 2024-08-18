using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPBossHud
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPBossHud), "Start", MethodType.Normal)]
        static void PatchFPBossHudStart(FPBossHud __instance)
        {
            //We fightin' everyone, so move the health bars
            if (FPSaveManager.character <= (FPCharacterID)5)
            {
                switch (__instance.name)
                {
                    case "Boss Lilac":
                        __instance.healthBarOffset = new Vector2(-240, 16);
                        break;
                    case "Boss Carol":
                        __instance.healthBarOffset = new Vector2(-80, 16);
                        break;
                    case "Boss Milla":
                        __instance.healthBarOffset = new Vector2(80, 16);
                        break;
                    case "Boss Neera":
                        __instance.healthBarOffset = new Vector2(240, 0);
                        break;
                }
            }
        }
    }
}
