using HarmonyLib;
using System;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuWorldMap
    {
        private static MenuWorldMap instance;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "SetPlayerSprite", MethodType.Normal)]
        private static bool PatchSetPlayerSprite(bool walking, bool ___vehicleMode, ref SpriteRenderer ___playerSpriteRenderer, ref SpriteRenderer ___playerShadowRenderer, FPMap[] ___renderedMap, int ___currentMap, float ___animTimer)
        {
            //Return for non-customs
            if (FPSaveManager.character < (FPCharacterID)5) return true;

            PlayableChara chara = PlayerHandler.currentCharacter;

            if (___vehicleMode)
            {
                ___playerSpriteRenderer.sprite = ___renderedMap[___currentMap].vehicle[chara.airshipSprite];
                if (___renderedMap[___currentMap].waterVehicle)
                {
                    ___playerShadowRenderer.sprite = null;
                    ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    ___playerShadowRenderer.sprite = ___playerSpriteRenderer.sprite;
                    ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, Mathf.Sin(0.017453292f * FPStage.platformTimer * 2f) * 4f, 0f);
                }
            }
            else if (walking)
            {
                ___playerSpriteRenderer.sprite = chara.worldMapWalk[((int)(___animTimer) % chara.worldMapWalk.Length)];
                ___playerShadowRenderer.sprite = null;
                ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            else
            {
                ___playerSpriteRenderer.sprite = chara.worldMapIdle[Mathf.Min((int)((___animTimer) % 12), chara.worldMapIdle.Length - 1)];
                ___playerShadowRenderer.sprite = null;
                ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
            }

            return false;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuWorldMap), "CutsceneCheck", MethodType.Normal)]
        static void PatchCutsceneCheck(MenuWorldMap __instance, ref float ___badgeCheckTimer, ref GameObject ___targetMenu)
        {
            //Skip doing anything on built-in chars, or if custom char is null
            if (FPSaveManager.character <= FPCharacterID.NEERA || PlayerHandler.currentCharacter == null) return;

            //Unironically just re-run the cutscene check ourselves
            if (__instance.cutscenes.Length > 0)
            {
                for (int i = 0; i < __instance.cutscenes.Length; i++)
                {
                    if (__instance.cutscenes[i].requiredStoryFlags.Length <= 0)
                    {
                        continue;
                    }
                    int num = __instance.cutscenes[i].requiredStoryFlags.Length;
                    for (int j = 0; j < __instance.cutscenes[i].requiredStoryFlags.Length; j++)
                    {
                        if (FPSaveManager.storyFlag[__instance.cutscenes[i].requiredStoryFlags[j]] > 0)
                        {
                            num--;
                        }
                    }
                    for (int k = 0; k < __instance.cutscenes[i].deactivateAtFlag.Length; k++)
                    {
                        if (FPSaveManager.storyFlag[__instance.cutscenes[i].deactivateAtFlag[k]] > 0)
                        {
                            num = 99;
                        }
                    }
                    if (__instance.cutscenes[i].requiredMap >= 0 && FPSaveManager.lastMap != __instance.cutscenes[i].requiredMap)
                    {
                        num = 99;
                    }
                    if (__instance.cutscenes[i].requiredLocation >= 0 && FPSaveManager.lastMapLocation != __instance.cutscenes[i].requiredLocation)
                    {
                        num = 99;
                    }
                    bool flag = false;
                    for (int l = 0; l < __instance.cutscenes[i].dialogSequence.Length; l++)
                    {
                        //If using own cutscene activators, check if the mod added its own scene
                        if (PlayerHandler.currentCharacter.useOwnCutsceneActivators)
                        {
                            if (__instance.cutscenes[i].dialogSequence[l].characters.Length > PlayerHandler.currentCharacter.id)
                            {
                                if (__instance.cutscenes[i].dialogSequence[l].characters[0])
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (__instance.cutscenes[i].dialogSequence[l].characters[(int)PlayerHandler.currentCharacter.eventActivatorCharacter])
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        num = 99;
                    }
                    if (num <= 0)
                    {
                        CutsceneDialog cutsceneDialog = GameObject.Instantiate(__instance.menuCutscene);
                        cutsceneDialog.currentScene = __instance.cutscenes[i].sceneID;
                        cutsceneDialog.dialogSystem = __instance.dialogSystem;
                        cutsceneDialog.dialogSequence = __instance.cutscenes[i].dialogSequence;
                        ___targetMenu = cutsceneDialog.gameObject;
                        instance = __instance;
                        __instance.state = State_WaitForMenu;
                    }
                    else
                    {
                        ___badgeCheckTimer = 1f;
                    }
                }
            }
        }
        public static void State_WaitForMenu()
        {
            State_WaitForMenu(instance);
        }

        [HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
        [HarmonyPatch(typeof(MenuWorldMap), "State_WaitForMenu", MethodType.Normal)]
        public static void State_WaitForMenu(object instance)
        {
            // Replaced at runtime with reverse patch
            throw new NotImplementedException("Method failed to reverse patch!");
        }
    }
}