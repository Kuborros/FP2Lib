using System;
using UnityEngine;

namespace FP2Lib.Badge
{

    public enum FPBadgeType
    {
        GOLD,
        SILVER
    }

    public enum FPBadgeVisible
    {
        ALWAYS,
        HIDDEN
    }

    [Serializable]
    public class BadgeData
    {
        public string uid;
        public string name;
        public string description;
        public int id = 0;
        public FPBadgeVisible visibility;
        public FPBadgeType type;
        internal Sprite sprite;

        public BadgeData(string uid, string name, string description, FPBadgeType type, FPBadgeVisible visible, Sprite sprite)
        {
            this.uid = uid;
            this.name = name;
            this.description = description;
            this.type = type;
            this.visibility = visible;
            this.sprite = sprite;
        }

        internal static BadgeData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<BadgeData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
