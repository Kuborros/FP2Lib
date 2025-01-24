using System;
using UnityEngine;

namespace FP2Lib.Stage
{
    [Serializable]
    internal class StageData
    {

        /// <summary>
        /// Unique Identifier string for the stage.
        /// </summary>
        public string uid;
        /// <summary>
        /// Name of the stage.
        /// </summary>
        public string name;
        /// <summary>
        /// Stage ID used in save files and world map warps
        /// </summary>
        public int id = 0;

        public StageData() { }
        internal StageData(string uid, string name, int id)
        {
            this.uid = uid;
            this.name = name;
            this.id = id;
        }

        internal static StageData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<StageData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
