using UnityEngine;

namespace FP2Lib.Vinyl
{
    public enum VAddToShop {
        None,
        ClassicOnly,
        Naomi, //Battlesphere
        Digo, //Adventure Square
        Fawnstar, //Paradise Prime
        All //All of the above
    }

    [System.Serializable]
    public class VinylData
    {
        public string uid;
        public int id = 0;
        public string name;
        internal AudioClip audioClip;
        public int starCards;
        public int crystalsPrice;
        public VAddToShop shopLocation;

        public VinylData(string uid ,string name, AudioClip audioClip, VAddToShop shop, int starCards, int crystalsPrice)
        {
            this.uid = uid;
            this.name = name;
            this.audioClip = audioClip;
            this.shopLocation = shop;
            this.starCards = starCards;
            this.crystalsPrice = crystalsPrice;
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
