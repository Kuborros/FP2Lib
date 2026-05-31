using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchBFFCombiner
    {

        private static void BFFCombinerHandler(BFFCombinerCutscene instance)
        {
            int character = (int)FPSaveManager.character;

            //If character has their own cutscenes, they will be added to the array and can be played.
            if (PlayerHandler.currentCharacter.useOwnCutsceneActivators)
            {
                switch (character)
                {
                    case 0:
                        instance.cutsceneToStart[0].Activate(false);
                        break;
                    case 1:
                    case 2:
                        instance.cutsceneToStart[1].Activate(false);
                        break;
                    case 3:
                        instance.cutsceneToStart[2].Activate(false);
                        break;
                    case 4:
                        instance.cutsceneToStart[3].Activate(false);
                        break;
                    default:
                        instance.cutsceneToStart[character].Activate(false);
                        break;

                }
            }
            //Otherwise, fire off the activator for the character they mantle
            else
            {
                instance.cutsceneToStart[(int)PlayerHandler.currentCharacter.eventActivatorCharacter].Activate(false);
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BFFCombinerCutscene), "Update", MethodType.Normal)]
        static IEnumerable<CodeInstruction> BFFCombinerUpdateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_S)
                {
                    exitLabel = (Label)codes[i + 8].operand;
                    codes[i + 1].operand = entryLabel;
                }
            }
            CodeInstruction entry = new CodeInstruction(OpCodes.Ldarg_0);
            entry.labels.Add(entryLabel);
            codes.Add(entry);
            codes.Add(CodeInstruction.Call(typeof(PatchBFFCombiner), nameof(BFFCombinerHandler)));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));
            return codes;
        }
    }
}
