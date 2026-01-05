using System;
using System.Collections.Generic;
using UnityEngine;

namespace FP2Lib.Item
{

    public enum IAddToShop
    {
        None,
        ClassicOnly,
        Yuni, //Old Temple
        Blake, //Battlesphere
        Chloe, //Multiple places
        Florin //Paradise Prime
    }

    public enum PAddToShop
    {
        None,
        ClassicOnly,
        Milla //All Milla locations
    }

    [Serializable]
    public class ItemData
    {
        /// <summary>
        /// Unique identifier of the item
        /// </summary>
        public string uid;
        /// <summary>
        /// Name of the item
        /// </summary>
        public string name;

        /// <summary>
        /// Description of the item - Generic edition.
        /// Used when others are not set, and for custom characters.
        /// </summary>
        public string descriptionGeneric;
        /// <summary>
        /// Description of the item when playing as Lilac.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionLilac;
        /// <summary>
        /// Description of the item when playing as Carol.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionCarol;
        /// <summary>
        /// Description of the item when playing as Milla.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionMilla;
        /// <summary>
        /// Description of the item when playing as Neera.
        /// Leave empty to use default description.
        /// </summary>
        [NonSerialized]
        public string descriptionNeera;
        /// <summary>
        /// Description of the item when playing as a custom character.
        /// Key is the character UID, value is the description.
        /// </summary>
        [NonSerialized]
        public Dictionary<string,string> descriptionCustom = [];

        /// <summary>
        /// Sprite shown in inventory, item list, and equipment slots.
        /// </summary>
        [NonSerialized]
        internal Sprite sprite;
        /// <summary>
        /// How many starcards needed to unlock the item.
        /// </summary>
        [NonSerialized]
        public int starCards;
        /// <summary>
        /// Price of the item in Gold Gems.
        /// </summary>
        [NonSerialized]
        public int goldGemsPrice;
        /// <summary>
        /// Gem Bonus granted at the end of the stage.
        /// </summary>
        [NonSerialized]
        public float gemBonus = 0f;
        /// <summary>
        /// Which shop should the item appear in
        /// </summary>
        public IAddToShop itemShopLocation = IAddToShop.None;


        /// <summary>
        /// Set for items which exist as equivalent to a registered potion.
        /// This enables the following fields.
        /// </summary>
        public bool isPotion = false;
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
        /// Sprite for the middle slots in the bottle preview.
        /// </summary>
        [NonSerialized]
        public Sprite spriteBottleMid = null;
        /// <summary>
        /// Sprite for the top (last) slot in the bottle.
        /// </summary>
        [NonSerialized]
        public Sprite spriteBottleTop = null;
        /// <summary>
        /// Sprite for the bottom (first) slot in the bottle.
        /// </summary>
        [NonSerialized]
        public Sprite spriteBottleBottom = null;
        /// <summary>
        /// Which shop should the potion appear in
        /// </summary>
        [NonSerialized]
        public PAddToShop potionShopLocation = PAddToShop.None;


        public int itemID = 0;
        public int potionID = 0;

        public ItemData()
        {
        }

        //Constructor for item
        public ItemData(string uid, string name, string descriptionGeneric, string descriptionLilac, string descriptionCarol, string descriptionMilla, string descriptionNeera,
            Dictionary<string, string> descriptionCustom, Sprite sprite, int starCards, int goldGemsPrice, float gemBonus, IAddToShop itemShopLocation)
        {
            this.uid = uid;
            this.name = name;
            this.descriptionGeneric = descriptionGeneric;
            this.descriptionLilac = descriptionLilac;
            this.descriptionCarol = descriptionCarol;
            this.descriptionMilla = descriptionMilla;
            this.descriptionNeera = descriptionNeera;
            this.descriptionCustom = descriptionCustom;
            this.sprite = sprite;
            this.starCards = starCards;
            this.goldGemsPrice = goldGemsPrice;
            this.gemBonus = gemBonus;
            this.itemShopLocation = itemShopLocation;
        }

        //Constructor for item - Lite
        public ItemData(string uid, string name, string descriptionGeneric, Sprite sprite, int starCards, int goldGemsPrice, float gemBonus, IAddToShop itemShopLocation)
        {
            this.uid = uid;
            this.name = name;
            this.descriptionGeneric = descriptionGeneric;
            this.sprite = sprite;
            this.starCards = starCards;
            this.goldGemsPrice = goldGemsPrice;
            this.gemBonus = gemBonus;
            this.itemShopLocation = itemShopLocation;
        }

        //Constructor for potion
        public ItemData(string uid, string name, string descriptionGeneric, string descriptionLilac, string descriptionCarol, string descriptionMilla, string descriptionNeera,
            Dictionary<string, string> descriptionCustom, Sprite sprite, int starCards, int goldGemsPrice, bool isPotion, float effectPercentage, string effect, 
            Sprite spriteBottleMid, Sprite spriteBottleTop, Sprite spriteBottleBottom, PAddToShop potionShopLocation)
        {
            this.uid = uid;
            this.name = name;
            this.descriptionGeneric = descriptionGeneric;
            this.descriptionLilac = descriptionLilac;
            this.descriptionCarol = descriptionCarol;
            this.descriptionMilla = descriptionMilla;
            this.descriptionNeera = descriptionNeera;
            this.descriptionCustom = descriptionCustom;
            this.sprite = sprite;
            this.starCards = starCards;
            this.goldGemsPrice = goldGemsPrice;
            this.isPotion = isPotion;
            this.effectPercentage = effectPercentage;
            this.effect = effect;
            this.spriteBottleMid = spriteBottleMid;
            this.spriteBottleTop = spriteBottleTop;
            this.spriteBottleBottom = spriteBottleBottom;
            this.potionShopLocation = potionShopLocation;
        }

        public ItemData(string uid, string name, string descriptionGeneric, Sprite sprite, int starCards, int goldGemsPrice, bool isPotion, float effectPercentage, string effect,
            Sprite spriteBottleMid, Sprite spriteBottleTop, Sprite spriteBottleBottom, PAddToShop potionShopLocation)
        {
            this.uid = uid;
            this.name = name;
            this.descriptionGeneric = descriptionGeneric;
            this.sprite = sprite;
            this.starCards = starCards;
            this.goldGemsPrice = goldGemsPrice;
            this.isPotion = isPotion;
            this.effectPercentage = effectPercentage;
            this.effect = effect;
            this.spriteBottleMid = spriteBottleMid;
            this.spriteBottleTop = spriteBottleTop;
            this.spriteBottleBottom = spriteBottleBottom;
            this.potionShopLocation = potionShopLocation;
        }

        internal static ItemData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<ItemData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
