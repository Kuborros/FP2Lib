using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPStage
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
        static void PatchStart(ref FPPlayer[] ___playerList)
        {
            //Load each playable characters GameObject prefab here, append to ___playerList in order of IDs
            for (int i = 4;  i < PlayerHandler.highestID; i++)
            {
                ___playerList = ___playerList.AddToArray(null);
            }

            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                if (chara.registered)
                {
                    ___playerList[chara.id] = chara.prefab.GetComponent<FPPlayer>();
                }
            }
        }
    }
}
