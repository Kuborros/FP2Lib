using BepInEx;
using BepInEx.Logging;
using FP2Lib.Vinyl;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.Stage
{
    internal class StagePatches
    {
        private static readonly ManualLogSource StageLogSource = FP2Lib.logSource;
        //Patch World Map "Go to level" menu. This *actualy* handles whole logic for sending you to right level.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMapConfirm), "Start", MethodType.Normal)]
        static void PatchMenuWorldMapConfirm(ref string[] ___hubSceneToLoad, ref string[] ___sceneToLoad, ref FPHubNPC[] ___shopkeepers, ref GameObject[] ___shopMenus, ref Sprite[] ___stageIcon, ref Sprite[] ___hubIcon)
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                //Load in custom stages.
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    //Only registered stages should be loaded.
                    if (stage.id != 0)
                    {
                        //Extend hub array.
                        if (stage.isHUB)
                        {
                            for (int i = ___hubSceneToLoad.Length; i < stage.id + 1; i++)
                            {
                                ___hubSceneToLoad = ___hubSceneToLoad.AddToArray(string.Empty);
                            }
                            for (int i = ___shopkeepers.Length; i < stage.id + 1; i++)
                            {
                                ___shopkeepers = ___shopkeepers.AddToArray(null);
                            }
                            for (int i = ___shopMenus.Length; i < stage.id + 1; i++)
                            {
                                ___shopMenus = ___shopMenus.AddToArray(null);
                            }
                            for (int i = ___hubIcon.Length; i < stage.id + 1; i++)
                            {
                                ___hubIcon = ___hubIcon.AddToArray(null);
                            }
                        }
                        //Normal stages
                        else
                        {
                            for (int i = ___sceneToLoad.Length; i < stage.id + 1; i++)
                            {
                                ___sceneToLoad = ___sceneToLoad.AddToArray(null);
                            }
                            for (int i = ___stageIcon.Length; i < stage.id + 1; i++)
                            {
                                ___stageIcon = ___stageIcon.AddToArray(null);
                            }
                        }
                        //Add the stages
                        if (stage.registered)
                        {
                            if (stage.isHUB)
                            {
                                ___hubSceneToLoad[stage.id] = stage.sceneName;
                                ___shopkeepers[stage.id] = stage.shopkeeper;
                                ___shopMenus[stage.id] = stage.quickShop;
                                ___hubIcon[stage.id] = stage.preview;
                            }
                            else
                            {
                                ___sceneToLoad[stage.id] = stage.sceneName;
                                ___stageIcon[stage.id] = stage.preview;
                            }
                        }
                    }
                }
            }
        }

        //Extend the story flag array if custom stage has id higher than the array size
        //Also set Item and Vinyl IDs from their uids
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad()
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                //Load in custom stages.
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    //Create story flags as needed
                    if (stage.storyFlag >= FPSaveManager.storyFlag.Length)
                        FPSaveManager.storyFlag = FPSaveManager.ExpandByteArray(FPSaveManager.storyFlag, stage.storyFlag);

                    //If custom Vinyl is defined, set it here
                    stage.vinylID = GetStageMusic(stage);
                    //Same for items
                    stage.itemID = GetStageItem(stage);
                }
                StageHandler.WriteToStorage();
            }
        }

        private static FPMusicTrack GetStageMusic(CustomStage stage)
        {
            //Check if a custom uid is even set
            if (!stage.vinylUID.IsNullOrWhiteSpace())
            {
                StageLogSource.LogDebug("Linking vinyl with uid: " + stage.vinylUID + " for stage:" + stage.uid);
                VinylData vinyl = VinylHandler.GetVinylDataByUid(stage.vinylUID);
                //Is the uid pointing to a vinyl that exists
                if (vinyl != null)
                {
                    StageLogSource.LogDebug("Got id:" + vinyl.id + " for track name: " + vinyl.name);
                    return (FPMusicTrack)vinyl.id;
                }
            }
            return stage.vinylID;
        }

        private static FPPowerup GetStageItem(CustomStage stage)
        {
            if (!stage.itemUID.IsNullOrWhiteSpace())
            {
                StageLogSource.LogDebug("Linking item with uid: " + stage.itemUID + " for stage:" + stage.uid);
                //TODO: Implement code once ItemHandler is done
                StageLogSource.LogDebug("Item linking is not available yet!");
                return FPPowerup.NONE;
            }
            else
            {
                return stage.itemID;
            }
        }

        //Extend time records array if needed
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "Awake", MethodType.Normal)]
        static void PatchFPSaveManagerAwake(ref int[] ___timeRecord, ref byte[] ___timeRank)
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    if (stage.id >= ___timeRecord.Length)
                    {
                        FPSaveManager.ExpandIntArray(___timeRecord, stage.id + 1);
                    }
                    if (stage.id >= ___timeRank.Length)
                    {
                        FPSaveManager.ExpandByteArray(___timeRank, stage.id + 1);
                    }
                }
            }
        }

        //Extend time records to include the stage icon and collectibles
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "Start", MethodType.Normal)]
        static void PatchMenuGlobalPauseRecords(ref Sprite[] ___stageIconSprites, ref FPMusicTrack[] ___hudVinylID, ref FPPowerup[] ___hudChestItem)
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    if (stage.id >= ___stageIconSprites.Length)
                    {
                        for (int i = ___stageIconSprites.Length; i <= (stage.id); i++)
                            //Default icon will use the "?" from Weapon's Core (in case stage lacks the needed sprite, or we have id hole.
                            ___stageIconSprites = ___stageIconSprites.AddToArray(___stageIconSprites[30]);
                    }
                    if (stage.id >= ___hudVinylID.Length)
                    {
                        for (int i = ___hudVinylID.Length; i <= (stage.id); i++)
                            //Default is no music.
                            ___hudVinylID = ___hudVinylID.AddToArray(FPMusicTrack.NONE);
                    }
                    if (stage.id >= ___hudChestItem.Length)
                    {
                        for (int i = ___hudChestItem.Length; i <= (stage.id); i++)
                            //Default is no item.
                            //Fun fact! On old 1.0.0 era save files, Bakunawa Chase lacks this value which causes funny bugs!
                            ___hudChestItem = ___hudChestItem.AddToArray(FPPowerup.NONE);
                    }
                    ___stageIconSprites[stage.id] = stage.preview;
                    ___hudVinylID[stage.id] = stage.vinylID;
                    ___hudChestItem[stage.id] = stage.itemID;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCredits), "Start", MethodType.Normal)]
        static void PatchMenuCreditsStart(ref int[] ___stageOrder, ref Sprite[] ___spriteStageIcon)
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    if (stage.id >= ___spriteStageIcon.Length)
                    {
                        for (int i = ___spriteStageIcon.Length; i <= (stage.id); i++)
                            //Default icon will use the "?" from Weapon's Core (in case stage lacks the needed sprite, or we have id hole.
                            ___spriteStageIcon = ___spriteStageIcon.AddToArray(___spriteStageIcon[30]);
                    }
                    if (!___stageOrder.Contains(stage.id))
                    {
                        ___stageOrder = ___stageOrder.AddToArray(stage.id);
                        ___spriteStageIcon[___stageOrder.Length - 1] = stage.preview;
                    }
                }
            }
        }

        //Set the proper stageID for the stage
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
        static void PatchFPStageStart(ref int ___stageID)
        {
            //This should *just work*, as the value is set by MenuWorldMapConfirm miliseconds before this
            ___stageID = FPSaveManager.debugStageID;
        }

        //Add stage name
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetStageName", MethodType.Normal)]
        static void PatchGetStageName(int stage, ref string __result)
        {
            if (stage > 32 && __result.IsNullOrWhiteSpace())
            {
                CustomStage customStage = StageHandler.getCustomStageByRuntimeId(stage);
                if (customStage != null)
                    __result = customStage.name;
            }
        }

        //Add stage par time
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetStageParTime", MethodType.Normal)]
        static void PatchGetStageParTime(int stage, ref int __result)
        {
            if (stage > 32)
            {
                CustomStage customStage = StageHandler.getCustomStageByRuntimeId(stage);
                if (customStage != null)
                    __result = customStage.parTime;
            }
        }

        //Add stage's story flag
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetStoryFlag", MethodType.Normal)]
        static void PatchGetStageStoryFlag(int stage, ref int __result)
        {
            if (stage > 32)
            {
                CustomStage customStage = StageHandler.getCustomStageByRuntimeId(stage);
                if (customStage != null)
                    __result = customStage.storyFlag;
            }
        }

        //Add hub name
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetHubName", MethodType.Normal)]
        static void PatchGetHubName(int hub, ref string __result)
        {
            //For reasons unknown to Man, the result can be "0" on missing Hubs
            if (hub > 14 && (__result == "0" || __result.IsNullOrWhiteSpace()))
            {
                CustomStage customStage = StageHandler.getCustomHubByRuntimeId(hub);
                if (customStage != null)
                    __result = customStage.name;
            }
        }

        //Patch the map to add dialog for extra hubIDs
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "Start", MethodType.Normal)]
        static void PatchMenuWorldMap(ref FPMapDialog[] ___hubDialog)
        {
            int id = 14;
            //Get the highest hubID.
            foreach (CustomStage stage in StageHandler.Stages.Values)
            {
                if (stage.isHUB && stage.id > id)
                {
                    id = stage.id;
                }
            }
            //If our ID is out of the array's bounds extend it
            //Should be non-destructive in case a mod adds their own dialogue
            if (id >= ___hubDialog.Length)
            {
                FPMapDialog[] array2 = new FPMapDialog[id + 1];
                //Copy data over
                for (int i = 0; i < ___hubDialog.Length; i++)
                {
                    array2[i] = ___hubDialog[i];
                }
                //Any nulls need a clean empty object.
                for (int i = 0; i < array2.Length; i++)
                {
                    if (array2[i] == null)
                        array2[i] = new FPMapDialog { dialogSequence = [], sceneID = 0, storyFlagValue = 0, disableAtStoryFlag = 0 };
                }
                ___hubDialog = array2;
            }
        }
    }
}
