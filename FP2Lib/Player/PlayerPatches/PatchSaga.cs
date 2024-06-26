﻿using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchSaga
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Saga), "Start", MethodType.Normal)]
        private static void PatchSagaStart(ref Animator ___animator, Saga __instance) 
        {
            //If not playing as custom character, no touchie. Also do not mess with not yet initialised values
            if (FPSaveManager.character < (FPCharacterID)4 && __instance != null && ___animator != null)
            {
                //RuntimeAnimatorController spadeAnimatorSaga = Plugin.moddedBundle.LoadAsset<RuntimeAnimatorController>("SagaSpade");
                //RuntimeAnimatorController spadeAnimatorSaga2 = Plugin.moddedBundle.LoadAsset<RuntimeAnimatorController>("Saga2Spade");

                //if (__instance.name.Contains("Syntax")) //Code Black ver.
                //{
                //    ___animator.runtimeAnimatorController = spadeAnimatorSaga2;
                //}
                //else
                //{
                //    ___animator.runtimeAnimatorController = spadeAnimatorSaga;
                //}
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Saga), "State_Default", MethodType.Normal)]
        static IEnumerable<CodeInstruction> SagaDefaultTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_S)
                {
                    exitLabel = codes[i].labels[0];
                    codes[i].operand = entryLabel;
                }
            }

            CodeInstruction groundCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            groundCodeStart.labels.Add(entryLabel);

            codes.Add(groundCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Ldfld, typeof(Saga).GetField("animator", BindingFlags.NonPublic | BindingFlags.Instance)));
            codes.Add(new CodeInstruction(OpCodes.Ldstr, "TrapPlayer"));
            codes.Add(new CodeInstruction(OpCodes.Callvirt, typeof(Animator).GetMethod("Play", [typeof(string)])));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));

            return codes;
        }

    }
}
