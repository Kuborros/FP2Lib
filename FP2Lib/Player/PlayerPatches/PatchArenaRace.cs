using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchArenaRace
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArenaRace), "Update", MethodType.Normal)]
        static void PatchArenaRaceStart(ref FPHudDigit ___hudDistanceMarker)
        {
            if (FPSaveManager.character >= (FPCharacterID)5)
            {
                if (___hudDistanceMarker.digitFrames.Length < 16)
                {
                    ___hudDistanceMarker.digitFrames = ___hudDistanceMarker.digitFrames.AddToArray(PlayerHandler.currentCharacter.livesIconAnim[0]);
                }
                if (FPStage.currentStage.GetPlayerInstance_FPPlayer().characterID >= (FPCharacterID)5)
                {
                    ___hudDistanceMarker.SetDigitValue(15);
                }
            }
        }
    }
}
