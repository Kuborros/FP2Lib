using HarmonyLib;

namespace FP2Lib.Player.Patches
{
    internal class PatchMenuFile
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuFile), "State_Main", MethodType.Normal)]
        static void PatchMenuFileStateMain(MenuFile __instance)
        {
            PlayerHandler.WriteToStorage();
        }
    }
}
