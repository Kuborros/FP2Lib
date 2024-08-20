using HarmonyLib;
using System;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuWorldMap
    {

        private static MenuWorldMap currInstance;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "SetPlayerSprite", MethodType.Normal)]
        private static bool PatchSetPlayerSprite(bool walking, bool ___vehicleMode, ref SpriteRenderer ___playerSpriteRenderer, ref SpriteRenderer ___playerShadowRenderer, FPMap[] ___renderedMap, int ___currentMap, float ___animTimer)
        {
            //Return for non-customs
            if (FPSaveManager.character < (FPCharacterID)5) return true;

            PlayableChara chara = PlayerHandler.currentCharacter;

            if (___vehicleMode)
            {
                ___playerSpriteRenderer.sprite = ___renderedMap[___currentMap].vehicle[1];
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


        internal static void State_WaitForMenu()
        {
            State_WaitForMenu(currInstance);
        }

        [HarmonyReversePatch(0)]
        [HarmonyPatch(typeof(MenuWorldMap), "State_WaitForMenu", MethodType.Normal)]
        public static void State_WaitForMenu(MenuWorldMap instance)
        {
            throw new NotImplementedException("Method failed to reverse patch!");
        }

        //TODO:
        //Fix Battlesphere in story mode being dumb. It sends us to shadow realm instead of progressing story. Force flag maybe?
        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "CutsceneCheck", MethodType.Normal)]
        private static bool PatchCutsceneCheck(MenuWorldMap __instance, ref bool ___cutsceneCheck, ref float ___badgeCheckTimer, ref GameObject ___targetMenu)
        {
            
        }
        */

    }
}