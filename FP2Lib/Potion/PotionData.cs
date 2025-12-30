using UnityEngine;

namespace FP2Lib.Potion
{

    public enum IAddToShop
    {
        None,
        ClassicOnly,
        Milla //All Milla locations
    }

    [System.Serializable]
    internal class PotionData
    {
        /// <summary>
        /// Unique identifier for your potion
        /// </summary>
        public string uid;
        /// <summary>
        /// 
        /// </summary>
        public string name;
        /// <summary>
        /// Item description
        /// </summary>
        public string description;
        /// <summary>
        /// Inventory sprite (shops and item list)
        /// </summary>
        public Sprite inventorySprite;
        /// <summary>
        /// Sprite for the middle slots in the bottle preview.
        /// </summary>
        public Sprite spriteBottleMid;
        /// <summary>
        /// Sprite for the top (last) slot in the bottle.
        /// </summary>
        public Sprite spriteBottleTop;
        /// <summary>
        /// Sprite for the bottom (first) slot in the bottle.
        /// </summary>
        public Sprite spriteBottleBottom;
        /// <summary>
        /// How many Star Cards are needed to unlock it in shops.
        /// </summary>
        public int starCards;
        /// <summary>
        /// Price in Gold Gems.
        /// </summary>
        public int goldGemsPrice;

        internal int id = 0;
        //POTION SELLER POTION
        internal bool specialWorkaround = false;

        internal static PotionData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<PotionData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
