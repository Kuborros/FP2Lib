using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPEventSequence
    {


        //Sets activation ranges. Can maybe be done with prefix? (They are not edited if it falls trough)
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPEventSequence), "Start", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceStartTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label carolCheck = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_S)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    codes[i + 8].labels.Add(carolCheck);
                    targets = targets.AddItem(carolCheck).ToArray();
                    codes[i].operand = targets;
                    break;
                }
            }
            return codes;
        }

        //Checks if should we activate for character. Consider replacing with postfix, as it simply fires this.Activate(false) at the end!
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPEventSequence), "State_Default", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceDefaultTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label carolCheck = il.DefineLabel();
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && (codes[i - 1].opcode == OpCodes.Ldloc_2 || codes[i - 1].opcode == OpCodes.Ldloc_S))
                {
                    Label[] targets = (Label[])codes[i].operand;
                    codes[i + 8].labels.Add(carolCheck);
                    targets = targets.AddItem(carolCheck).ToArray();
                    codes[i].operand = targets;
                }
            }
            return codes;
        }

        //This handles getting to waypoints. Unless mod adds their own ones, assign ones for the character we masquarade as
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPEventSequence), "State_Event", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceEventTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label carolCheck = il.DefineLabel();
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    codes[i + 8].labels.Add(carolCheck);
                    targets = targets.AddItem(carolCheck).ToArray();
                    codes[i].operand = targets;
                    break;
                }
            }
            return codes;
        }
    }
}
