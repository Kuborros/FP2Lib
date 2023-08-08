using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.NPC
{

    public enum FPCharacterSpecies
    {
        UNKNOWN = 0,
        CAT = 1,
        BIRD = 2,
        BEAR = 3,
        PANDA = 4,
        RED_PANDA = 5,
        MONKEY = 6,
        DOG = 7,
        DEER = 8,
        FOX = 9,
        RABBIT = 10,
        DRAGON = 11,
        BAT = 12,
        BOAR = 13,
        OTTER = 14,
        RACOON = 15,
        GOAT = 16,
        HEDGEHOG = 17,
        REPTILIAN = 18,
        //Number 19 is missing in the game
        MOUSE = 20,
        FERRET = 21,
        PLANT = 22,
        ROBOT = 23,
        PANGOLIN = 24,
    }

    public enum FPCharacterHome
    {
        UNKNOWN = 0,
        SHANG_TU = 1,
        SHANG_MU = 2,
        SHUIGANG = 3,
        PARUSA = 4
    }


    public static class NPCHandler
    {
        internal static Dictionary<string, HubNPC> HubNPCs = new();
        private static string storePath, ezPath;
        private static readonly ManualLogSource NPCLogSource = FP2Lib.logSource;


        internal static void InitialiseHandler()
        {

            storePath = Path.Combine(Paths.ConfigPath, "NPCLibStore");
            ezPath = Path.Combine(Paths.ConfigPath, "NPCLibEzNPC");
            Directory.CreateDirectory(storePath);
            Directory.CreateDirectory(ezPath);

            LoadFromStorage();
            LoadEzModeNPC();
        }

        public static bool RegisterNPC(string uID, string Name, string Scene, GameObject Prefab, FPCharacterSpecies Species = FPCharacterSpecies.UNKNOWN, FPCharacterHome Home = FPCharacterHome.UNKNOWN, int DialogueTopics = 1)
        {
            return RegisterNPC(uID, Name, Scene, Prefab, (int)Species, (int)Home, DialogueTopics);
        }

        public static bool RegisterNPC(string uID, string Name, string Scene, GameObject Prefab, int Species = 0, int Home = 0, int DialogueTopics = 1)
        {
            if (!HubNPCs.ContainsKey(uID))
            {
                HubNPC npc = new HubNPC(uID, Name, Scene, Prefab, Species, Home, DialogueTopics);
                HubNPCs.Add(uID, npc);
                return true;
            }
            else if (HubNPCs.ContainsKey(uID) && HubNPCs[uID].Prefabs.Count == 0)
            {
                HubNPC npc = new HubNPC(uID, Name, Scene, Prefab, Species, Home, DialogueTopics);
                npc.ID = HubNPCs[uID].ID;
                HubNPCs[uID] = npc;
                return true;
            }
            else return false;
        }

        public static bool RegisterNPCMultiScene(string uID, string Name, Dictionary<string, GameObject> Prefabs, FPCharacterSpecies Species = FPCharacterSpecies.UNKNOWN, FPCharacterHome Home = FPCharacterHome.UNKNOWN, int DialogueTopics = 1)
        {
            return RegisterNPCMultiScene(uID, Name, Prefabs, (int)Species, (int)Home, DialogueTopics);
        }

        public static bool RegisterNPCMultiScene(string uID, string Name, Dictionary<string, GameObject> Prefabs, int Species = 0, int Home = 0, int DialogueTopics = 1)
        {
            if (!HubNPCs.ContainsKey(uID))
            {
                HubNPC npc = new HubNPC(uID, Name, Prefabs, Species, Home, DialogueTopics);
                HubNPCs.Add(uID, npc);
                return true;
            }
            else if (HubNPCs.ContainsKey(uID) && HubNPCs[uID].Prefabs.Count == 0)
            {
                HubNPC npc = new HubNPC(uID, Name, Prefabs, Species, Home, DialogueTopics);
                npc.ID = HubNPCs[uID].ID;
                HubNPCs[uID] = npc;
                return true;
            }
            else return false;
        }


        public static HubNPC GetNPCByUID(string UID)
        {
            return HubNPCs[UID];
        }


        private static void LoadEzModeNPC()
        {
            foreach (string js in Directory.GetFiles(ezPath))
            {
                if (js.EndsWith(".json"))
                {
                    EzModeData data = EzModeData.LoadFromJson(File.ReadAllText(js));
                    GameObject gameObject;
                    //Console.WriteLine("Loaded EzMode NPC: " + data.name + "(" + data.UID + ")");

                    try
                    {
                        gameObject = AssetBundle.LoadFromFile(data.bundlePath).LoadAllAssets<GameObject>()[0];
                    } 
                    catch (Exception ex)
                    {
                        NPCLogSource.LogError("Failed to load EzMode AssetBundle! " + ex.Message);
                        return;
                    }
                    //Something went wrong, abort!
                    if (gameObject == null || data.UID == null) return;


                    if (!HubNPCs.ContainsKey(data.UID))
                    {
                        HubNPC npc = new HubNPC(data.UID, data.name, data.scene, gameObject, data.species, data.home, data.dialogue);
                        HubNPCs.Add(data.UID, npc);
                    }
                    else if (HubNPCs.ContainsKey(data.UID) && HubNPCs[data.UID].Prefabs.Count == 0)
                    {
                        HubNPC npc = new HubNPC(data.UID, data.name, data.scene, gameObject, data.species, data.home, data.dialogue);
                        npc.ID = HubNPCs[data.UID].ID;
                        HubNPCs[data.UID] = npc;
                    }
                }
            }
        }

        private static void LoadFromStorage()
        {
            foreach (string js in Directory.GetFiles(storePath))
            {
                if (js.EndsWith(".json"))
                {
                    NPCData data = NPCData.LoadFromJson(File.ReadAllText(js));
                    NPCLogSource.LogDebug("Loaded NPC from storage: " + data.name + "(" + data.UID + ")");
                    if (!HubNPCs.ContainsKey(data.UID))
                    {
                        HubNPC npc = new HubNPC(data.UID, data.name, data.runtimeID);
                        HubNPCs.Add(data.UID, npc);
                    }
                }
            }
        }

        internal static void WriteToStorage()
        {
            foreach (HubNPC npc in HubNPCs.Values)
            {
                string json = npc.GetNPCData().WriteToJson();
                try
                {
                    byte[] bytes = new UTF8Encoding().GetBytes(json);
                    using FileStream fileStream = new(string.Concat(new object[]
                    {
                    storePath,
                    "/",
                    npc.UID,
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

