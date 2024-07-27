using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuWorldMap
    {

        private static Sprite[] spadeIdle;
        private static Sprite[] spadeWalk;
        private static MenuWorldMap currInstance;

        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "Start", MethodType.Normal)]
        private static void PatchMenuWorldMapStart()
        {
           
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "SetPlayerSprite", MethodType.Normal)]
        private static bool PatchSetPlayerSprite(bool walking, bool ___vehicleMode, ref SpriteRenderer ___playerSpriteRenderer, ref SpriteRenderer ___playerShadowRenderer, FPMap[] ___renderedMap, int ___currentMap, float ___animTimer)
        {
            
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "CutsceneCheck", MethodType.Normal)]
        private static bool PatchCutsceneCheck(MenuWorldMap __instance, ref bool ___cutsceneCheck, ref float ___badgeCheckTimer, ref GameObject ___targetMenu)
        {
            
        }
        */

    }
}