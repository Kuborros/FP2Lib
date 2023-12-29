using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Patches
{
    internal class ModdedPotionsFix
    {
        [HarmonyFinalizer]
        [HarmonyPatch(typeof(FPSaveManager), "DrawPotion", new Type[] { typeof(FPPowerup[]), typeof(byte[]),
            typeof(SpriteRenderer[]),typeof(SpriteRenderer), typeof(Sprite[]), typeof(Sprite[]),
            typeof(Sprite[]), typeof(Sprite[])})]
        public static Exception PatchDrawPotion(Exception __exception, FPSaveManager __instance,
            FPPowerup[] powerups, ref byte[] activePotions, SpriteRenderer[] potionSlot,
            SpriteRenderer bottleRenderer, Sprite[] spriteBottle, Sprite[] spriteBottom,
            Sprite[] spriteMiddle, Sprite[] spriteTop)
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
