using System;
using System.Collections.Generic;
using UnityEngine;

namespace FP2Lib.Potion
{

    public enum PAddToShop
    {
        None,
        ClassicOnly,
        Milla //All Milla locations
    }

    [System.Serializable]
    public class PotionData
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
        /// Potion description, if no character specific one is found.
        /// </summary>
        public string description;
        /// <summary>
        /// Description of the potion when playing as Lilac.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionLilac;
        /// <summary>
        /// Description of the potion when playing as Carol.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionCarol;
        /// <summary>
        /// Description of the potion when playing as Milla.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionMilla;
        /// <summary>
        /// Description of the potion when playing as Neera.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionNeera;
        /// <summary>
        /// Description of the potion when playing as a custom character.
        /// Key is the character UID, value is the description.
        /// </summary>
        [NonSerialized]
        public Dictionary<string, string> descriptionCustom = new Dictionary<string, string>();

        /// <summary>
        /// The effect amount per one potion slot taken. Can be full number, can be float, whatever fits the text below.
        /// </summary>
        public float effectPercentage = 0f;
        /// <summary>
        /// The text following the calculated effect percentage
        /// </summary>
        [NonSerialized]
        public string effect = " of nothing. The mod is not installed, silly!";
        /// <summary>
        /// Inventory sprite (shops and item list)
        /// </summary>
        [NonSerialized]
        public Sprite inventorySprite;
        /// <summary>
        /// Sprite for the middle slots in the bottle preview.
        /// </summary>
        [NonSerialized]
        public Sprite spriteBottleMid;
        /// <summary>
        /// Sprite for the top (last) slot in the bottle.
        /// </summary>
        [NonSerialized]
        public Sprite spriteBottleTop;
        /// <summary>
        /// Sprite for the bottom (first) slot in the bottle.
        /// </summary>
        [NonSerialized]
        public Sprite spriteBottleBottom;
        /// <summary>
        /// How many Star Cards are needed to unlock it in shops.
        /// </summary>
        [NonSerialized]
        public int starCards;
        /// <summary>
        /// Price in Gold Gems.
        /// </summary>
        [NonSerialized]
        public int goldGemsPrice;
        /// <summary>
        /// Which shop should the potion appear in
        /// </summary>
        [NonSerialized]
        public PAddToShop shopLocation = PAddToShop.None;

        public int id = 0;

        public PotionData()
        {
        }

        public PotionData(string uid, string name, string description, Sprite inventorySprite, Sprite spriteBottleMid, Sprite spriteBottleTop, Sprite spriteBottleBottom, int starCards, int goldGemsPrice, PAddToShop shopLocation)
        {
            this.uid = uid;
            this.name = name;
            this.description = description;
            this.inventorySprite = inventorySprite;
            this.spriteBottleMid = spriteBottleMid;
            this.spriteBottleTop = spriteBottleTop;
            this.spriteBottleBottom = spriteBottleBottom;
            this.starCards = starCards;
            this.goldGemsPrice = goldGemsPrice;
            this.shopLocation = shopLocation;
        }

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
