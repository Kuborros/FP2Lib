﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuWorldMapConfirm
    {

        internal static readonly MethodInfo m_getTutorialScene = SymbolExtensions.GetMethodInfo(() => getTutorialScene());

        private static string getTutorialScene()
        {
            //Load up Lilac's tutorial for now.
            return "Tutorial1";
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "State_Transition", MethodType.Normal)]
        static IEnumerable<CodeInstruction> MWConfirmTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label patchStart = il.DefineLabel();
            Label patchEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_2)
                {
                    codes[i + 1].operand = patchStart;
                    codes[i+4].labels.Add(patchEnd);
                }

            }
            CodeInstruction spadCodeStart = new CodeInstruction(OpCodes.Ldloc_1);
            spadCodeStart.labels.Add(patchStart);

            codes.Add(spadCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_getTutorialScene)); 
            codes.Add(new CodeInstruction(OpCodes.Br, patchEnd));

            return codes;
        }
    }
}
