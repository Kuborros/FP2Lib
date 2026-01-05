using HarmonyLib;
using System;
using System.Linq;

namespace FP2Lib.Item.Patches
{
    internal class ItemMenuPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuItemSelect),"Start", MethodType.Normal)]
        static void PatchMenuItemSelectStart(MenuItemSelect __instance,ref FPHudDigit ___pfPowerupIcon)
        {
            foreach (ItemData item in ItemHandler.Items.Values)
            {
                //Skip if the item is only a potion placeholder.
                if (item.isPotion) continue;
                //Make sure we are not re-adding it!
                if (!__instance.amuletList.Contains((FPPowerup)item.itemID))
                {
                    __instance.amuletList = __instance.amuletList.AddToArray((FPPowerup)item.itemID);
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
                        ___pfPowerupIcon.digitFrames[item.itemID] = item.sprite;
                    }
                    //The "?" icon
                    else ___pfPowerupIcon.digitFrames[item.itemID] = ___pfPowerupIcon.digitFrames[1];
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
                        digit.digitFrames[item.itemID] = item.sprite;
                    }
                    //The "?" icon
                    else digit.digitFrames[item.itemID] = digit.digitFrames[1];
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
                        ___pfPowerupIcon.digitFrames[item.itemID] = item.sprite;
                    }
                    //The "?" icon
                    else ___pfPowerupIcon.digitFrames[item.itemID] = ___pfPowerupIcon.digitFrames[1];
                }
            }
        }
    }
}
