using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Item.Patches
{
    internal class PotionMenuPatches
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuItemSelect), "Start", MethodType.Normal)]
        static void PatchMenuItemSelectStart(MenuItemSelect __instance)
        {
            foreach (ItemData potion in ItemHandler.Items.Values)
            {
                if (!potion.isPotion) continue;
                //Make sure we are not re-adding it!
                if (!__instance.potionList.Contains((FPPowerup)potion.potionID))
                {
                    __instance.potionList = __instance.potionList.AddToArray((FPPowerup)potion.potionID);
                    __instance.potions = __instance.potions.AddToArray(false);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPauseMenu), "Start", MethodType.Normal)]
        static void PatchMenuMultipleStart(ref Sprite[] ___spriteBottom, ref Sprite[] ___spriteMiddle, ref Sprite[] ___spriteTop)
        {
            int totalPotions = ItemHandler.basePotions + ItemHandler.potionCount;
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

            foreach (ItemData potion in ItemHandler.Items.Values)
            {
                if (!potion.isPotion) continue;

                if (potion.spriteBottleTop != null)
                {
                    ___spriteTop[potion.potionID] = potion.spriteBottleTop;
                }
                else ___spriteTop[potion.potionID] = ___spriteTop[1];

                if (potion.spriteBottleMid != null)
                {
                    ___spriteMiddle[potion.potionID] = potion.spriteBottleMid;
                }
                else ___spriteMiddle[potion.potionID] = ___spriteMiddle[1];

                if (potion.spriteBottleBottom != null)
                {
                    ___spriteBottom[potion.potionID] = potion.spriteBottleBottom;
                }
                else ___spriteBottom[potion.potionID] = ___spriteBottom[1];
            }
        }

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuFile), "Start", MethodType.Normal)]
        static void PatchMenuFileStart(ref MenuFilePanel[] ___files)
        {
            int totalPotions = ItemHandler.basePotions + ItemHandler.potionCount;
            foreach (MenuFilePanel file in ___files)
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

                foreach (ItemData potion in ItemHandler.Items.Values)
                {
                    if (!potion.isPotion) continue;

                    if (potion.spriteBottleTop != null)
                    {
                        file.spriteTop[potion.potionID] = potion.spriteBottleTop;
                    }
                    else file.spriteTop[potion.potionID] = file.spriteTop[1];

                    if (potion.spriteBottleMid != null)
                    {
                        file.spriteMiddle[potion.potionID] = potion.spriteBottleMid;
                    }
                    else file.spriteMiddle[potion.potionID] = file.spriteMiddle[1];

                    if (potion.spriteBottleBottom != null)
                    {
                        file.spriteBottom[potion.potionID] = potion.spriteBottleBottom;
                    }
                    else file.spriteBottom[potion.potionID] = file.spriteBottom[1];
                }
            }
        }
    }
}
