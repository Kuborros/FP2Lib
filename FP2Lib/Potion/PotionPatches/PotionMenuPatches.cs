using FP2Lib.Item;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Potion.PotionPatches
{
    internal class PotionMenuPatches
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuItemSelect), "Start", MethodType.Normal)]
        static void PatchMenuItemSelectStart(MenuItemSelect __instance, ref FPHudDigit ___pfPowerupIcon)
        {
            foreach (PotionData potion in PotionHandler.Potions.Values)
            {
                //Make sure we are not re-adding it!
                if (!__instance.potionList.Contains((FPPowerup)potion.id))
                {
                    __instance.amuletList = __instance.amuletList.AddToArray((FPPowerup)potion.id);
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
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPauseMenu), "Start", MethodType.Normal)]
        static void PatchMenuMultipleStart(ref Sprite[] ___spriteBottom, ref Sprite[] ___spriteMiddle, ref Sprite[] ___spriteTop)
        {
            int totalPotions = PotionHandler.basePotions + PotionHandler.Potions.Count;
            if (___spriteTop.Length < totalPotions)
            {
                Array.Resize(ref ___spriteTop, totalPotions);
            }

            if (___spriteMiddle.Length < totalPotions)
            {
                Array.Resize(ref ___spriteMiddle, totalPotions);
            }

            if (___spriteBottom.Length < totalPotions)
            {
                Array.Resize(ref ___spriteBottom, totalPotions);
            }

            foreach (PotionData potion in PotionHandler.Potions.Values)
            {
                if (potion.spriteBottleTop != null)
                {
                    ___spriteTop[potion.id] = potion.spriteBottleTop;
                }
                else ___spriteTop[potion.id] = ___spriteTop[1];

                if (potion.spriteBottleMid != null)
                {
                    ___spriteMiddle[potion.id] = potion.spriteBottleMid;
                }
                else ___spriteMiddle[potion.id] = ___spriteMiddle[1];

                if (potion.spriteBottleBottom != null)
                {
                    ___spriteBottom[potion.id] = potion.spriteBottleBottom;
                }
                else ___spriteBottom[potion.id] = ___spriteBottom[1];
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuFile), "Start", MethodType.Normal)]
        static void PatchMenuFileStart(ref MenuFilePanel[] ___file)
        {
            int totalPotions = PotionHandler.basePotions + PotionHandler.Potions.Count;
            foreach (MenuFilePanel file in ___file)
            {
                if (file.spriteTop.Length < totalPotions)
                {
                    Array.Resize(ref file.spriteTop, totalPotions);
                }

                if (file.spriteMiddle.Length < totalPotions)
                {
                    Array.Resize(ref file.spriteMiddle, totalPotions);
                }

                if (file.spriteBottom.Length < totalPotions)
                {
                    Array.Resize(ref file.spriteBottom, totalPotions);
                }

                foreach (PotionData potion in PotionHandler.Potions.Values)
                {
                    if (potion.spriteBottleTop != null)
                    {
                        file.spriteTop[potion.id] = potion.spriteBottleTop;
                    }
                    else file.spriteTop[potion.id] = file.spriteTop[1];

                    if (potion.spriteBottleMid != null)
                    {
                        file.spriteMiddle[potion.id] = potion.spriteBottleMid;
                    }
                    else file.spriteMiddle[potion.id] = file.spriteMiddle[1];

                    if (potion.spriteBottleBottom != null)
                    {
                        file.spriteBottom[potion.id] = potion.spriteBottleBottom;
                    }
                    else file.spriteBottom[potion.id] = file.spriteBottom[1];
                }
            }
        }
    }
}
