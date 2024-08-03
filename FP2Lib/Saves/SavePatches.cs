using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Saves
{
    internal class SavePatches
    {
        /// <summary>
        /// Returns path to current profile's save directory
        /// </summary>
        /// <returns>Save file path</returns>
        public static string getSavesPath()
        {
            if (FP2Lib.configSaveRedirect.Value && FP2Lib.configSaveProfile.Value != 0)
            {
                Directory.CreateDirectory("Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString());
                return "Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString();
            }
            else return Application.persistentDataPath;
        }

        //Modifies output of JsonUtility to switch to 'everyting is not yeeted into single line' mode.
        static string fancifyJson(Object obj)
        {
            if (FP2Lib.configSaveFancy.Value)
            {
                return JsonUtility.ToJson(obj, true);
            }
            else return JsonUtility.ToJson(obj, false);
        }

        //Change Call OpCode target from 'JsonUtility.ToJson(obj)' to 'fancifyJson(obj)'
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "SaveToFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchJsonStyle(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && (codes[i - 1].opcode == OpCodes.Ldloc_0 || codes[i - 1].opcode == OpCodes.Ldloc_1) && codes[i - 2].opcode == OpCodes.Stfld)
                {
                    codes[i] = Transpilers.EmitDelegate(fancifyJson);
                }
            }
            return codes;
        }

        //Override path from 'Application.persistentDataPath' to return value of 'getSavesPath()'
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager),"SaveToFile",MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveWrite(IEnumerable<CodeInstruction> instructions, ILGenerator il)
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

        //Override path from 'Application.persistentDataPath' to return value of 'getSavesPath()'
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

        //Override path from 'Application.persistentDataPath' to return value of 'getSavesPath()'
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

        //Override path from 'Application.persistentDataPath' to return value of 'getSavesPath()'
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
