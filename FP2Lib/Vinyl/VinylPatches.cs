using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Vinyl
{
    internal class VinylPatches
    {

        private static readonly ManualLogSource VinylLogSource = FP2Lib.logSource;

        //Update save file to account for extra Vinyls
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad(ref bool[] ___musicTracks)
        {
            //Calculate how many tracks we have
            int totalTracks = VinylHandler.baseTracks + VinylHandler.Vinyls.Count;
            foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
            {
                //Highest ID is the track number
                if (vinyl.id > totalTracks)
                {
                    totalTracks = vinyl.id;
                    VinylLogSource.LogDebug("Detected gap in Vinyl ids!");
                }
            }
            //Add slots in file for extra tracks.
            ___musicTracks = FPSaveManager.ExpandBoolArray(___musicTracks, totalTracks);
            //But if it's too ling things will break, so we trim it in such case.
            //Very important as the game _will_ detonate if you scroll to a vinyl you have 'unlocked' but it has no data for.
            if (___musicTracks.Length > totalTracks - 1)
                ___musicTracks = ___musicTracks.Take(totalTracks).ToArray();

            VinylHandler.WriteToStorage();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetMusicTrackName", MethodType.Normal)]
        static void PatchFPSaveManagerTrackName(int id, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace()) //Default method returned nothing, check if maybe we got the track
            {
                foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
                {
                    if (vinyl.id == id)
                    {
                        if (vinyl.name.IsNullOrWhiteSpace()) __result = "Deleted Mod Track!"; //Deleted vinyl with no data somehow (should not render, but adding anyways)
                        else if (vinyl.audioClip == null) __result = vinyl.name + " - (Deleted Mod)"; //Deleted vinyl without audio
                        else __result = vinyl.name;
                        
                        break; 
                    }
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuJukebox), "Start", MethodType.Normal)]
        static void PatchMenuJukeBoxStart(ref MenuJukebox __instance,ref AudioClip[] ___music,ref int[] ___soundtrackOrder,ref  FPHudDigit[] ___trackIcon)
        {
            //Load in custom Vinyls and add them each to the track listing.
            //Loop 1 - Vinyls with IDs
            foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
            {
                if (vinyl.id != 0)
                {
                    //If the Vinyl ID is higher than the current music array size, extend it and fill the empty spaces.
                    if (vinyl.id >= ___music.Length)
                    {
                        for (int i = ___music.Length; i <= (vinyl.id); i++)
                            ___music = ___music.AddToArray(null);
                    }
                    if (vinyl.id >= ___soundtrackOrder.Length)
                    {
                        for (int i = ___soundtrackOrder.Length; i <= (vinyl.id); i++)
                            //This array also needs to match the lenght, but it can handle having 0 as location
                            ___soundtrackOrder = ___soundtrackOrder.AddToArray(0);
                    }
                    //Add the track at it's place.
                    ___music[vinyl.id] = vinyl.audioClip;
                    //Add vinyl into playback order
                    ___soundtrackOrder[vinyl.id] = vinyl.id;
                }
            }
            VinylHandler.WriteToStorage();
        }

        //Patch the NPCs
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
                            || (vinyl.shopLocation == VAddToShop.Fawnstar && ___NPCName == "Fawnstar")
                            || vinyl.shopLocation == VAddToShop.All)
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


        //Patching the classic mode shop
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
                                ___itemCosts = ___itemCosts.AddToArray(vinyl.crystalsPrice);
                                ___starCardRequirements = ___starCardRequirements.AddToArray(vinyl.starCards);
                                ___musicID = ___musicID.AddToArray((FPMusicTrack)vinyl.id);
                        }
                    }
                }
            }
        }

        //Patching the quickshop
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuClassicShopHub), "Start", MethodType.Normal)]
        static void PatchMenuClassicShopHub(ref FPHubNPC[] ___shopkeepers)
        {
            foreach (FPHubNPC fPHubNPC in ___shopkeepers)
            {
                if (fPHubNPC != null) {
                    if ((fPHubNPC.NPCName == "Naomi" || fPHubNPC.NPCName == "Digo" || fPHubNPC.NPCName == "Fawnstar") && fPHubNPC.itemCosts != null && fPHubNPC.itemsForSale != null)
                    {
                        foreach (VinylData vinyl in VinylHandler.Vinyls.Values)
                        {
                            if (vinyl.shopLocation != VAddToShop.None || vinyl.shopLocation != VAddToShop.ClassicOnly)
                            {
                                if ((vinyl.shopLocation == VAddToShop.Naomi && fPHubNPC.NPCName == "Naomi")
                                || (vinyl.shopLocation == VAddToShop.Digo && fPHubNPC.NPCName == "Digo")
                                || (vinyl.shopLocation == VAddToShop.Fawnstar && fPHubNPC.NPCName == "Fawnstar")
                                || vinyl.shopLocation == VAddToShop.All)
                                {
                                    if (!fPHubNPC.musicID.Contains((FPMusicTrack)vinyl.id))
                                    {
                                        fPHubNPC.itemsForSale = fPHubNPC.itemsForSale.AddToArray(FPPowerup.NONE);
                                        fPHubNPC.itemCosts = fPHubNPC.itemCosts.AddToArray(vinyl.crystalsPrice);
                                        fPHubNPC.starCardRequirements = fPHubNPC.starCardRequirements.AddToArray(vinyl.starCards);
                                        fPHubNPC.musicID = fPHubNPC.musicID.AddToArray((FPMusicTrack)vinyl.id);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
