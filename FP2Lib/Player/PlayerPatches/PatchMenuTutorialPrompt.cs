using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuTutorialPrompt
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuTutorialPrompt), "State_Transition", MethodType.Normal)]
        static IEnumerable<CodeInstruction> MWConfirmTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label patchStart = il.DefineLabel();
            Label patchEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    codes[i + 1].operand = patchStart;
                    codes[i + 4].labels.Add(patchEnd);
                }

            }
            CodeInstruction codeStart = new CodeInstruction(OpCodes.Ldloc_0);
            codeStart.labels.Add(patchStart);

            codes.Add(codeStart);
            codes.Add(new CodeInstruction(OpCodes.Call,PatchMenuWorldMapConfirm.m_getTutorialScene));
            codes.Add(new CodeInstruction(OpCodes.Br, patchEnd));

            return codes;
        }
    }
}
