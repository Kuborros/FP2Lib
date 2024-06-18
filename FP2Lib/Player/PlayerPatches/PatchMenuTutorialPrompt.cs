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
            Label spadStart = il.DefineLabel();
            Label spadEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(spadStart).ToArray();
                    codes[i].operand = targets;
                    codes[i + 2].labels.Add(spadStart); //Load up Lilac's tutorial
                }

            }
            return codes;
        }
    }
}
