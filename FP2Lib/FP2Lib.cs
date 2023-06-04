using BepInEx;
using BepInEx.Configuration;
using FP2Lib.NPC;
using FP2Lib.Saves;
using HarmonyLib;

namespace FP2Lib
{
    [BepInPlugin("000.kuborro.libraries.fp2.fp2lib", "FP2Lib", "0.1.0")]
    [BepInProcess("FP2.exe")]
    public class FP2Lib : BaseUnityPlugin
    {

        public static ConfigEntry<bool> configSaveRedirect;
        public static ConfigEntry<bool> configSaveFancy;
        public static ConfigEntry<int> configSaveProfile;


        private void Awake()
        {
            configSaveRedirect = Config.Bind("Save Redirection", "Enabled", false, "Enable save file redirection.");
            configSaveFancy = Config.Bind("Save Redirection", "Fancy Json", false, "Makes JSON files more human-readable.");
            configSaveProfile = Config.Bind("Save Redirection", "Profile", 1, "Select save redirection profile.");
            NPCHandler.InitialiseHandler();


            setupHarmonyPatches();
        }

        private void setupHarmonyPatches()
        {

            Harmony npcPatches = new Harmony("000.kuborro.libraries.fp2.fp2lib.npc");
            npcPatches.PatchAll(typeof(NPCPatches));

            //Harmony playerPatches = new Harmony("000.kuborro.libraries.fp2.fp2lib.player");

            Harmony savePatches = new Harmony("000.kuborro.libraries.fp2.fp2lib.saves");
            savePatches.PatchAll(typeof(SavePatches));

        }
    }
}
