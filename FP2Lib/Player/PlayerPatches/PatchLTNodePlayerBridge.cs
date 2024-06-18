using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchLTNodePlayerBridge
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LTNodePlayerBridge), "Action_Dismount", MethodType.Normal)]
        static void PatchPlayerBridge()
        {
            PatchFPPlayer.upDash = true;
        }
    }
}
