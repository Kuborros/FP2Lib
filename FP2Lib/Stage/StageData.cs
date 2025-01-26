using System;
using UnityEngine;

namespace FP2Lib.Stage
{
    [Serializable]
    public class StageData
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
        /// <summary>
        /// Is the stage a HUB location?
        /// </summary>
        public bool isHUB;
        /// <summary>
        /// Record time.
        /// </summary>
        public int bestTime;

        public StageData() { }
        public StageData(string uid, string name,bool isHUB, int id)
        {
            this.uid = uid;
            this.name = name;
            this.isHUB = isHUB;
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
