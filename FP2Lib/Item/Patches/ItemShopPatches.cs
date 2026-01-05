using HarmonyLib;
using System;
using System.Linq;

namespace FP2Lib.Item.Patches
{
    internal class ItemShopPatches
    {
        //Patch the NPCs
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPHubNPC), "Start", MethodType.Normal)]
        static void PatchHubShopkeep(string ___NPCName, ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts, ref int[] ___starCardRequirements, ref FPMusicTrack[] ___musicID)
        {
            if ((___NPCName == "Blake" || ___NPCName == "Yuni" || ___NPCName == "Florin" || ___NPCName == "Chloe" || ___NPCName == "Milla") && ___itemCosts != null && ___itemsForSale != null)
            {
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    //Potions
                    if (item.isPotion)
                    {
                        if (item.potionShopLocation == PAddToShop.Milla && ___NPCName == "Milla" && !___itemsForSale.Contains((FPPowerup)item.itemID))
                        {
                            ___itemsForSale = ___itemsForSale.AddToArray((FPPowerup)item.itemID);
                            ___itemCosts = ___itemCosts.AddToArray(item.goldGemsPrice);
                            ___starCardRequirements = ___starCardRequirements.AddToArray(item.starCards);
                            ___musicID = ___musicID.AddToArray(FPMusicTrack.NONE);
                        }
                    }
                    //Normal items
                    else if (item.itemShopLocation != IAddToShop.None || item.itemShopLocation != IAddToShop.ClassicOnly)
                    {
                        if (item.itemShopLocation == IAddToShop.Blake && ___NPCName == "Blake"
                        || item.itemShopLocation == IAddToShop.Yuni && ___NPCName == "Yuni"
                        || item.itemShopLocation == IAddToShop.Florin && ___NPCName == "Florin"
                        || item.itemShopLocation == IAddToShop.Chloe && ___NPCName == "Chloe")
                        {
                            if (!___itemsForSale.Contains((FPPowerup)item.itemID))
                            {
                                ___itemsForSale = ___itemsForSale.AddToArray((FPPowerup)item.itemID);
                                ___itemCosts = ___itemCosts.AddToArray(item.goldGemsPrice);
                                ___starCardRequirements = ___starCardRequirements.AddToArray(item.starCards);
                                ___musicID = ___musicID.AddToArray(FPMusicTrack.NONE);
                            }
                        }
                    }
                }
            }
        }

        //Patching the shop
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuShop), "Start", MethodType.Normal)]
        static void PatchClasicItems(MenuShop __instance, ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts, ref int[] ___starCardRequirements, ref FPMusicTrack[] ___musicID, ref FPHudDigit[] ___powerups)
        {
            if (___itemCosts != null && ___itemsForSale != null)
            {
                //Are we in Item shop?
                if (___itemsForSale[0] != FPPowerup.NONE)
                {
                    if (ItemHandler.Items.Count > 0)
                    {
                        int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
                        foreach (FPHudDigit digit in ___powerups)
                        {
                            if (digit.digitFrames.Length < totalItems)
                            {
                                Array.Resize(ref digit.digitFrames, totalItems);
                            }
                            foreach (ItemData item in ItemHandler.Items.Values)
                            {
                                if (item.sprite != null)
                                {
                                    digit.digitFrames[item.itemID] = item.sprite;
                                }
                                //The "?" icon
                                else digit.digitFrames[item.itemID] = digit.digitFrames[1];
                            }
                        }

                        foreach (ItemData item in ItemHandler.Items.Values)
                        {
                            //Add both items and potions, since it's the classic mode shop.
                            if (item.itemShopLocation != IAddToShop.None && FPSaveManager.gameMode == FPGameMode.CLASSIC)
                            {
                                if (!___itemsForSale.Contains((FPPowerup)item.itemID))
                                {
                                    ___itemsForSale = ___itemsForSale.AddToArray((FPPowerup)item.itemID);
                                    ___itemCosts = ___itemCosts.AddToArray(item.goldGemsPrice);
                                    ___starCardRequirements = ___starCardRequirements.AddToArray(item.starCards);
                                    ___musicID = ___musicID.AddToArray(FPMusicTrack.NONE);
                                }
                            }
                        }
                        SortItems(__instance);
                        UpdateItemList(__instance, true);
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
                if (fPHubNPC != null)
                {
                    if ((fPHubNPC.name == "Blake" || fPHubNPC.name == "Yuni" || fPHubNPC.name == "Florin" || fPHubNPC.name == "Chloe" || fPHubNPC.name == "Milla") && fPHubNPC.itemCosts != null && fPHubNPC.itemsForSale != null)
                    {
                        foreach (ItemData item in ItemHandler.Items.Values)
                        {
                            //Potions
                            if (item.isPotion)
                            {
                                if (item.potionShopLocation == PAddToShop.Milla && fPHubNPC.name == "Milla" && !fPHubNPC.itemsForSale.Contains((FPPowerup)item.itemID))
                                {
                                    fPHubNPC.itemsForSale = fPHubNPC.itemsForSale.AddToArray((FPPowerup)item.itemID);
                                    fPHubNPC.itemCosts = fPHubNPC.itemCosts.AddToArray(item.goldGemsPrice);
                                    fPHubNPC.starCardRequirements = fPHubNPC.starCardRequirements.AddToArray(item.starCards);
                                    fPHubNPC.musicID = fPHubNPC.musicID.AddToArray(FPMusicTrack.NONE);
                                }
                            }
                            //Normal items
                            else if (item.itemShopLocation != IAddToShop.None || item.itemShopLocation != IAddToShop.ClassicOnly)
                            {
                                if (item.itemShopLocation == IAddToShop.Blake && fPHubNPC.name == "Blake"
                                || item.itemShopLocation == IAddToShop.Yuni && fPHubNPC.name == "Yuni"
                                || item.itemShopLocation == IAddToShop.Florin && fPHubNPC.name == "Florin"
                                || item.itemShopLocation == IAddToShop.Chloe && fPHubNPC.name == "Chloe")
                                {
                                    if (!fPHubNPC.itemsForSale.Contains((FPPowerup)item.itemID))
                                    {
                                        fPHubNPC.itemsForSale = fPHubNPC.itemsForSale.AddToArray((FPPowerup)item.itemID);
                                        fPHubNPC.itemCosts = fPHubNPC.itemCosts.AddToArray(item.goldGemsPrice);
                                        fPHubNPC.starCardRequirements = fPHubNPC.starCardRequirements.AddToArray(item.starCards);
                                        fPHubNPC.musicID = fPHubNPC.musicID.AddToArray(FPMusicTrack.NONE);
                                    }
                                }
                            }
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
