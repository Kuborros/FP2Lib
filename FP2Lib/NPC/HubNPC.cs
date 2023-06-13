
using System.Collections.Generic;
using UnityEngine;

namespace FP2Lib.NPC
{
    public class HubNPC
    {
        public bool registered = false;
        public string UID;
        public string Name;
        public int Species;
        public int Home;
        public int ID;
        public int DialogueTopics = 1;
        public Dictionary<string,GameObject> Prefabs = new();
        public GameObject RuntimeObject;

        public HubNPC(string uID, string name, string scene, GameObject prefab, int species = 0, int home = 0, int topics = 1)
        {
            this.UID = uID;
            this.Name = name;
            Prefabs.Add(scene,prefab);
            this.Species = species;
            this.Home = home;
            this.DialogueTopics = topics;
            this.registered = true;
        }

        public HubNPC(string uID, string name, Dictionary<string,GameObject> prefabs, int species = 0, int home = 0, int topics = 1)
        {
            this.UID = uID;
            this.Name = name;
            this.Prefabs = prefabs;
            this.Species = species;
            this.Home = home;
            this.DialogueTopics = topics;
            this.registered = true;
        }

        public HubNPC(string uID, string name, int id)
        {
            this.UID = uID;
            this.Name = name;
            this.ID = id;
        }

        internal string getNpcString()
        {
            return string.Format("{0} {1} {2}",Species.ToString("D2"),Home.ToString("D2"), Name);
        }

        internal NPCData GetNPCData()
        {
            return new NPCData(UID,ID,Name);
        }

    }
}
