using BepInEx;
using BepInEx.Configuration;
using FP2Lib.BepIn;
using FP2Lib.NPC;
using FP2Lib.Patches;
using FP2Lib.Player;
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
            configSaveRedirect = Config.Bind("Save Redirection", "Enabled", false, new ConfigDescription("Enable save file redirection.",null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveFancy = Config.Bind("Save Redirection", "Fancy Json", false, new ConfigDescription("Makes JSON files more human-readable.",null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveProfile = Config.Bind("Save Redirection", "Profile", 1, new ConfigDescription("Select save redirection profile.",new AcceptableValueRange<int>(0,9),new ConfigurationManagerAttributes { IsAdvanced = true }));

            NPCHandler.InitialiseHandler();
            //PlayerHandler.InitialiseHandler();

            setupHarmonyPatches();
        }

        private void setupHarmonyPatches()
        {
            //NPC Lib
            Harmony npcPatches = new("000.kuborro.libraries.fp2.fp2lib.npc");
            npcPatches.PatchAll(typeof(NPCPatches));

            //Harmony playerPatches = new("000.kuborro.libraries.fp2.fp2lib.player");

            //Save Redirection
            Harmony savePatches = new("000.kuborro.libraries.fp2.fp2lib.saves");
            savePatches.PatchAll(typeof(SavePatches));

            //Misc fixes and patches
            Harmony generalPatches = new("000.kuborro.libraries.fp2.fp2lib.patches");
            generalPatches.PatchAll(typeof(ScreenshotFix));
            generalPatches.PatchAll(typeof(PotionSizeFix));
        }
    }
}
