using System;
using UnityEngine;

namespace FP2Lib.Map
{
    [Serializable]
    public class MapData
    {
        /// <summary>
        /// Unique Identifier of the world map.
        /// </summary>
        public string uid;
        /// <summary>
        /// Internal ID used in the save/level data.
        /// </summary>
        public int id = 0;
        /// <summary>
        /// Name of the Map.
        /// </summary>
        public string name;
        /// <summary>
        /// Disables an extra background object which acts as background colour on some maps.
        /// It uses a sprite resembling daytime water, and does not look good on nightime maps with water distortion, where it can leak trough
        /// </summary>
        public bool disableBackingWater;
        /// <summary>
        /// GameObject prefab containing the map itself.
        /// </summary>
        internal GameObject prefab;



        public MapData(string uid, string name, GameObject prefab)
        {
            this.uid = uid;
            this.name = name;
            this.prefab = prefab;
        }

        internal static MapData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<MapData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
