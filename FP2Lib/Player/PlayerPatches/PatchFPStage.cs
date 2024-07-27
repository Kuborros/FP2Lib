using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPStage
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
        static void PatchStart(ref FPPlayer[] ___playerList)
        {
            //Load each playable characters GameObject prefab here, append to ___playerList in order of IDs

            //GameObject spadeObject = Plugin.moddedBundle.LoadAsset<GameObject>("Player Spade");
            //___playerList = ___playerList.AddItem(spadeObject.GetComponent<FPPlayer>()).ToArray();

        }
    }
}
