using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Badge
{
    internal class BadgePatches
    {
        private static readonly ManualLogSource BadgeLogSource = FP2Lib.logSource;

        //The save manager allocates 99 badge slots by default.
        //We set it to 255, as there are no side-effects.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad(ref byte[] ___badges)
        {
            if (BadgeHandler.takenIDs.Length > FPSaveManager.badges.Length)
            {
                //There should be no need to trim it, as base game already has much more slots than badges.
                ___badges = FPSaveManager.ExpandByteArray(___badges, BadgeHandler.takenIDs.Length);
            }
        }

        //Both can be patched at once - we add the same things.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "Start", MethodType.Normal)]
        [HarmonyPatch(typeof(MenuGallery), "Start", MethodType.Normal)]
        static void PatchMenuGlobalPause(ref Sprite[] ___badgeSprites, ref int[] ___badgeOrder, Sprite[] ___badgeBlankSprites)
        {
            foreach (BadgeData badge in BadgeHandler.Badges.Values)
            {
                if (badge.id != 0 && !___badgeSprites.Contains(badge.sprite))
                {
                    //Extend the array accordingly for a badge with existing ID. Fill free spaces with default sprite.
                    //Badge menu handles gaps properly, as long as their order placement is set to 0.
                    if (badge.id >= ___badgeSprites.Length)
                    {
                        for (int i = ___badgeSprites.Length; i <= (badge.id); i++)
                            ___badgeSprites = ___badgeSprites.AddToArray(___badgeBlankSprites[0]);
                    }
                    //Add the badge at it's place.
                    if (badge.sprite != null) ___badgeSprites[badge.id] = badge.sprite;
                    ___badgeOrder = ___badgeOrder.AddItem(badge.id).ToArray();
                }
            }
        }

        //Used specifically for the "Badge Unlocked!" sliding message.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BadgeMessage), "Start", MethodType.Normal)]
        static void PatchBadgeMessage(ref Sprite[] ___badgeSprites)
        {
            foreach (BadgeData badge in BadgeHandler.Badges.Values)
            {
                if (badge.id >= ___badgeSprites.Length)
                {
                    for (int i = ___badgeSprites.Length; i <= (badge.id); i++)
                        ___badgeSprites = ___badgeSprites.AddToArray(null); //Null sprite will display nothing without causing issues.
                }
                //Add the badge at it's place.
                ___badgeSprites[badge.id] = badge.sprite;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetBadgeName", MethodType.Normal)]
        static void PatchSaveManagerName(int id, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace())
            {
                foreach (BadgeData badge in BadgeHandler.Badges.Values)
                {
                    if (badge.id == id)
                    {
                        if (badge.name.IsNullOrWhiteSpace()) __result = "A missing badge!";
                        else __result = badge.name;
                    }
                }

            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetBadgeDescription", MethodType.Normal)]
        static void PatchSaveManagerDescription(int id, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace())
            {
                foreach (BadgeData badge in BadgeHandler.Badges.Values)
                {
                    if (badge.id == id)
                    {
                        if (badge.description.IsNullOrWhiteSpace()) __result = "Badge from deleted Mod!";
                        else if (badge.sprite != null) __result = badge.description;
                        else __result = badge.description + " (Deleted Mod)";
                    }
                }
            }
        }


        //0 = Gold
        //1 = Silver
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetBadgeCategory", MethodType.Normal)]
        static void PatchSaveManagerCategory(int id, ref int __result)
        {
            if (__result == 1)
            {
                foreach (BadgeData badge in BadgeHandler.Badges.Values)
                {
                    if (badge.id == id) __result = (int)badge.type;
                }
            }
        }

        //Hides description and name if 1
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetBadgeVisibility", MethodType.Normal)]
        static void PatchSaveManagerVisibility(int id, ref int __result)
        {
            if (__result == 0)
            {
                foreach (BadgeData badge in BadgeHandler.Badges.Values)
                {
                    if (badge.id == id) __result = (int)badge.visibility;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager), "BadgeOnlineSync", MethodType.Normal)]
        static bool PatchBadgeOnlineSync()
        {
            //Skip sending badges to Steam if we detect modded ones.
            if (BadgeHandler.Badges.Values.Count > 0)
            {
                BadgeLogSource.LogInfo("Mod badges installed, skipping Steam online sync!");
                //Skip execution of the original method, since we would send dumb badge IDs to Steam.
                return false;
            }
            else
                return true;
        }
    }
}
