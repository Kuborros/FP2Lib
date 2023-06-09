﻿using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace FP2Lib.Player.Patches
{
    internal class PatchItemFuel
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemFuel), "Start", MethodType.Normal)]
        static void PatchStart(ref Sprite[] ___iconSprite)
        {
            for (int i = 1; i <= PlayerHandler.PlayableChars.Count; i++) 
            {
                ___iconSprite = ___iconSprite.AddToArray(___iconSprite[1]);
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ItemFuel), "CollisionCheck", MethodType.Normal)]
        static IEnumerable<CodeInstruction> ItemFuelTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label fuelCheck = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_2)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    codes[i + 33].labels.Add(fuelCheck);
                    targets = targets.AddItem(fuelCheck).ToArray();
                    codes[i].operand = targets;
                }

            }
            return codes;
        }

    }
}
