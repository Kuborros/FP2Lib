using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FP2Lib.BepIn;
using FP2Lib.NPC;
using FP2Lib.Patches;
using FP2Lib.Saves;
using FP2Lib.Tools;
using HarmonyLib;

namespace FP2Lib
{
    [BepInPlugin("000.kuborro.libraries.fp2.fp2lib", "FP2Lib", "0.2.3.1")]
    [BepInProcess("FP2.exe")]
    public class FP2Lib : BaseUnityPlugin
    {

        public static ConfigEntry<bool> configSaveRedirect;
        public static ConfigEntry<bool> configSaveFancy;
        public static ConfigEntry<int> configSaveProfile;

        public static GameInfo gameInfo;
        internal static ManualLogSource logSource;

        private void Awake()
        {

            gameInfo = new GameInfo();
            logSource = Logger;

            configSaveRedirect = Config.Bind("Save Redirection", "Enabled", false, new ConfigDescription("Enable save file redirection.",null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveFancy = Config.Bind("Save Redirection", "Fancy Json", false, new ConfigDescription("Makes JSON files more human-readable.",null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveProfile = Config.Bind("Save Redirection", "Profile", 1, new ConfigDescription("Select save redirection profile.",new AcceptableValueRange<int>(0,9),new ConfigurationManagerAttributes { IsAdvanced = true }));

            NPCHandler.InitialiseHandler();
            //PlayerHandler.InitialiseHandler();

            Logger.LogMessage("Running FP2 Version: " + gameInfo.getVersionString());
            if (gameInfo.build == GameRelease.SAMPLE)
            {
                Logger.LogWarning("Running within Sample Version! Aborting hook-in.");
                return;
            }

            Logger.LogInfo("FP2Lib initialisation started!");
            setupHarmonyPatches();
        }

        private void setupHarmonyPatches()
        {
            //NPC Lib
            Logger.LogDebug("NPC Patch Init");
            Harmony npcPatches = new("000.kuborro.libraries.fp2.fp2lib.npc");
            npcPatches.PatchAll(typeof(NPCPatches));

            //Logger.LogDebug("Player Patch Init");
            //Harmony playerPatches = new("000.kuborro.libraries.fp2.fp2lib.player");

            //Save Redirection
            Logger.LogDebug("Saves Patch Init");
            Harmony savePatches = new("000.kuborro.libraries.fp2.fp2lib.saves");
            savePatches.PatchAll(typeof(SavePatches));

            //Misc fixes and patches
            Logger.LogDebug("Bugfix Patch Init");
            Harmony generalPatches = new("000.kuborro.libraries.fp2.fp2lib.patches");
            if (gameInfo.gameVersion.CompareTo(new System.Version("1.2.6")) <= 0)
            {
                Logger.LogInfo("Game version lower than 1.2.6, applying screenshot resolution fix!");
                generalPatches.PatchAll(typeof(ScreenshotFix));
            }
            generalPatches.PatchAll(typeof(PotionSizeFix));
            generalPatches.PatchAll(typeof(ModdedPotionsFix));

            Logger.LogInfo("Init done!");
        }
    }
}
