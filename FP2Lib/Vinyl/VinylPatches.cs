using BepInEx;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Vinyl
{
    internal class VinylPatches
    {

        //Update save file to account for extra Vinyls
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad(ref bool[] ___musicTracks)
        {
            //Add slots in file for extra tracks.
            ___musicTracks = FPSaveManager.ExpandBoolArray(___musicTracks, VinylHandler.totalTracks +1);
            //But if it's too ling things will break, so we trim it in such case.
            if (___musicTracks.Length > VinylHandler.totalTracks)
                ___musicTracks = ___musicTracks.Take(VinylHandler.totalTracks +1).ToArray();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetMusicTrackName", MethodType.Normal)]
        static void PatchFPSaveManagerTrackName(int id, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace())
            {
                foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
                {
                    if (vinyl.id == id) { 
                        __result = vinyl.name;
                        if (vinyl.audioClip == null) __result = "Deleted Mod Track!";
                        break; 
                    }
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuJukebox), "Start", MethodType.Normal)]
        static void PatchMenuJukeBoxStart(ref MenuJukebox __instance,ref AudioClip[] ___music,ref int[] ___soundtrackOrder,ref  FPHudDigit[] ___trackIcon)
        {
            //Load in custom Vinyls and add them each to the track listing
            foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
            {
                ___music = ___music.AddItem(vinyl.audioClip).ToArray();
                ___soundtrackOrder = ___soundtrackOrder.AddItem(vinyl.id).ToArray();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPHubNPC), "Start", MethodType.Normal)]
        static void PatchHubShopkeep(string ___NPCName, ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts, ref int[] ___starCardRequirements, ref FPMusicTrack[] ___musicID)
        {
            if ((___NPCName == "Naomi" || ___NPCName == "Digo" || ___NPCName == "Fawnstar") && ___itemCosts != null && ___itemsForSale != null)
            {
                    foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
                    {
                        if (vinyl.shopLocation != VAddToShop.None || vinyl.shopLocation != VAddToShop.ClassicOnly)
                        {
                            if ((vinyl.shopLocation == VAddToShop.Naomi && ___NPCName == "Naomi")
                            || (vinyl.shopLocation == VAddToShop.Digo && ___NPCName == "Digo")
                            || (vinyl.shopLocation == VAddToShop.Fawnstar && ___NPCName == "Fawnstar"))
                            {
                                if (!___musicID.Contains((FPMusicTrack)vinyl.id))
                                {
                                    ___itemsForSale = ___itemsForSale.AddToArray(FPPowerup.NONE);
                                    ___itemCosts = ___itemCosts.AddToArray(300);
                                    ___starCardRequirements = ___starCardRequirements.AddToArray(0);
                                    ___musicID = ___musicID.AddToArray((FPMusicTrack)vinyl.id);
                                }
                            }
                        }
                    }       
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "Start", MethodType.Normal)]
        static void PatchClasicMusic(ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts, ref int[] ___starCardRequirements, ref FPMusicTrack[] ___musicID)
        {
            if (___itemCosts != null && ___itemsForSale != null)
            {
                foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
                {
                    if (vinyl.shopLocation != VAddToShop.None && FPSaveManager.gameMode == FPGameMode.CLASSIC)
                    {
                        if (!___musicID.Contains((FPMusicTrack)vinyl.id))
                            {
                                ___itemsForSale = ___itemsForSale.AddToArray(FPPowerup.NONE);
                                ___itemCosts = ___itemCosts.AddToArray(300);
                                ___starCardRequirements = ___starCardRequirements.AddToArray(0);
                                ___musicID = ___musicID.AddToArray((FPMusicTrack)vinyl.id);
                            }
                        }
                    }
                }
            }
    }
}
