using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchSBBeaconCutscene
    {

        private static SBBeaconCutscene instance;
        internal static readonly MethodInfo m_SBBeaconHandler = SymbolExtensions.GetMethodInfo(() => SBBeaconCutsceneHandler());


        private static void SBBeaconCutsceneHandler()
        {
            int character = (int)FPSaveManager.character;

            //If character has their own cutscenes, they will be added to the array and can be played. (I hope, that's the mod maker's responibility.)
            if (PlayerHandler.currentCharacter.useOwnCutsceneActivators)
            {
                instance.cutsceneToStart[character].Activate(false);
            }
            //Otherwise, fire off the activator for the character they mantle
            else
            {
                instance.cutsceneToStart[(int)PlayerHandler.currentCharacter.eventActivatorCharacter].Activate(false);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SBBeaconCutscene), "Update", MethodType.Normal)]
        static void EventSequencePrefix(SBBeaconCutscene __instance)
        {
            //Dirty hack
            instance = __instance;
        }


        //Adding new "default:" section for the enum. All extra characters will miss the built-in sections and get forwarded to our code instead. 
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(SBBeaconCutscene), "Update", MethodType.Normal)]
        static IEnumerable<CodeInstruction> EventSequenceDefaultTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_3)
                {
                    exitLabel = (Label)codes[i+1].operand;
                    codes[i+1].operand = entryLabel;
                }
            }
            CodeInstruction entry = new CodeInstruction(OpCodes.Ldarg_0);
            entry.labels.Add(entryLabel);
            codes.Add(entry);
            codes.Add(new CodeInstruction(OpCodes.Callvirt, m_SBBeaconHandler));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));
            return codes;
        }
    }
}
