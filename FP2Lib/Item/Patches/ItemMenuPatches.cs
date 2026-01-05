using BepInEx.Bootstrap;
using FP2Lib.Tools;
using HarmonyLib;
using System;
using System.Linq;

namespace FP2Lib.Item.Patches
{
    internal class ItemMenuPatches
    {

        [HarmonyPrefix]
        [HarmonyAfter("com.eps.plugin.fp2.potion-seller")]
        [HarmonyPatch(typeof(MenuMain), "Start", MethodType.Normal)]
        static void PotionSellerRadar()
        {
            //Detect if Potion Seller is installed.
            ItemHandler.isPotionSellerInstalled = Chainloader.PluginInfos.ContainsKey("com.eps.plugin.fp2.potion-seller");
        }

        [HarmonyPrefix]
        [HarmonyAfter("com.eps.plugin.fp2.potion-seller")]
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
                //Force early firing of Potion Seller's patch - otherwise it would run *after* we extended the array already, and not add it's own items.
                if (ItemHandler.isPotionSellerInstalled) ___pfPowerupIcon.GetRenderer();

                if (___pfPowerupIcon.digitFrames.Length < totalItems)
                {
                    ___pfPowerupIcon.digitFrames = Utils.ExpandSpriteArray(___pfPowerupIcon.digitFrames, totalItems, ___pfPowerupIcon.digitFrames[1]);
                }
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.sprite != null)
                    {
                        ___pfPowerupIcon.digitFrames[item.itemID] = item.sprite;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyAfter("com.eps.plugin.fp2.potion-seller")]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPauseMenu), "Start", MethodType.Normal)]
        static void PatchMenuMultipleStart(ref FPHudDigit[] ___itemIcon)
        {
            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            foreach (FPHudDigit digit in ___itemIcon)
            {
                digit.GetRenderer();

                if (digit.digitFrames.Length < totalItems)
                {
                    digit.digitFrames = Utils.ExpandSpriteArray(digit.digitFrames, totalItems, digit.digitFrames[1]);
                }
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.sprite != null)
                    {
                        digit.digitFrames[item.itemID] = item.sprite;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyAfter("com.eps.plugin.fp2.potion-seller")]
        [HarmonyPatch(typeof(MenuFile), "Start", MethodType.Normal)]
        static void PatchMenuFileStart(ref FPHudDigit ___pfPowerupIcon)
        {
            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            if (___pfPowerupIcon != null)
            {

                //Force early firing of Potion Seller's patch - otherwise it would run *after* we extended the array already, and not add it's own items.
                if (ItemHandler.isPotionSellerInstalled) ___pfPowerupIcon.GetRenderer();

                if (___pfPowerupIcon.digitFrames.Length < totalItems)
                {
                    ___pfPowerupIcon.digitFrames = Utils.ExpandSpriteArray(___pfPowerupIcon.digitFrames, totalItems, ___pfPowerupIcon.digitFrames[1]);
                }
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.sprite != null)
                    {
                        ___pfPowerupIcon.digitFrames[item.itemID] = item.sprite;
                    }
                }
            }
        }

        //This might need a transpiler for CreateOverviewList() and UpdateItemList() - both use a hardcoded 50 item long array for their list creation code.
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyAfter("com.eps.plugin.fp2.potion-seller")]
        [HarmonyPatch(typeof(MenuGlobalPause), "Start", MethodType.Normal)]
        static void PatchMenuGlobalStart(ref FPHudDigit ___powerupIcon, ref FPPowerup[] ___powerupSortingOrder)
        {
            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            if (___powerupIcon != null)
            {

                //Force early firing of Potion Seller's patch - otherwise it would run *after* we extended the array already, and not add it's own items.
                if (ItemHandler.isPotionSellerInstalled) ___powerupIcon.GetRenderer();

                if (___powerupIcon.digitFrames.Length < totalItems)
                {
                    ___powerupIcon.digitFrames = Utils.ExpandSpriteArray(___powerupIcon.digitFrames, totalItems, ___powerupIcon.digitFrames[1]);
                }
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.sprite != null)
                    {
                        ___powerupIcon.digitFrames[item.itemID] = item.sprite;
                    }
                    //The "?" icon
                    else ___powerupIcon.digitFrames[item.itemID] = ___powerupIcon.digitFrames[1];
                }
            }

            foreach (ItemData item in ItemHandler.Items.Values)
            {
                if (!___powerupSortingOrder.Contains((FPPowerup)item.itemID))
                {
                    ___powerupSortingOrder = ___powerupSortingOrder.AddToArray((FPPowerup)item.itemID);
                }
            }
        }
    }
}
