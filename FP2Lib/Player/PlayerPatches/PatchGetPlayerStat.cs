using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchGetPlayerStat
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "GetPlayerStat_Default_Acceleration", [typeof(FPCharacterID)] )]
        static void PatchPlayerStatDefaultAcceleration(ref float __result, FPCharacterID character)
        {
            if (character > FPCharacterID.NEERA)
            {
                __result = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)character).statDefaultAcceleration;
            }
        }

        [HarmonyPostfix]
        //Misspelled in the game code as Aceleration.
        [HarmonyPatch(typeof(FPPlayer), "GetPlayerStat_Default_AirAceleration", [typeof(FPCharacterID)])]
        static void PatchPlayerStatDefaultAirAcceleration(ref float __result, FPCharacterID character)
        {
            if (character > FPCharacterID.NEERA)
            {
                __result = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)character).statDefaultAirAcceleration;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "GetPlayerStat_Default_Deceleration", [typeof(FPCharacterID)])]
        static void PatchPlayerStatDefaultDeceleration(ref float __result, FPCharacterID character)
        {
            if (character > FPCharacterID.NEERA)
            {
                __result = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)character).statDefaultDeceleration;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "GetPlayerStat_Default_JumpRelease", [typeof(FPCharacterID)])]
        static void PatchPlayerStatDefaultJumpRelease(ref float __result, FPCharacterID character)
        {
            if (character > FPCharacterID.NEERA)
            {
                __result = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)character).statDefaultJumpRelease;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "GetPlayerStat_Default_JumpStrength", [typeof(FPCharacterID)])]
        static void PatchPlayerStatDefaultJumpStrength(ref float __result, FPCharacterID character)
        {
            if (character > FPCharacterID.NEERA)
            {
                __result = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)character).statDefaultJumpStrength;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "GetPlayerStat_Default_TopSpeed", [typeof(FPCharacterID)])]
        static void PatchPlayerStatDefaultTopSpeed(ref float __result, FPCharacterID character)
        {
            if (character > FPCharacterID.NEERA)
            {
                __result = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)character).statDefaultTopSpeed;
            }
        }
    }
}
