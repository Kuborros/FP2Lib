using FP2Lib.Player.Patches.Character;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Player.Patches
{
    internal class PatchFPStage
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
        static void PatchStart(ref FPPlayer[] ___playerList)
        {

            foreach (PlayableChara character in FP2Lib.Player.Patches.PlayableChars.Values)
            {
                if (character.registered)
                {
                    if (character.id >= ___playerList.Length)
                    {
                        ___playerList = ___playerList.AddRangeToArray(new FPPlayer[(character.id + 1) - ___playerList.Length]);
                    }

                    if (character.id != 0)
                    {
                        ___playerList[character.id] = character.prefab.GetComponent<FPPlayer>();
                    }
                }
            }
        }
    }
}
