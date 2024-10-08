﻿using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Patches
{
    internal class ModdedPotionsFix
    {
        //If a mod introduced new potion IDs and then got removed game will try to render invalid potion sprites within the save menu, resulting in an unhandled null pointer.
        //This Finalizer intercepts this exception, and restores sane defaults which lets the game proceed far enough for its own sanity checks to fire and correct the file.
        [HarmonyFinalizer]
        [HarmonyPatch(typeof(FPSaveManager), "DrawPotion", new Type[] { typeof(FPPowerup[]), typeof(byte[]),
            typeof(SpriteRenderer[]),typeof(SpriteRenderer), typeof(Sprite[]), typeof(Sprite[]),
            typeof(Sprite[]), typeof(Sprite[])})]
        public static Exception PatchDrawPotion(Exception __exception, FPPowerup[] powerups, SpriteRenderer[] potionSlot, Sprite[] spriteBottom, Sprite[] spriteMiddle, Sprite[] spriteTop)
        {
            if (__exception != null)
            {
                Console.WriteLine(__exception.Message);

                int potionSlots = FPSaveManager.GetPotionSlots();
                for (int l = 0; l < potionSlots; l++)
                {
                    if (!powerups.Contains(FPPowerup.BOTTLE_BOOSTER))
                    {
                        potionSlot[l].sprite = null;
                    }
                    else if (l == 0)
                    {
                        potionSlot[l].sprite = spriteBottom[0];
                    }
                    else if (l >= 9)
                    {
                        potionSlot[l].sprite = spriteTop[0];
                    }
                    else
                    {
                        potionSlot[l].sprite = spriteMiddle[0];
                    }
                }
                return null;
            }
            return __exception;
        }
    }
}
