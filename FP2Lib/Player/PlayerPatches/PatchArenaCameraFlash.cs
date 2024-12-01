using HarmonyLib;
using UnityEngine.SceneManagement;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchArenaCameraFlash
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArenaCameraFlash), "Start", MethodType.Normal)]
        static void PatchArenaFlash(ref int[] ___voicePlayerKO)
        {
            if (SceneManager.GetActiveScene().name != "Cutscene_BattlesphereEnding" && FPSaveManager.character >= (FPCharacterID)5)
            {
                if (___voicePlayerKO != null && PlayerHandler.currentCharacter.Gender == CharacterGender.MALE)
                {
                    //Swap to male variant
                    for (int i = 0; i < ___voicePlayerKO.Length; i++)
                    {
                        if (___voicePlayerKO[i] == 8) ___voicePlayerKO[i] = 9;
                    }
                }
                else if (___voicePlayerKO != null && PlayerHandler.currentCharacter.Gender == CharacterGender.NON_BINARY)
                {
                    //Swap to non-gendered line
                    for (int i = 0; i < ___voicePlayerKO.Length; i++)
                    {
                        if (___voicePlayerKO[i] == 8) ___voicePlayerKO[i] = 0;
                    }
                }
            }
        }
    }
}
