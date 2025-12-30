using BepInEx;
using BepInEx.Logging;
using FP2Lib.Player;
using HarmonyLib;
using System.Linq;

namespace FP2Lib.Item
{
    internal class ItemPatches
    {
        private static readonly ManualLogSource ItemLogSource = FP2Lib.logSource;

        //Update save file to account for extra Items
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad(ref byte[] ___inventory)
        {
            //Calculate how many items we have
            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            foreach (ItemData item in ItemHandler.Items.Values)
            {
                //Highest ID is the item number
                if (item.id > totalItems)
                {
                    totalItems = item.id;
                }
            }
            //Add slots in file for extra items. 
            //Potion seller already does 99, so we should start at 100 to not mess with it
            ___inventory = FPSaveManager.ExpandByteArray(___inventory, totalItems);

            ItemHandler.WriteToStorage();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemName", MethodType.Normal)]
        static void PatchFPSaveManagerItemName(FPPowerup item, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace()) //Default method returned nothing, check if maybe we got the item
            {
                foreach (ItemData itemData in ItemHandler.Items.Values)
                {
                    if (itemData.id == (int)item)
                    {
                        if (itemData.name.IsNullOrWhiteSpace()) __result = "Deleted Mod Item!";
                        else if (itemData.sprite == null) __result = itemData.name + " - (Deleted Mod)"; 
                        else __result = itemData.name;
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemDescription", MethodType.Normal)]
        static void PatchFPSaveManagerItemDescription(FPPowerup item, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace()) //Default method returned nothing, check if maybe we got the item
            {
                foreach (ItemData itemData in ItemHandler.Items.Values)
                {
                    if (itemData.id == (int)item)
                    {
                        switch(FPSaveManager.character)
                        {
                            case FPCharacterID.LILAC:
                                if (!itemData.descriptionLilac.IsNullOrWhiteSpace()) __result = itemData.descriptionLilac;
                                break;
                            case FPCharacterID.CAROL:
                            case FPCharacterID.BIKECAROL:
                                if (!itemData.descriptionCarol.IsNullOrWhiteSpace()) __result = itemData.descriptionCarol;
                                break;
                            case FPCharacterID.MILLA:
                                if (!itemData.descriptionMilla.IsNullOrWhiteSpace()) __result = itemData.descriptionMilla;
                                break;
                            case FPCharacterID.NEERA:
                                if (!itemData.descriptionNeera.IsNullOrWhiteSpace()) __result = itemData.descriptionNeera;
                                break;
                            default:
                                if (FPSaveManager.character > FPCharacterID.NEERA) {
                                    if (PlayerHandler.currentCharacter != null)
                                    {
                                        if (itemData.descriptionCustom.ContainsKey(PlayerHandler.currentCharacter.uid))
                                            __result = itemData.descriptionCustom[PlayerHandler.currentCharacter.uid];
                                        else __result = itemData.descriptionGeneric;
                                    }
                                    else __result = itemData.descriptionGeneric;
                                }
                                else __result = itemData.descriptionGeneric;
                                break;
                        }
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemDescription", MethodType.Normal)]
        static void PatchFPSaveManagerItemDescription(FPPowerup item, ref float __result)
        {
            if (__result == 0f)
            {
                foreach (ItemData itemData in ItemHandler.Items.Values)
                {
                    if (itemData.id == (int)item)
                    {
                        __result = itemData.gemBonus;
                        break;
                    }
                }
            }
        }

        //Patch the NPCs
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPHubNPC), "Start", MethodType.Normal)]
        static void PatchHubShopkeep(string ___NPCName, ref FPPowerup[] ___itemsForSale, ref int[] ___itemCosts, ref int[] ___starCardRequirements, ref FPMusicTrack[] ___musicID)
        {
            if ((___NPCName == "Blake" || ___NPCName == "Yuni" || ___NPCName == "Florin" || ___NPCName == "Chloe") && ___itemCosts != null && ___itemsForSale != null)
            {
                foreach (ItemData item in ItemHandler.Items.Values)
                {
                    if (item.shopLocation != IAddToShop.None || item.shopLocation != IAddToShop.ClassicOnly)
                    {
                        if ((item.shopLocation == IAddToShop.Blake && ___NPCName == "Blake")
                        || (item.shopLocation == IAddToShop.Yuni && ___NPCName == "Yuni")
                        || (item.shopLocation == IAddToShop.Florin && ___NPCName == "Florin")
                        || (item.shopLocation == IAddToShop.Chloe && ___NPCName == "Chloe"))
                        {
                            if (!___itemsForSale.Contains((FPPowerup)item.id))
                            {
                                ___itemsForSale = ___itemsForSale.AddToArray((FPPowerup)item.id);
                                ___itemCosts = ___itemCosts.AddToArray(item.goldGemsPrice);
                                ___starCardRequirements = ___starCardRequirements.AddToArray(item.starCards);
                                ___musicID = ___musicID.AddToArray(FPMusicTrack.NONE);
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
                //Are we in Item shop?
                if (___itemsForSale[0] != FPPowerup.NONE)
                {
                    foreach (ItemData item in ItemHandler.Items.Values)
                    {
                        if (item.shopLocation != IAddToShop.None && FPSaveManager.gameMode == FPGameMode.CLASSIC)
                        {
                            if (!___itemsForSale.Contains((FPPowerup)item.id))
                            {
                                ___itemsForSale = ___itemsForSale.AddToArray((FPPowerup)item.id);
                                ___itemCosts = ___itemCosts.AddToArray(item.goldGemsPrice);
                                ___starCardRequirements = ___starCardRequirements.AddToArray(item.starCards);
                                ___musicID = ___musicID.AddToArray(FPMusicTrack.NONE);
                            }
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
                if (fPHubNPC != null)
                {
                    if ((fPHubNPC.name == "Blake" || fPHubNPC.name == "Yuni" || fPHubNPC.name == "Florin" || fPHubNPC.name == "Chloe") && fPHubNPC.itemCosts != null && fPHubNPC.itemsForSale != null)
                    {
                        foreach (ItemData item in ItemHandler.Items.Values)
                        {
                            if (item.shopLocation != IAddToShop.None || item.shopLocation != IAddToShop.ClassicOnly)
                            {
                                if ((item.shopLocation == IAddToShop.Blake && fPHubNPC.name == "Blake")
                                || (item.shopLocation == IAddToShop.Yuni && fPHubNPC.name == "Yuni")
                                || (item.shopLocation == IAddToShop.Florin && fPHubNPC.name == "Florin")
                                || (item.shopLocation == IAddToShop.Chloe && fPHubNPC.name == "Chloe"))
                                {
                                    if (!fPHubNPC.itemsForSale.Contains((FPPowerup)item.id))
                                    {
                                        fPHubNPC.itemsForSale = fPHubNPC.itemsForSale.AddToArray((FPPowerup)item.id);
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
    }
}
