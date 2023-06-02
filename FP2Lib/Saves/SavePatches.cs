using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Saves
{
    internal class SavePatches
    {

        static string getSavesPath()
        {
            if (FP2Lib.configSaveRedirect.Value)
            {
                Directory.CreateDirectory("Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString());
                return "Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString();
            }
            else return Application.persistentDataPath;
        }


        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager),"SaveToFile",MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveWrite(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                FileLog.Log(codes[i].opcode.Name);
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveLoad(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "DeleteFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveDelete(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuFile), "GetFileInfo", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchFileInfo(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(getSavesPath);
                }
            }
            return codes;
        }

    }
}
