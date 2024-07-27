using UnityEngine;

namespace FP2Lib.Vinyl
{
    public enum VAddToShop {
        None,
        ClassicOnly,
        Naomi, //Battlesphere
        Digo, //Adventure Square
        Fawnstar //Paradise Prime
    }

    [System.Serializable]
    public class VinylData
    {
        public int id;
        public string name;
        internal AudioClip audioClip;
        public VAddToShop shopLocation;

        public VinylData(string name, AudioClip audioClip, int id, VAddToShop shop)
        {
            this.name = name;
            this.audioClip = audioClip;
            this.id = id;
            this.shopLocation = shop;
        }

        internal static VinylData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<VinylData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
