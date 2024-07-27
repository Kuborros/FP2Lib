using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuPhoto
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuPhoto), "Start", MethodType.Normal)]
        static void PatchMenuPhotoStart(ref MenuPhotoPose[] ___poseList)
        {
            //Add pose data for each custom (even if empty)
        }
    }
}
