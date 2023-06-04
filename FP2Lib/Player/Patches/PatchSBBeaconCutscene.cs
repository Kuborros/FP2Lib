using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FP2Lib.Player.Patches
{
    internal class PatchSBBeaconCutscene
    {

        internal static MethodInfo m_ActivatorHandler = SymbolExtensions.GetMethodInfo(() => CustomActivationHandler());
        private static SBBeaconCutscene instance;

        private static void CustomActivationHandler()
        {
            if (!FP2Lib.Player.Patches.currentCharacter.useOwnCutsceneActivators)
            {
                switch (FP2Lib.Player.Patches.currentCharacter.eventActivatorCharacter)
                {
                    case FPCharacterID.LILAC:
                        instance.cutsceneToStart[0].Activate(false);
                        break;
                    case FPCharacterID.CAROL:
                    case FPCharacterID.BIKECAROL:
                        instance.cutsceneToStart[1].Activate(false);
                        break;
                    case FPCharacterID.MILLA:
                        instance.cutsceneToStart[2].Activate(false);
                        break;
                    case FPCharacterID.NEERA:
                        instance.cutsceneToStart[3].Activate(false);
                        break;
                    default:
                        instance.cutsceneToStart[0].Activate(false); //Default to Lilac
                        break;
                }
            }
            else 
            {
                //Nothin yet
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SBBeaconCutscene), "Update", MethodType.Normal)]
        static void PatchSBBeaconUpdate(SBBeaconCutscene __instance)
        {
            instance = __instance;
        }


        [HarmonyTranspiler]
        [HarmonyPatch(typeof(SBBeaconCutscene), "Update", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceDefaultTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label customCheck = il.DefineLabel();
            Label customCheckReturn = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_3)
                {
                    codes[i + 1].operand = customCheck;
                    codes[i + 30].labels.Add(customCheckReturn);
                }

                CodeInstruction customStart = new CodeInstruction(OpCodes.Ldarg_0);
                customStart.labels.Add(customCheck);

                codes.Add(customStart);
                codes.Add(new CodeInstruction(OpCodes.Call, m_ActivatorHandler));
                codes.Add(new CodeInstruction(OpCodes.Br, customCheckReturn));


            }
            return codes;
        }
    }
}
