using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FP2Lib.BepIn;
using FP2Lib.NPC;
using FP2Lib.Patches;
using FP2Lib.Player;
using FP2Lib.Saves;
using FP2Lib.Tools;
using FP2Lib.Vinyl;
using HarmonyLib;

namespace FP2Lib
{
    [BepInPlugin("000.kuborro.libraries.fp2.fp2lib", "FP2Lib", "0.2.4.0")]
    [BepInProcess("FP2.exe")]
    public class FP2Lib : BaseUnityPlugin
    {

        public static ConfigEntry<bool> configSaveRedirect;
        public static ConfigEntry<bool> configSaveFancy;
        public static ConfigEntry<int> configSaveProfile;

        internal static ConfigEntry<string> configLanguageForce;

        public static ConfigEntry<bool> configCowabunga;

        public static GameInfo gameInfo;
        public static string language = "english";
        internal static ManualLogSource logSource;

#pragma warning disable IDE0051 // Remove unused private members
        private void Awake()
#pragma warning restore IDE0051 // Remove unused private members
        {

            gameInfo = new GameInfo();
            logSource = Logger;

            configSaveRedirect = Config.Bind("Save Redirection", "Enabled", false, new ConfigDescription("Enable save file redirection.",null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveFancy = Config.Bind("Save Redirection", "Fancy Json", false, new ConfigDescription("Makes JSON files more human-readable.",null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveProfile = Config.Bind("Save Redirection", "Profile", 1, new ConfigDescription("Select save redirection profile.",new AcceptableValueRange<int>(0,9),new ConfigurationManagerAttributes { IsAdvanced = true }));
            configLanguageForce = Config.Bind("Language Settings", "Force Language", "", new ConfigDescription("Force specific language on launch. Leave empty for default behaviour.", null, new ConfigurationManagerAttributes { IsAdvanced = true }));

            configCowabunga = Config.Bind("Debug", "Cowabunga", false, new ConfigDescription("Engages cowabunga mode. No sanity checks will be run, will attempt to hook in on any FP2 version.\n" +
                "Yes, this includes 2015 Sample Versions. Your mileage might vary and bug reports with this mode on will *not* be accepted!\n" +
                "Some mods might read this value to skip their own checks.", null, new ConfigurationManagerAttributes { IsAdvanced = true }));

            if (!configLanguageForce.Value.IsNullOrWhiteSpace())
            {
                language = configLanguageForce.Value;
            }

            NPCHandler.InitialiseHandler();
            PlayerHandler.InitialiseHandler();
            VinylHandler.InitialiseHandler();

            Logger.LogMessage("Running FP2 Version: " + gameInfo.getVersionString());
            if (gameInfo.build == GameRelease.SAMPLE && !configCowabunga.Value)
            {
                Logger.LogWarning("Running within Sample Version! Aborting hook-in.");
                return;
            }

            if (configCowabunga.Value)
                Logger.LogWarning("Hook-in checks disabled by config option - things might break, but you asked for it! Cowabunga it is.");

            Logger.LogInfo("FP2Lib initialisation started!");
            SetupHarmonyPatches();
        }

        private void SetupHarmonyPatches()
        {
            //NPC
            Logger.LogDebug("NPC Patch Init");
            Harmony npcPatches = new("000.kuborro.libraries.fp2.fp2lib.npc");
            npcPatches.PatchAll(typeof(NPCPatches));

            //Player
            //Logger.LogDebug("Player Patch Init");
            //Harmony playerPatches = new("000.kuborro.libraries.fp2.fp2lib.player");

            //Vinyls
            Logger.LogDebug("Vinyl Patch Init");
            Harmony vinylPatches = new("000.kuborro.libraries.fp2.fp2lib.vinyl");
            vinylPatches.PatchAll(typeof(VinylPatches));

            //Save Redirection
            Logger.LogDebug("Saves Patch Init");
            Harmony savePatches = new("000.kuborro.libraries.fp2.fp2lib.saves");
            savePatches.PatchAll(typeof(SavePatches));

            //Misc fixes and patches
            Logger.LogDebug("Bugfix Patch Init");
            Harmony generalPatches = new("000.kuborro.libraries.fp2.fp2lib.patches");
            generalPatches.PatchAll(typeof(ScreenshotFix));
            generalPatches.PatchAll(typeof(PotionSizeFix));
            generalPatches.PatchAll(typeof(ModdedPotionsFix));
            generalPatches.PatchAll(typeof(LanguageExpander));

            Logger.LogInfo("Init done!");
        }
    }
}
