﻿using UnityEngine;

namespace FP2Lib.NPC
{
    [System.Serializable]
    internal class NPCData
    {
        public string UID;
        public int runtimeID;
        public string name;

        internal NPCData(string uid, int runtimeID, string name)
        {
            this.UID = uid;
            this.runtimeID = runtimeID;
            this.name = name;
        }

        internal static NPCData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<NPCData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
