using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPResultsMenu
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPResultsMenu),"Update",MethodType.Normal)]
        static bool PatchResultsMenuUpdate(ref int ___animationStep, ref FPResultsMenu __instance, ref float ___genericTimer, int ___menuSelection)
        {
            if (FPStage.state == FPStageState.STATE_PAUSED)
            {
                return true;
            }

            //Only run for modded characters in adventure mode.
            if (___animationStep == 4 && FPSaveManager.character > FPCharacterID.NEERA && FPSaveManager.gameMode == FPGameMode.ADVENTURE && PlayerHandler.currentCharacter != null)
            {
                ___genericTimer += FPStage.deltaTime;
                if (___genericTimer < 50f || ___menuSelection == 1)
                {
                    return true;
                }
                FPScreenTransition component = GameObject.Find("Screen Transition").GetComponent<FPScreenTransition>();
                component.transitionType = FPTransitionTypes.WIPE;
                component.transitionSpeed = 48f;
                if (FPSaveManager.gameMode == FPGameMode.ADVENTURE || FPStage.currentStage.alwaysForceCutscene)
                {
                    bool flag = false;
                    if (__instance.adventureCutscene.Length > 0)
                    {
                        for (int i = 0; i < __instance.adventureCutscene.Length; i++)
                        {
                            //If they have own player activators then the normal code will do fine
                            if (!PlayerHandler.currentCharacter.useOwnCutsceneActivators)
                            {
                                if (PlayerHandler.currentCharacter.eventActivatorCharacter == __instance.adventureCutscene[i].character && FPSaveManager.storyFlag[__instance.adventureCutscene[i].storyFlag] <= __instance.adventureCutscene[i].storyFlagValue)
                                {
                                    component.sceneToLoad = __instance.adventureCutscene[i].scene;
                                    FPSaveManager.lastEventScene = __instance.adventureCutscene[i].scene;
                                    flag = true;
                                }
                                else if (__instance.forceCutscene && PlayerHandler.currentCharacter.eventActivatorCharacter == __instance.adventureCutscene[i].character)
                                {
                                    component.sceneToLoad = __instance.adventureCutscene[i].scene;
                                    FPSaveManager.lastEventScene = __instance.adventureCutscene[i].scene;
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                component.SetTransitionColor(0f, 0f, 0f);
                                component.BeginTransition();
                                FPStage.checkpointEnabled = false;
                                FPStage.checkpointPos = new Vector2(0f, 0f);
                                FPSaveManager.SaveToFile(FPSaveManager.fileSlot);
                                FPAudio.PlayMenuSfx(3);
                                ___animationStep++;
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
