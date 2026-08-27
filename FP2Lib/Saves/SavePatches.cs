using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace FP2Lib.Saves
{

    [Serializable]
    internal class GlobalData
    {
        public byte[] badges = new byte[512];
    }
    internal class SavePatches
    {
        internal static string GetSavesPath()
        {
            if (FP2Lib.configSaveRedirect.Value)
            {
                if (!SaveRedirectHandler.CurrentModRedirectProfile.IsNullOrWhiteSpace())
                {
                    Directory.CreateDirectory("Saves\\Profile_" + SaveRedirectHandler.CurrentModRedirectProfile);
                    return "Saves\\Profile_" + SaveRedirectHandler.CurrentModRedirectProfile;
                }
                else if (FP2Lib.configSaveProfile.Value != 0)
                {
                    Directory.CreateDirectory("Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString());
                    return "Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString();
                }
            }
            return Application.persistentDataPath;
        }

        //Modifies output of JsonUtility to switch to 'everyting is not yeeted into single line' mode.
        static string FancifyJson(System.Object obj)
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
                    codes[i] = Transpilers.EmitDelegate(FancifyJson);
                }
            }
            return codes;
        }

        //Override path from 'Application.persistentDataPath' to return value of 'getSavesPath()'
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "SaveToFile", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchSaveWrite(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Ldc_I4_0 && codes[i - 2].opcode == OpCodes.Dup)
                {
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);
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
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);
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
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);
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
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);
                }
            }
            return codes;
        }


        //Override path from 'Application.persistentDataPath' to return value of 'getSavesPath()'
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "SaveGlobalData", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchGlobalSave(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Stloc_3 && codes[i - 2].opcode == OpCodes.Callvirt)
                {
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);
                }
            }
            return codes;
        }

        //Override path from 'Application.persistentDataPath' to return value of 'getSavesPath()'
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPSaveManager), "LoadGlobalData", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PatchGlobalLoad(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i - 1].opcode == OpCodes.Stloc_1 && codes[i - 2].opcode == OpCodes.Ldsfld)
                {
                    codes[i] = Transpilers.EmitDelegate(GetSavesPath);
                }
            }
            return codes;
        }


        //Load the global badges in case we don't have per-profile ones.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadGlobalData", MethodType.Normal)]
        static bool PatchGlobalLoadPre(ref bool __result, ref byte[] ___badges)
        {
            if (!File.Exists(GetSavesPath() + "/global.json"))
            {
                if (!FP2Lib.configSaveBadges.Value)
                {
                    FP2Lib.logSource.LogInfo("Per-profile badge data not present, loading empty one!");
                    ___badges = new byte[99];
                    return false;
                }

                FP2Lib.logSource.LogInfo("Per-profile badge data not present, loading global one!");
                if (File.Exists(Application.persistentDataPath + "/global.json"))
                {
                    try
                    {
                        string json = string.Empty;
                        using (StreamReader streamReader = new StreamReader(Application.persistentDataPath + "/global.json", Encoding.UTF8))
                        {
                            json = streamReader.ReadToEnd();
                        }
                        ___badges = FPSaveManager.ExpandByteArray(JsonUtility.FromJson<GlobalData>(json).badges, 99);
                        __result = true;
                        return false;
                    }
                    catch (Exception ex)
                    {
                        __result = false;
                        Debug.LogError($"An exception of type '{ex.GetType().Name}' has been caught when loading global data from global path! Oh no!\nMessage: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    }
                }
            }
            return true;
        }
    }
}

