using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchBakunawaInstakill
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BFSyntaxHunt), "Start", MethodType.Normal)]
        static void PatchBFSyntaxHuntStart(ref GameObject ___playerBody)
        {
            if (FPSaveManager.character >= (FPCharacterID)5)
            {
                PlayableChara character = PlayerHandler.currentCharacter;
                if (character != null)
                {
                    if (!character.useOwnCutsceneActivators && character.bfImpaleSprite != null)
                    {
                        SpriteRenderer[] sprites = ___playerBody.transform.GetComponentsInChildren<SpriteRenderer>(true);
                        foreach (SpriteRenderer spriteRenderer in sprites)
                        {
                            spriteRenderer.sprite = character.bfImpaleSprite;
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bakunawa), "Start", MethodType.Normal)]
        static void PatchBakunawaStart(ref GameObject ___players)
        {
            if (FPSaveManager.character >= (FPCharacterID)5)
            {
                PlayableChara character = PlayerHandler.currentCharacter;
                if (character != null)
                {
                    if (!character.useOwnCutsceneActivators && character.bfImpaleSprite != null)
                    {
                        SpriteRenderer[] sprites = ___players.transform.GetComponentsInChildren<SpriteRenderer>(true);
                        foreach (SpriteRenderer spriteRenderer in sprites)
                        {
                            spriteRenderer.sprite = character.bfImpaleSprite;
                        }
                    }
                }
            }
        }
    }
}
