using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using FP2Lib.Player;
using HarmonyLib;
using System.Linq;

namespace FP2Lib.Item.ItemPatches
{
    internal class ItemFPSaveManagerPatches
    {
        private static readonly ManualLogSource ItemLogSource = FP2Lib.logSource;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PotionSellerRadar()
        {
            //Detect if Potion Seller is installed. Needed to adjust inventory clean up ranges.
            ItemHandler.isPotionSellerInstalled = Chainloader.PluginInfos.ContainsKey("com.eps.plugin.fp2.potion-seller");
        }


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
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemName", MethodType.Normal)]
        static void PatchFPSaveManagerItemName(FPPowerup item, ref string __result)
        {
            if (__result == "No Item") //Default method returned "No item", check if maybe we got the item
            {
                foreach (ItemData itemData in ItemHandler.Items.Values)
                {
                    if (itemData.id == (int)item)
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
        [HarmonyPatch(typeof(FPSaveManager), "GetPotionDescription", MethodType.Normal)]
        static void PatchFPSaveManagerItemDescription(FPPowerup item, ref string __result)
        {
            if (__result.IsNullOrWhiteSpace()) //Default method returned nothing, check if maybe we got the item
            {
                foreach (ItemData itemData in ItemHandler.Items.Values)
                {
                    if (itemData.id == (int)item)
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
                                if (FPSaveManager.character > FPCharacterID.NEERA)
                                {
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
        [HarmonyPatch(typeof(FPSaveManager), "GetItemBonus", MethodType.Normal)]
        static void PatchFPSaveManagerItemBonus(FPPowerup item, ref float __result)
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
    }
}
