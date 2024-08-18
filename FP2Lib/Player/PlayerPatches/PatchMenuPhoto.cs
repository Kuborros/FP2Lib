using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuPhoto
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuPhoto), "Start", MethodType.Normal)]
        static void PatchMenuPhotoStart(ref MenuPhotoPose[] ___poseList)
        {
            for (int i = 4; i < PlayerHandler.highestID; i++)
            {
                ___poseList = ___poseList.AddToArray(new MenuPhotoPose());
            }

            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                ___poseList[chara.id] = chara.menuPhotoPose;
            }
        }
    }
}
