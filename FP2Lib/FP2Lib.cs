using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FP2Lib.Badge;
using FP2Lib.BepIn;
using FP2Lib.Item;
using FP2Lib.Item.ItemPatches;
using FP2Lib.Map;
using FP2Lib.NPC;
using FP2Lib.Patches;
using FP2Lib.Player;
using FP2Lib.Player.PlayerPatches;
using FP2Lib.Potion;
using FP2Lib.Potion.PotionPatches;
using FP2Lib.Saves;
using FP2Lib.Stage;
using FP2Lib.Tools;
using FP2Lib.Vinyl;
using HarmonyLib;
using System;

namespace FP2Lib
{
    [BepInPlugin("000.kuborro.libraries.fp2.fp2lib", "FP2Lib", "0.5.0.0")]
    [BepInProcess("FP2.exe")]
    public class FP2Lib : BaseUnityPlugin
    {

        public static ConfigEntry<bool> configSaveRedirect;
        public static ConfigEntry<bool> configSaveFancy;
        public static ConfigEntry<int> configSaveProfile;

        public static ConfigEntry<bool> configCowabunga;

        public static GameInfo gameInfo;
        internal static ManualLogSource logSource;

        private void Awake()
        {

            gameInfo = new GameInfo();
            logSource = Logger;

            configSaveRedirect = Config.Bind("Save Redirection", "Enabled", false, new ConfigDescription("Enable save file redirection.", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveFancy = Config.Bind("Save Redirection", "Fancy Json", false, new ConfigDescription("Makes JSON files more human-readable.", null, new ConfigurationManagerAttributes { IsAdvanced = true }));
            configSaveProfile = Config.Bind("Save Redirection", "Profile", 1, new ConfigDescription("Select save redirection profile.", new AcceptableValueRange<int>(0, 9), new ConfigurationManagerAttributes { IsAdvanced = true }));

            configCowabunga = Config.Bind("Debug", "Cowabunga", false, new ConfigDescription("Engages cowabunga mode. No version checks will be run, will attempt to hook in on any FP2 version.\n" +
                "Yes, this includes 2015 Sample Versions. Your mileage might vary and bug reports with this mode on will *not* be accepted!\n" +
                "Some mods might read this value to skip their own checks.", null, new ConfigurationManagerAttributes { IsAdvanced = true }));

            NPCHandler.InitialiseHandler();
            PlayerHandler.InitialiseHandler();
            VinylHandler.InitialiseHandler();
            BadgeHandler.InitialiseHandler();
            MapHandler.InitialiseHandler();
            StageHandler.InitialiseHandler();
            PotionHandler.InitialiseHandler();
            ItemHandler.InitialiseHandler();

            Logger.LogMessage("Running FP2 Version: " + gameInfo.getVersionString());
            if (gameInfo.build == GameRelease.SAMPLE && !configCowabunga.Value)
            {
                Logger.LogError("Running within Sample Version! Aborting hook-in.");
                return;
            }
            //1.2.6r adds major changes to the game's code, and many mods will not work on older builds.
            if (gameInfo.gameVersion < new System.Version("1.2.6"))
            {
                Logger.LogWarning("You are running an older version of FP2! Not all mods might be compatible!");
            }

            if (configCowabunga.Value)
                Logger.LogWarning("Hook-in checks disabled by config option - things might break, but you asked for it! **Cowabunga it is.**");

            Logger.LogInfo("FP2Lib initialisation started!");
            SetupHarmonyPatches();
        }

        private void SetupHarmonyPatches()
        {
            //Patches are separate to allow for hooking-in even if one set of them fails.
            //This also allows for selective unpatching of library's features.

            //NPC
            Logger.LogDebug("NPC Patch Init");
            //Code can handle older game builds.
            try
            {
                Harmony npcPatches = new("000.kuborro.libraries.fp2.fp2lib.npc");
                npcPatches.PatchAll(typeof(NPCPatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("NPC Patch Failed! Info:" + ex.Message);
            }

            //Player
            Logger.LogDebug("Player Patch Init");
            //Player patches *will* break on anything older than 1.2.6 due to changes in guard code.
            if (gameInfo.gameVersion >= new Version("1.2.6") || configCowabunga.Value)
            {
                try
                {
                    Harmony playerPatches = new("000.kuborro.libraries.fp2.fp2lib.player");
                    playerPatches.PatchAll(typeof(PatchFPPlayer));
                    playerPatches.PatchAll(typeof(PatchItemFuel));
                    playerPatches.PatchAll(typeof(PatchPlayerSpawnPoint));
                    playerPatches.PatchAll(typeof(PatchFPStage));
                    playerPatches.PatchAll(typeof(PatchMenuFile));
                    playerPatches.PatchAll(typeof(PatchFPEventSequence));
                    playerPatches.PatchAll(typeof(PatchParentActivator));
                    playerPatches.PatchAll(typeof(PatchArenaRace));
                    playerPatches.PatchAll(typeof(PatchMenuCharacterSelect));
                    playerPatches.PatchAll(typeof(PatchMenuCharacterWheel));
                    playerPatches.PatchAll(typeof(PatchMenuTutorialPrompt));
                    playerPatches.PatchAll(typeof(PatchMenuGlobalPause));
                    playerPatches.PatchAll(typeof(PatchMenuPhoto));
                    playerPatches.PatchAll(typeof(PatchBFFCombiner));
                    playerPatches.PatchAll(typeof(PatchFPHudDigit));
                    playerPatches.PatchAll(typeof(PatchFPHudMaster));
                    playerPatches.PatchAll(typeof(PatchSBBeaconCutscene));
                    playerPatches.PatchAll(typeof(PatchBakunawaFusion));
                    playerPatches.PatchAll(typeof(PatchBFWallRunZone));
                    playerPatches.PatchAll(typeof(PatchBSAutoscroller));
                    playerPatches.PatchAll(typeof(PatchArenaCameraFlash));
                    playerPatches.PatchAll(typeof(PatchMenuCredits));
                    playerPatches.PatchAll(typeof(PatchMenuWorldMap));
                    playerPatches.PatchAll(typeof(PatchFPBossHud));
                    playerPatches.PatchAll(typeof(PatchMenuWorldMapConfirm));
                    playerPatches.PatchAll(typeof(PatchSaga));
                    playerPatches.PatchAll(typeof(PatchAcrabellePieTrap));
                    playerPatches.PatchAll(typeof(PatchMenuShop));
                    playerPatches.PatchAll(typeof(PatchPlayerDialogZone));
                    playerPatches.PatchAll(typeof(PatchZLBaseballFlyer));
                    playerPatches.PatchAll(typeof(PatchItemStarCard));
                    playerPatches.PatchAll(typeof(PatchPlayerBossMerga));
                    playerPatches.PatchAll(typeof(PatchGetPlayerStat));
                    playerPatches.PatchAll(typeof(PatchFPSaveManager));
                    playerPatches.PatchAll(typeof(PatchFPBaseEnemy));
                    playerPatches.PatchAll(typeof(PatchFPPauseMenu));
                }
                catch (Exception ex)
                {
                    Logger.LogError("Something went wrong while patching player code! Info:\n" + ex.Message);
                    Logger.LogDebug(ex);
                }
            }
            else
            {
                Logger.LogWarning("Player patches are not functional in FP2 builds older than 1.2.6! Sorry!\nMods dependent on these *will* be broken!");
            }

            //Vinyls
            Logger.LogDebug("Vinyl Patch Init");
            try
            {
                Harmony vinylPatches = new("000.kuborro.libraries.fp2.fp2lib.vinyl");
                vinylPatches.PatchAll(typeof(VinylPatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("Vinyl Patch Failed! Info:" + ex.Message);
            }

            //Badges
            Logger.LogDebug("Badge Patch Init");
            try
            {
                Harmony badgePatches = new("000.kuborro.libraries.fp2.fp2lib.badge");
                badgePatches.PatchAll(typeof(BadgePatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("Badge Patch Failed! Info:" + ex.Message);
            }

            //Items
            Logger.LogDebug("Item Patch Init");
            try
            {
                Harmony itemPatches = new("000.kuborro.libraries.fp2.fp2lib.item");
                itemPatches.PatchAll(typeof(ItemShopPatches));
                itemPatches.PatchAll(typeof(ItemMenuPatches));
                itemPatches.PatchAll(typeof(ItemFPSaveManagerPatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("Item Patch Failed! Info:" + ex.Message);
            }

            //Potions
            Logger.LogDebug("Potion Patch Init");
            try
            {
                Harmony potionPatches = new("000.kuborro.libraries.fp2.fp2lib.potion");
                potionPatches.PatchAll(typeof(PotionShopPatches));
                potionPatches.PatchAll(typeof(PotionMenuPatches));
                potionPatches.PatchAll(typeof(PotionFPSaveManagerPatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("Potion Patch Failed! Info:" + ex.Message);
            }

            //World Maps
            Logger.LogDebug("World Map Patch Init");
            try
            {
                Harmony mapPatches = new("000.kuborro.libraries.fp2.fp2lib.worldmap");
                mapPatches.PatchAll(typeof(MapPatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("Map Patch Failed! Info:" + ex.Message);
            }

            //Levels
            Logger.LogDebug("Stage Patch Init");
            try
            {
                Harmony stagePatches = new("000.kuborro.libraries.fp2.fp2lib.stage");
                stagePatches.PatchAll(typeof(StagePatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("Stage Patch Failed! Info:" + ex.Message);
            }

            //Save Redirection
            Logger.LogDebug("Saves Patch Init");
            try
            {
                Harmony savePatches = new("000.kuborro.libraries.fp2.fp2lib.saves");
                savePatches.PatchAll(typeof(SavePatches));
            }
            catch (Exception ex)
            {
                Logger.LogError("Save Patch Failed! VERY BAD!!!!! Info:" + ex.Message);
            }

            //Misc fixes and patches
            Logger.LogDebug("Bugfix Patch Init");
            try
            {
                Harmony generalPatches = new("000.kuborro.libraries.fp2.fp2lib.patches");
                generalPatches.PatchAll(typeof(ScreenshotFix));
                generalPatches.PatchAll(typeof(PotionSizeFix));
                generalPatches.PatchAll(typeof(ModdedPotionsFix));
            }
            catch (Exception ex)
            {
                Logger.LogError("Extras Patch Failed! Info:" + ex.Message);
            }

            Logger.LogInfo("Init done!");
        }
    }
}
