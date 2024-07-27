using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchBakunawaFusion
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bakunawa), "Activate", MethodType.Normal)]
        static void PatchBakunawaActivate(Bakunawa __instance)
        {
            if (FPSaveManager.character <= (FPCharacterID)5)
            {
                //Everyone is here to help!
                __instance.assistRoster.Add(Bakunawa.Assist.Milla);
                __instance.assistRoster.Add(Bakunawa.Assist.Neera);
                __instance.assistRoster.Add(Bakunawa.Assist.Carol);
                __instance.assistRoster.Add(Bakunawa.Assist.Lilac);
                __instance.assistRoster.Add(Bakunawa.Assist.Merga);
            }
        }
    }
}
