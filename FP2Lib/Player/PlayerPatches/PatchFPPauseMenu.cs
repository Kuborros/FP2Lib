using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPPauseMenu
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPauseMenu), "Start", MethodType.Normal)]
        static void PatchFPPauseMenuStart(ref FPPauseMenu __instance)
        {
            //Just to be 100% sure we got it right.
            if (__instance != null && FPSaveManager.character >= (FPCharacterID)5)
            {
                //Replace instruction menu with custom one
                if (PlayerHandler.currentCharacter.menuInstructionPrefab != null)
                {
                    __instance.guideMenu = PlayerHandler.currentCharacter.menuInstructionPrefab.GetComponent<MenuInstructions>();
                }
            }
        }
    }
}
