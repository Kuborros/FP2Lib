using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchBFWallRunZone
    {
        //Fire GroundMoves of custom character

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BFWallRunZone), "Update", MethodType.Normal)]
        static IEnumerable<CodeInstruction> BFWallRunZoneTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label groundStart = il.DefineLabel();
            Label groundEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_3)
                {
                    codes[i + 1].operand = groundStart;
                    groundEnd = (Label)codes[i + 5].operand;
                    break;
                }
            }

            CodeInstruction groundCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            groundCodeStart.labels.Add(groundStart);

            codes.Add(groundCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, PatchFPPlayer.m_GroundMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, groundEnd));


            return codes;
        }
    }
}
