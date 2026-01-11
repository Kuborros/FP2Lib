using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPEventSequence
    {
        private static bool HandleEventStart(FPEventSequence instance)
        {
            if (PlayerHandler.currentCharacter.useOwnCutsceneActivators)
            {
                //If using mod's own code, simply behave as we did not match. The mod will be handling it by itself.
                //Instance of current FPEventSequence is handed over for manipulation
                //A mod will likely just want to instance their own version, but borrow some data from the existing instance.
                PlayerHandler.currentCharacter.EventSequenceStart?.Invoke(instance);
                return false;
            }
            else
            {
                //Else, return the activation status of the character they picked
                switch (PlayerHandler.currentCharacter.eventActivatorCharacter)
                {
                    case FPCharacterID.LILAC:
                        return instance.lilac;
                    case FPCharacterID.CAROL:
                    case FPCharacterID.BIKECAROL:
                        return instance.carol;
                    case FPCharacterID.MILLA:
                        return instance.milla;
                    case FPCharacterID.NEERA:
                        return instance.neera;
                }
            }
            return false;
        }

        //This is only fired in cases where in HandleEventStart we picked built-in character.
        private static bool HandleEventDefault(FPEventSequence instance)
        {
            if (PlayerHandler.currentCharacter.useOwnCutsceneActivators)
            {
                //If using mod's own code, simply behave as we did not match. The mod will be handling it by itself.
                return false;
            }
            else
            {
                //Else, return the activation status of the character they picked
                switch (PlayerHandler.currentCharacter.eventActivatorCharacter)
                {
                    case FPCharacterID.LILAC:
                        return instance.lilac;
                    case FPCharacterID.CAROL:
                    case FPCharacterID.BIKECAROL:
                        return instance.carol;
                    case FPCharacterID.MILLA:
                        return instance.milla;
                    case FPCharacterID.NEERA:
                        return instance.neera;
                }
            }
            return false;
        }

        private static int HandleEventEvent(FPEventSequence instance, int num)
        {
            if (!PlayerHandler.currentCharacter.useOwnCutsceneActivators)
            {
                switch (PlayerHandler.currentCharacter.eventActivatorCharacter)
                {
                    case FPCharacterID.LILAC:
                        if (instance.lilacEndpoint > 0)
                        {
                            num = Mathf.Min(instance.lilacEndpoint, num);
                        }
                        break;
                    case FPCharacterID.CAROL:
                    case FPCharacterID.BIKECAROL:
                        if (instance.carolEndpoint > 0)
                        {
                            num = Mathf.Min(instance.carolEndpoint, num);
                        }
                        break;
                    case FPCharacterID.MILLA:
                        if (instance.millaEndpoint > 0)
                        {
                            num = Mathf.Min(instance.millaEndpoint, num);
                        }
                        break;
                    case FPCharacterID.NEERA:
                        if (instance.neeraEndpoint > 0)
                        {
                            num = Mathf.Min(instance.neeraEndpoint, num);
                        }
                        break;
                }
            }
            return num;
        }

        //Sets activation ranges.
        //First switch case
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPEventSequence), "Start", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceStartTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();
            LocalBuilder localvar = null;

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_S)
                {
                    exitLabel = (Label)codes[i + 1].operand;
                    codes[i + 1].operand = entryLabel;
                    localvar = (LocalBuilder)codes[i + 6].operand;
                    break;
                }
            }

            CodeInstruction codeStart = new CodeInstruction(OpCodes.Ldarg_0);
            codeStart.labels.Add(entryLabel);

            codes.Add(codeStart);
            codes.Add(CodeInstruction.Call(typeof(PatchFPEventSequence), nameof(HandleEventStart)));
            codes.Add(new CodeInstruction(OpCodes.Brfalse, exitLabel));
            codes.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
            codes.Add(new CodeInstruction(OpCodes.Stloc_S, localvar));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));

            return codes;
        }

        //Checks if should we activate for character.
        //First switch case
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPEventSequence), "State_Default", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceDefaultTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && (codes[i - 1].opcode == OpCodes.Ldloc_2))
                {
                    exitLabel = (Label)codes[i + 1].operand;
                    codes[i + 1].operand = entryLabel;
                    break;
                }
            }

            CodeInstruction entry = new CodeInstruction(OpCodes.Ldarg_0);
            entry.labels.Add(entryLabel);
            codes.Add(entry);
            codes.Add(CodeInstruction.Call(typeof(PatchFPEventSequence), nameof(HandleEventDefault)));
            codes.Add(new CodeInstruction(OpCodes.Stloc_0));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));
            return codes;
        }

        //Second switch case. They _are_ different, i promise! (They need another jump labels etc.)
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPEventSequence), "State_Default", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceDefaultTranspiler2(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && (codes[i - 1].opcode == OpCodes.Ldloc_S))
                {
                    exitLabel = (Label)codes[i + 1].operand;
                    codes[i + 1].operand = entryLabel;
                    break;
                }
            }

            CodeInstruction entry = new CodeInstruction(OpCodes.Ldarg_0);
            entry.labels.Add(entryLabel);
            codes.Add(entry);
            codes.Add(CodeInstruction.Call(typeof(PatchFPEventSequence), nameof(HandleEventDefault)));
            codes.Add(new CodeInstruction(OpCodes.Stloc_0));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));
            return codes;
        }

        //This handles getting to waypoints. Unless mod adds their own ones, assign ones for the character we masquarade as
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPEventSequence), "State_Event", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceEventTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    exitLabel = (Label)codes[i + 1].operand;
                    codes[i + 1].operand = entryLabel;
                    break;
                }
            }

            CodeInstruction entry = new CodeInstruction(OpCodes.Ldarg_0);
            entry.labels.Add(entryLabel);
            codes.Add(entry);
            codes.Add(new CodeInstruction(OpCodes.Ldloc_0));
            codes.Add(CodeInstruction.Call(typeof(PatchFPEventSequence), nameof(HandleEventEvent)));
            codes.Add(new CodeInstruction(OpCodes.Stloc_0));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));
            return codes;
        }
    }
}
