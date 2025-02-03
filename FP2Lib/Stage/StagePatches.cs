using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Stage
{
    internal class StagePatches
    {
        //TODO: Where needed, append logic for Classic Mode world map.

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
                    if (stage.storyFlag >= FPSaveManager.storyFlag.Length)
                        FPSaveManager.storyFlag = FPSaveManager.ExpandByteArray(FPSaveManager.storyFlag, stage.storyFlag);
                }
                StageHandler.WriteToStorage();
            }
        }

        //Extend time records array if needed
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "Awake", MethodType.Normal)]
        static void PatchFPSaveManagerAwake(ref int[] ___timeRecord)
        {
            //Don't run code if there is nothing to add.
            if (StageHandler.Stages.Count > 0)
            {
                foreach (CustomStage stage in StageHandler.Stages.Values)
                {
                    if (stage.id > ___timeRecord.Length)
                    {
                        FPSaveManager.ExpandIntArray(___timeRecord, stage.id + 1);
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
            if (hub > 14 && __result.IsNullOrWhiteSpace())
            {
                CustomStage customStage = StageHandler.getCustomHubByRuntimeId(hub);
                if (customStage != null)
                    __result = customStage.name;
            }
        }
    }
}
