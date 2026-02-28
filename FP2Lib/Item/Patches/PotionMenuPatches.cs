using BepInEx.Logging;
using FP2Lib.Tools;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Item.Patches
{
    internal class PotionMenuPatches
    {
        //Special magical failsafe
        //This only bumps up how far we extend the sprite arrays, in case the file has some potion with unnaturaly high id
        static int highestEncounteredPotionID = 0;

        private static readonly ManualLogSource PotionLogSource = FP2Lib.logSource;

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
        [HarmonyAfter("com.eps.plugin.fp2.potion-seller")]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start", MethodType.Normal)]
        [HarmonyPatch(typeof(MenuItemSelect), "Start", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPauseMenu), "Start", MethodType.Normal)]
        static void PatchMenuMultipleStart(ref Sprite[] ___spriteBottom, ref Sprite[] ___spriteMiddle, ref Sprite[] ___spriteTop)
        {

            int totalPotions = ItemHandler.basePotions + ItemHandler.potionCount;
            //In case we ran into a potion that had a higher ID than the one we have registered
            if (totalPotions > highestEncounteredPotionID) highestEncounteredPotionID = totalPotions;
            else totalPotions = highestEncounteredPotionID;

            if (___spriteTop.Length < totalPotions)
            {
                ___spriteTop = Utils.ExpandSpriteArray(___spriteTop, totalPotions, ___spriteTop[1]);
            }

            if (___spriteMiddle.Length < totalPotions)
            {
                ___spriteMiddle = Utils.ExpandSpriteArray(___spriteMiddle, totalPotions, ___spriteMiddle[1]);
            }

            if (___spriteBottom.Length < totalPotions)
            {
                ___spriteBottom = Utils.ExpandSpriteArray(___spriteBottom, totalPotions, ___spriteBottom[1]);
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
        [HarmonyAfter("com.eps.plugin.fp2.potion-seller")]
        [HarmonyPatch(typeof(MenuFile), "Start", MethodType.Normal)]
        static void PatchMenuFileStart(ref MenuFilePanel[] ___files)
        {
            int totalPotions = ItemHandler.basePotions + ItemHandler.potionCount;
            //In case there was a higher potion ID somewhere along the line
            if (totalPotions > highestEncounteredPotionID) highestEncounteredPotionID = totalPotions;
            else totalPotions = highestEncounteredPotionID;

            foreach (MenuFilePanel file in ___files)
            {

                if (file.spriteTop.Length < totalPotions)
                {
                    file.spriteTop = Utils.ExpandSpriteArray(file.spriteTop, totalPotions, file.spriteTop[1]);
                }

                if (file.spriteMiddle.Length < totalPotions)
                {
                    file.spriteMiddle = Utils.ExpandSpriteArray(file.spriteMiddle, totalPotions, file.spriteMiddle[1]);
                }

                if (file.spriteBottom.Length < totalPotions)
                {
                    file.spriteBottom = Utils.ExpandSpriteArray(file.spriteBottom, totalPotions, file.spriteBottom[1]);
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

        //If a mod introduced new potion IDs outside ranges known to us and then got removed game will try to render invalid potion sprites within the save menu, resulting in an unhandled null pointer.
        //This Finalizer intercepts this exception, and restores sane defaults which lets the game proceed far enough for its own sanity checks to fire and correct the file.
        [HarmonyFinalizer]
        [HarmonyPatch(typeof(FPSaveManager), "DrawPotion", new Type[] { typeof(FPPowerup[]), typeof(byte[]),
            typeof(SpriteRenderer[]),typeof(SpriteRenderer), typeof(Sprite[]), typeof(Sprite[]),
            typeof(Sprite[]), typeof(Sprite[])})]
        public static Exception PatchDrawPotion(Exception __exception, FPPowerup[] powerups, SpriteRenderer[] potionSlot, byte[] activePotions, Sprite[] spriteBottom, Sprite[] spriteMiddle, Sprite[] spriteTop)
        {
            if (__exception != null)
            {
                PotionLogSource.LogWarning("Attempted to render a potion with out-of-bounds sprite! Bad!" +
                    "\nHighest encountered potion ID:" + activePotions.Length + "\nWe have expected the highest one to be: " + highestEncounteredPotionID +
                    "\nIncreasing value accordingly, applying fix!");
                highestEncounteredPotionID = activePotions.Length + 1;
                foreach (SpriteRenderer renderer in potionSlot)
                {
                    renderer.sprite = null;
                }
                return null;
            }
            return __exception;
        }
    }
}
