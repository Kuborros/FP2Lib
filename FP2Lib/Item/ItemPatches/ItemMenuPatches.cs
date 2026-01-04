using FP2Lib.Player;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Item.ItemPatches
{
    internal class ItemMenuPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuItemSelect),"Start", MethodType.Normal)]
        static void PatchMenuItemSelectStart(MenuItemSelect __instance,ref FPHudDigit ___pfPowerupIcon)
        {
            foreach (ItemData item in ItemHandler.Items.Values)
            {
                //Make sure we are not re-adding it!
                if (!__instance.amuletList.Contains((FPPowerup)item.id))
                {
                    __instance.amuletList = __instance.amuletList.AddToArray((FPPowerup)item.id);
                    __instance.amulets = __instance.amulets.AddToArray(false);
                }
            }

            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            if (___pfPowerupIcon != null)
            {
                if (___pfPowerupIcon.digitFrames.Length < totalItems)
                {
                    Array.Resize(ref ___pfPowerupIcon.digitFrames, totalItems);
                }
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.sprite != null)
                    {
                        ___pfPowerupIcon.digitFrames[item.id] = item.sprite;
                    }
                    //The "?" icon
                    else ___pfPowerupIcon.digitFrames[item.id] = ___pfPowerupIcon.digitFrames[1];
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPauseMenu), "Start", MethodType.Normal)]
        static void PatchMenuMultipleStart(ref FPHudDigit[] ___itemIcon)
        {
            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            foreach (FPHudDigit digit in ___itemIcon)
            {
                if (digit.digitFrames.Length < totalItems)
                {
                    Array.Resize(ref digit.digitFrames, totalItems);
                }
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.sprite != null)
                    {
                        digit.digitFrames[item.id] = item.sprite;
                    }
                    //The "?" icon
                    else digit.digitFrames[item.id] = digit.digitFrames[1];
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuFile), "Start", MethodType.Normal)]
        static void PatchMenuFileStart(ref FPHudDigit ___pfPowerupIcon)
        {
            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            if (___pfPowerupIcon != null)
            {
                if (___pfPowerupIcon.digitFrames.Length < totalItems)
                {
                    Array.Resize(ref ___pfPowerupIcon.digitFrames, totalItems);
                }
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.sprite != null)
                    {
                        ___pfPowerupIcon.digitFrames[item.id] = item.sprite;
                    }
                    //The "?" icon
                    else ___pfPowerupIcon.digitFrames[item.id] = ___pfPowerupIcon.digitFrames[1];
                }
            }
        }
    }
}
