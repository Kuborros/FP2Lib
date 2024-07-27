using HarmonyLib;

namespace FP2Lib.Vinyl
{
    internal class VinylPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.LoadFromFile), MethodType.Normal)]
        static void PatchFPSaveManager(ref bool[] ___musicTracks)
        {

            ___musicTracks = FPSaveManager.ExpandBoolArray(___musicTracks, VinylHandler.totalTracks);


        }
    }
}
