using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchGO3DColumn
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GO3DColumn), "Action_Dismount", MethodType.Normal)]
        static void Patch3DColumnDismount()
        {
            PatchFPPlayer.upDash = true;
        }
    }
}
