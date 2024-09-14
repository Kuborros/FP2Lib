using HarmonyLib;
using System;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchParentActivator
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ParentActivator), "Start", MethodType.Normal)]
        static void PatchPAStart(ParentActivator __instance)
        {
            //If no extra characters are loaded, abort patch.
            if (FPSaveManager.character >= (FPCharacterID)5)
            {
                if (!PlayerHandler.currentCharacter.useOwnCutsceneActivators)
                {
                    if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.LILAC && !__instance.lilac)
                    {
                        DisableObject(__instance);
                    }
                    if ((PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.CAROL
                      || PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.BIKECAROL) && !__instance.carol)
                    {
                        DisableObject(__instance);
                    }
                    if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.MILLA && !__instance.milla)
                    {
                        DisableObject(__instance);
                    }
                    if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.NEERA && !__instance.neera)
                    {
                        DisableObject(__instance);
                    }
                }
            }
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ParentActivator), "DisableObject", MethodType.Normal)]
        public static void DisableObject(ParentActivator instance)
        {
            // Replaced at runtime with reverse patch
            throw new NotImplementedException("Method failed to reverse patch!");
        }
    }
}
