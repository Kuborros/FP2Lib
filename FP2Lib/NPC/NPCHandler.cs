using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.NPC
{
    public class NPCHandler
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

        internal static Dictionary<string, HubNPC> HubNPCs = new();
        private static string storePath;
        ManualLogSource NPCLogSource = new ManualLogSource("FP2Lib-NPC");

        public NPCHandler()
        {
            storePath = Path.Combine(Paths.ConfigPath, "NPCLibStore");
            Directory.CreateDirectory(storePath);

            loadFromStorage();
        }

        public bool registerNPC(string uID, string Name, string Scene, GameObject Prefab, FPCharacterSpecies Species = FPCharacterSpecies.UNKNOWN, FPCharacterHome Home = FPCharacterHome.UNKNOWN, int DialogueTopics = 1)
        {
            return registerNPC(uID, Name, Scene, Prefab, (int)Species, (int)Home, DialogueTopics);
        }

        public bool registerNPC(string uID, string Name, string Scene, GameObject Prefab, int Species = 0, int Home = 0, int DialogueTopics = 1)
        {
            if (!HubNPCs.ContainsKey(uID))
            {
                HubNPC npc = new HubNPC(uID, Name, Scene, Prefab, Species, Home, DialogueTopics);
                HubNPCs.Add(uID, npc);
                return true;
            }
            else if (HubNPCs.ContainsKey(uID) && HubNPCs[uID].Prefab == null)
            {
                HubNPC npc = new HubNPC(uID, Name, Scene, Prefab, Species, Home, DialogueTopics);
                npc.ID = HubNPCs[uID].ID;
                HubNPCs[uID] = npc;
                return true;
            }
            else return false;
        }

        public HubNPC getNPCByUID(string UID)
        {
            return HubNPCs[UID];
        }

        private void loadFromStorage()
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

        internal void writeToStorage()
        {
            foreach (HubNPC npc in HubNPCs.Values)
            {
                string json = npc.GetNPCData().WriteToJson();
                try
                {
                    byte[] bytes = new UTF8Encoding().GetBytes(json);
                    using (FileStream fileStream = new FileStream(string.Concat(new object[]
                    {
                    storePath,
                    "/",
                    npc.UID,
                    ".json"
                    }), FileMode.Create, FileAccess.Write, FileShare.Read, bytes.Length, FileOptions.WriteThrough))
                    {
                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Flush();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}

