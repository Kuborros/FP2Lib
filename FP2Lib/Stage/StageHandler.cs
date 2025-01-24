using BepInEx.Logging;
using BepInEx;
using FP2Lib.Tools;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;

namespace FP2Lib.Stage
{
    internal class StageHandler
    {
        internal static Dictionary<string, CustomStage> Stages = new();
        private static string storePath, ezPath;
        private static readonly ManualLogSource StageLogSource = FP2Lib.logSource;


        internal static void InitialiseHandler()
        {

            storePath = Path.Combine(GameInfo.getProfilePath(), "StageDataStore");
            ezPath = Path.Combine(Paths.ConfigPath, "FP2LibEzStage");
            Directory.CreateDirectory(storePath);
            Directory.CreateDirectory(ezPath);

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
                        Stages.Add(data.uid, new CustomStage(data.uid,data.name,data.id));
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
