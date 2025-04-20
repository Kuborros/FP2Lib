﻿using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.Stage
{
    public class StageHandler
    {
        public static Dictionary<string, CustomStage> Stages { get; internal set; } = new();
        internal static bool[] takenStageIDs = new bool[1024];
        internal static bool[] takenHubIDs = new bool[1024];

        private static string storePath, ezPath;
        private static readonly ManualLogSource StageLogSource = FP2Lib.logSource;


        internal static void InitialiseHandler()
        {

            storePath = Path.Combine(GameInfo.getProfilePath(), "StageDataStore");
            ezPath = Path.Combine(Paths.ConfigPath, "FP2LibEzStage");
            Directory.CreateDirectory(storePath);
            Directory.CreateDirectory(ezPath);

            //32 stages in base game.
            for (int i = 0; i <= 32; i++)
            {
                takenStageIDs[i] = true;
            }
            //15 hubs in base game
            for (int i = 0; i <= 14; i++)
            {
                takenHubIDs[i] = true;
            }
            //In both cases 0 is an empty slot, but it should not be touched.

            LoadFromStorage();
            //LoadEzModeStages();
        }

        //TODO: Make similar system to NPC loading from JSON
        //Needs said json, scene file, and map icon (just a .png perhabs?)
        /*
        private static void LoadEzModeStages()
        {
            foreach (string js in Directory.GetFiles(ezPath))
            {
                if (js.EndsWith(".json"))
                {

                    //EzStageData data = EzStageData.LoadFromJson(File.ReadAllText(js));
                    GameObject gameObject;
                    try
                    {
                        gameObject = AssetBundle.LoadFromFile(data.bundlePath).LoadAllAssets<GameObject>()[0];
                    }
                    catch (Exception ex)
                    {
                        StageLogSource.LogError("Failed to load EzMode Scene! " + ex.Message);
                        return;
                    }
                    //Something went wrong, abort!
                    if (gameObject == null || data.uid == null) return;

                    if (!Stages.ContainsKey(data.uid))
                    {
                        HubNPC npc = new HubNPC(data.uid, data.name, data.scene, gameObject, data.species, data.home, data.dialogue);
                        Stages.Add(data.uid, npc);
                    }
                    else if (Stages.ContainsKey(data.uid) && HubNPCs[data.UID].Prefabs.Count == 0)
                    {
                        HubNPC npc = new HubNPC(data.UID, data.name, data.scene, gameObject, data.species, data.home, data.dialogue);
                        npc.ID = HubNPCs[data.UID].ID;
                        HubNPCs[data.UID] = npc;
                    }

                }
            }
        }
        */


        public static bool RegisterStage(CustomStage stage)
        {
            if (!Stages.ContainsKey(stage.uid))
            {
                stage.id = AssignStageID(stage, stage.isHUB);
                stage.registered = true;
                Stages.Add(stage.uid, stage);
                WriteToStorage();
                return true;
            }
            else if (Stages.ContainsKey(stage.uid) && !Stages[stage.uid].registered)
            {
                //Copy over existing id first
                stage.id = Stages[stage.uid].id;
                Stages[stage.uid] = stage;
                Stages[stage.uid].id = AssignStageID(stage, stage.isHUB);
                Stages[stage.uid].registered = true;
                WriteToStorage();
                return true;
            }
            return false;
        }

        public static CustomStage getCustomStageByUid(string uid)
        {
            if (Stages.ContainsKey(uid))
                return Stages[uid];
            else return null;
        }

        public static CustomStage getCustomStageByRuntimeId(int id)
        {
            foreach (CustomStage stage in Stages.Values)
            {
                if (stage.id == id && !stage.isHUB) return stage;
            }
            return null;
        }

        public static CustomStage getCustomHubByRuntimeId(int id)
        {
            foreach (CustomStage stage in Stages.Values)
            {
                if (stage.id == id && stage.isHUB) return stage;
            }
            return null;
        }

        private static int AssignStageID(CustomStage stage, bool isHub)
        {
            //Stage already has ID and it's free
            //Since id 0 is both always taken *and* given to stages with no id, it also works as a check if stage is registered.
            if ((isHub && !takenHubIDs[stage.id]) || (!isHub && !takenStageIDs[stage.id]))
            {
                //Extend array if needed
                if (isHub)
                {
                    if (stage.id > takenHubIDs.Length)
                        takenHubIDs = FPSaveManager.ExpandBoolArray(takenHubIDs, stage.id);
                    takenHubIDs[stage.id] = true;
                    StageLogSource.LogDebug("Stored Hub ID assigned (" + stage.uid + "): " + stage.id);
                }
                else
                {
                    if (stage.id > takenStageIDs.Length)
                        takenStageIDs = FPSaveManager.ExpandBoolArray(takenStageIDs, stage.id);
                    takenStageIDs[stage.id] = true;
                    StageLogSource.LogDebug("Stored Stage ID assigned (" + stage.uid + "): " + stage.id);
                }
                return stage.id;
            }
            else
            {
                StageLogSource.LogDebug("Stage with unassigned ID registered! Running assignment process for " + stage.uid);
                //Iterate over array, assign first non-taken slot
                if (isHub)
                {
                    for (int i = 15; i < takenHubIDs.Length; i++)
                    {
                        //First slot with false = empty space
                        if (!takenHubIDs[i])
                        {
                            stage.id = i;
                            takenHubIDs[i] = true;
                            StageLogSource.LogDebug("Hub ID assigned:" + stage.id);
                            return stage.id;
                        }
                    }
                }
                else
                {
                    for (int i = 33; i < takenStageIDs.Length; i++)
                    {
                        //First slot with false = empty space
                        if (!takenStageIDs[i])
                        {
                            stage.id = i;
                            takenStageIDs[i] = true;
                            StageLogSource.LogDebug("Stage ID assigned:" + stage.id);
                            return stage.id;
                        }
                    }
                }
            }
            StageLogSource.LogWarning("Scene: " + stage.uid + " failed ID assignment! That's bad!");
            return 0;
        }


        private static void LoadFromStorage()
        {
            foreach (string js in Directory.GetFiles(storePath))
            {
                if (js.EndsWith(".json"))
                {
                    StageData data = StageData.LoadFromJson(File.ReadAllText(js));
                    StageLogSource.LogDebug("Loaded Stage from storage: " + data.name + "(" + data.uid + ")");
                    if (!Stages.ContainsKey(data.uid))
                    {
                        Stages.Add(data.uid, new CustomStage(data.uid, data.name, data.isHUB, data.id));
                    }
                }
            }
        }

        internal static void WriteToStorage()
        {
            foreach (CustomStage stage in Stages.Values)
            {
                string json = stage.getStageData().WriteToJson();
                try
                {
                    byte[] bytes = new UTF8Encoding().GetBytes(json);
                    using FileStream fileStream = new(string.Concat(new object[]
                    {
                    storePath,
                    "/",
                    stage.uid,
                    ".json"
                    }), FileMode.Create, FileAccess.Write, FileShare.Read, bytes.Length, FileOptions.WriteThrough);
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Flush();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

    }
}
