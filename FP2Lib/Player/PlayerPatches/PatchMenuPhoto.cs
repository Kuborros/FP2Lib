using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuPhoto
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuPhoto), "Start", MethodType.Normal)]
        static void PatchMenuPhotoStart(ref MenuPhotoPose[] ___poseList)
        {
            MenuPhotoPose spadePoses = new MenuPhotoPose();
            ___poseList = ___poseList.AddToArray(spadePoses);
        }
    }
}
