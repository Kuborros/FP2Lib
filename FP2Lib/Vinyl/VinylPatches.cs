using BepInEx;
using BepInEx.Logging;
using FP2Lib.Item;
using FP2Lib.Stage;
using HarmonyLib;
using System;
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
            //But if it's too long things will break, so we trim it in such case.
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
        static void PatchMenuJukeBoxStart(ref AudioClip[] ___music, ref int[] ___soundtrackOrder)
        {
            //Load in custom Vinyls and add them each to the track listing.
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
                            //This array also needs to match the length, but it can handle having 0 as location
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
        //Yes i am aware it kinda repeats the same loop when in adventure. But since we check in both places if given Vinyl was already added, it can stay.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "Start", MethodType.Normal)]
        static void PatchClasicMusic(MenuShop __instance, ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts, ref int[] ___starCardRequirements, ref FPMusicTrack[] ___musicID)
        {
            if (___itemCosts != null && ___itemsForSale != null)
            {
                //Are we in Vinyl shop?
                if (___itemsForSale[0] == FPPowerup.NONE)
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
                    SortItems(__instance);
                    UpdateItemList(__instance, true);
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
                if (fPHubNPC != null)
                {
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

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(ItemChest), "Start", MethodType.Normal)]
        static void PatchItemChestStart(ItemChest __instance)
        {
            if (__instance.contents == FPItemChestContent.MUSIC)
            {
                if (FPStage.currentStage != null && __instance.powerupType == FPPowerup.RANDOM)
                {
                    // Stages over 32 are custom stages
                    if (FPStage.currentStage.stageID > 32)
                    {
                        CustomStage stage = StageHandler.getCustomStageByRuntimeId(FPStage.currentStage.stageID);
                        if (stage != null && !stage.vinylUID.IsNullOrWhiteSpace())
                        {
                            VinylData data = VinylHandler.GetVinylDataByUid(stage.vinylUID);
                            if (data != null) __instance.musicID = (FPMusicTrack)data.id;
                        }
                    }
                }
            }
        }


        [HarmonyReversePatch]
        [HarmonyPatch(typeof(MenuShop), "SortItems", MethodType.Normal)]
        public static void SortItems(MenuShop instance)
        {
            // Replaced at runtime with reverse patch
            throw new NotImplementedException("Method failed to reverse patch!");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(MenuShop), "UpdateItemList", MethodType.Normal)]
        public static void UpdateItemList(MenuShop instance, bool updateText)
        {
            // Replaced at runtime with reverse patch
            throw new NotImplementedException("Method failed to reverse patch!");
        }
    }
}
