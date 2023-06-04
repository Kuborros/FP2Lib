using UnityEngine;

namespace FP2Lib.Player
{
    [System.Serializable]
    internal class CharacterData
    {

        public string UID;
        public int runtimeID;
        public string name;

        internal CharacterData(string uid, int runtimeID, string name)
        {
            this.UID = uid;
            this.runtimeID = runtimeID;
            this.name = name;
        }

        internal static CharacterData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<CharacterData>(json);
        }
        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

    }
}
