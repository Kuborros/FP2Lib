using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchCutsceneDialog
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CutsceneDialog), "DialogCharacterMatch", MethodType.Normal)]
        static void PatchDialogCharacterMatch(int i, CutsceneDialog __instance, ref bool __result)
        {
            if (FPSaveManager.character > FPCharacterID.NEERA)
            {
                PlayableChara character = PlayerHandler.currentCharacter;
                if (character != null) 
                {
                    if (character.useOwnCutsceneActivators)
                    {
                        if (__instance.dialogSequence[i].characters.Length >= character.id)
                        {
                            __result = __instance.dialogSequence[i].characters[character.id];
                        }
                    }
                    else 
                        __result = __instance.dialogSequence[i].characters[(int)character.eventActivatorCharacter];
                }
            }
        }
    }
}
