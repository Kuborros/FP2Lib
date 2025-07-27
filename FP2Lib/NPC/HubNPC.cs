
using System.Collections.Generic;
using UnityEngine;

namespace FP2Lib.NPC
{
    public class HubNPC
    {
        /// <summary>
        /// Unique Identifier of the NPC
        /// </summary>
        public string UID;
        /// <summary>
        /// Name of the NPC
        /// </summary>
        public string Name;
        /// <summary>
        /// Species ID for the NPC
        /// </summary>
        public int Species;
        /// <summary>
        /// Custom species string for the character.
        /// Leave as null to use species ID instead.
        /// </summary>
        public string customSpecies = null;
        /// <summary>
        /// Hometown ID for the NPC
        /// </summary>
        public int Home;
        /// <summary>
        /// Custom home region for the NPC
        /// Leave null to use Hometown ID instead.
        /// </summary>
        public string customHome = null;
        /// <summary>
        /// Internal ID of the character
        /// </summary>
        public int ID { get; internal set; }
        /// <summary>
        /// Number of dialogue topics
        /// </summary>
        public int DialogueTopics = 1;
        /// <summary>
        /// Dictionary of Prefabs for the character. Key is the scene name, Value is the GameObject containing the prefab
        /// </summary>
        public Dictionary<string, GameObject> Prefabs = [];
        /// <summary>
        /// Instanced version of the NPC object
        /// </summary>
        public GameObject RuntimeObject { get; internal set; }

        public HubNPC(string uID, string name, int id)
        {
            UID = uID;
            Name = name;
            ID = id;
        }

        public HubNPC() { }

        internal string getNpcString()
        {
            return string.Format("{0} {1} {2}", Species.ToString("D2"), Home.ToString("D2"), Name);
        }

        internal NPCData GetNPCData()
        {
            return new NPCData(UID, ID, Name);
        }

    }
}
