using BepInEx;
using FP2Lib.Item;
using FP2Lib.Player;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FP2Lib.Potion.PotionPatches
{
    internal class PotionFPSaveManagerPatches
    {

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(FPSaveManager), "GetPotionEffect", MethodType.Normal)]
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
    }
}
