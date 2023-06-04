using HarmonyLib;

namespace FP2Lib.Player.Patches
{
    internal class PatchMenuPhoto
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuPhoto), "Start", MethodType.Normal)]
        static void PatchMenuPhotoStart(ref MenuPhotoPose[] ___poseList)
        {
            MenuPhotoPose poses = new MenuPhotoPose();
            for (int i = 1; i <= FP2Lib.Player.Patches.PlayableChars.Count; i++)
                ___poseList = ___poseList.AddToArray(poses);
        }
    }
}
