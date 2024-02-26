using HarmonyLib;
using System;
using UnityEngine;

namespace FP2Lib.Patches
{
    internal class PotionSizeFix
    {
        //A bug in base game makes potionCapacityUpgradeLevel behave incorrectly when set to 2. This restores the intended behaviour.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager),"GetPotionSlots", new Type[] { typeof(int) })]
        public static bool PatchGetPotionSlots(ref int __result, ref int potionCapacityUpgradeLevel)
        {
            potionCapacityUpgradeLevel = Mathf.Clamp(potionCapacityUpgradeLevel, 0, FPSaveManager.EXPANSION_LIMIT_POTION_SLOTS);
            if (potionCapacityUpgradeLevel >= 2)
            {
                __result = FPSaveManager.DEFAULT_POTION_SLOTS + FPSaveManager.EXTRA_POTION_SLOTS_LEVEL2;
                return false;
            }
            return true;
        }
    }
}
