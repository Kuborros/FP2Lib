using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPSaveManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetItemDescription", MethodType.Normal)]
        static void PatchGetItemDescription(FPPowerup item, ref string __result)
        {
            if (FPSaveManager.character > FPCharacterID.NEERA)
            {
                PlayableChara character = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)FPSaveManager.character);
                if (item == FPPowerup.POWERUP_START)
                {
                    __result = character.powerupStartDescription;
                }
                if (item == FPPowerup.ELEMENT_BURST)
                {
                    switch (character.element)
                    {
                        case CharacterElement.WATER:
                            __result = "Your attacks cause lingering water damage to enemies.";
                            break;
                        case CharacterElement.METAL:
                            __result = "Your attacks cause lingering metal damage to enemies.";
                            break;
                        case CharacterElement.WOOD:
                            __result = "Your attacks cause lingering wood damage to enemies.";
                            break;
                        case CharacterElement.EARTH:
                            __result = "Your attacks cause lingering earth damage to enemies.";
                            break;
                        case CharacterElement.FIRE:
                            __result = "Your attacks cause lingering fire damage to enemies.";
                            break;
                        default:
                            __result = "Your attacks cause lingering elemental damage to enemies.";
                            break;
                    }
                }
            }
        }
    }
}
