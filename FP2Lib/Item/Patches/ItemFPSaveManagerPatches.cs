using BepInEx;
using BepInEx.Logging;
using FP2Lib.Player;
using HarmonyLib;
using System;

namespace FP2Lib.Item.Patches
{
    internal class ItemFPSaveManagerPatches
    {
        private static readonly ManualLogSource ItemLogSource = FP2Lib.logSource;

        //Update save file to account for extra Items and Potions
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad(ref byte[] ___inventory)
        {
            //Calculate how many items we have
            int totalItems = ItemHandler.baseItems + ItemHandler.Items.Count;
            foreach (ItemData item in ItemHandler.Items.Values)
            {
                //Highest ID is the item number
                if (item.itemID > totalItems)
                {
                    totalItems = item.itemID;
                }
            }
            //Add slots in file for extra items. 
            //Potion seller already does 99, so we should start at 100 to not mess with it
            ___inventory = FPSaveManager.ExpandByteArray(___inventory, totalItems);

            ItemHandler.WriteItemsToStorage();
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemName", MethodType.Normal)]
        static void PatchFPSaveManagerItemName(FPPowerup item, ref string __result)
        {
            if (__result == "No Item") //Default method returned "No item", check if maybe we got the item
            {
                foreach (ItemData itemData in ItemHandler.Items.Values)
                {
                    if (itemData.itemID == (int)item)
                    {
                        //Even uninstalled items keep their name, so to see this the item must be turbo broken.
                        if (itemData.name.IsNullOrWhiteSpace()) __result = "Missing Item! You should not be seeing this!";
                        else if (itemData.sprite == null) __result = itemData.name + " - (Deleted Mod)";
                        else __result = itemData.name;
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemDescription", MethodType.Normal)]
        [HarmonyPatch(typeof(FPSaveManager), "GetPotionDescription", [typeof(FPPowerup)])]
        static void PatchFPSaveManagerItemDescription(FPPowerup item, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace()) //Default method returned nothing, check if maybe we got the item
            {
                foreach (ItemData itemData in ItemHandler.Items.Values)
                {
                    if (itemData.itemID == (int)item)
                    {
                        switch (FPSaveManager.character)
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
                                if (FPSaveManager.character > FPCharacterID.NEERA && PlayerHandler.currentCharacter != null)
                                {
                                    if (itemData.descriptionCustom.ContainsKey(PlayerHandler.currentCharacter.uid))
                                        __result = itemData.descriptionCustom[PlayerHandler.currentCharacter.uid];
                                }
                                break;
                        }
                        if (__result.IsNullOrWhiteSpace()) __result = itemData.descriptionGeneric;
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemBonus", MethodType.Normal)]
        static void PatchFPSaveManagerItemBonus(FPPowerup item, ref float __result)
        {
            if (__result == 0f)
            {
                ItemData itemData = ItemHandler.GetItemDataByRuntimeItemID((int)item);
                if (itemData != null)
                {
                    __result = itemData.gemBonus;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(FPSaveManager), "GetPotionEffect", MethodType.Normal)]
        static void PatchFPSaveManagerPotionEffect(int item, float amount, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace()) //Default method returned nothing, check if maybe we got the item
            {
                ItemData data = ItemHandler.GetItemDataByRuntimePotionID(item);
                if (data != null)
                {
                    float effectScale = amount * data.effectPercentage;
                    __result = String.Concat(effectScale, data.effect);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(FPSaveManager), "GetPotionID", MethodType.Normal)]
        static void PatchFPSaveManagerPotionID(FPPowerup item, ref int __result)
        {
            if (__result == -1) //Default method returned nothing, check if maybe we got the item
            {
                ItemData data = ItemHandler.GetItemDataByRuntimeItemID((int)item);
                if (data != null) __result = data.potionID;
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(FPSaveManager), "GetPotionInventoryID", MethodType.Normal)]
        static void PatchFPSaveManagerPotionInventoryID(int id, ref FPPowerup __result)
        {
            if (__result == 0) //Default method returned nothing, check if maybe we got the item
            {
                ItemData data = ItemHandler.GetItemDataByRuntimePotionID(id);
                if (data != null) __result = (FPPowerup)data.itemID;
            }
        }
    }
}

