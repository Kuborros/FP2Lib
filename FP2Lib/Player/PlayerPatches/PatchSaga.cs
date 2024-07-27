using HarmonyLib;
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
            if (FPSaveManager.character <= (FPCharacterID)5 && __instance != null && ___animator != null)
            {
                //Pull Saga animations from character's asstetpackage
                RuntimeAnimatorController AnimatorSaga = PlayerHandler.currentCharacter.dataBundle.LoadAssetWithSubAssets<RuntimeAnimatorController>("SagaBlock")[0];
                RuntimeAnimatorController AnimatorSaga2 = PlayerHandler.currentCharacter.dataBundle.LoadAssetWithSubAssets<RuntimeAnimatorController>("SyntaxSagaBlock")[0];

                if (__instance.name.Contains("Syntax")) //Code Black ver.
                {
                    ___animator.runtimeAnimatorController = AnimatorSaga2;
                }  
                else
                {
                    ___animator.runtimeAnimatorController = AnimatorSaga;
                }
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
