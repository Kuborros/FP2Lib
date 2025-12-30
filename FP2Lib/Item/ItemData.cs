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

    [System.Serializable]
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
        public string descriptionLilac;
        /// <summary>
        /// Description of the item when playing as Carol.
        /// Leave empty to use default description.
        /// </summary>
        public string descriptionCarol;
        /// <summary>
        /// Description of the item when playing as Milla.
        /// Leave empty to use default description.
        /// </summary>
        public string descriptionMilla;
        /// <summary>
        /// Description of the item when playing as Neera.
        /// Leave empty to use default description.
        /// </summary>
        public string descriptionNeera;
        /// <summary>
        /// Description of the item when playing as a custom character.
        /// Key is the character UID, value is the description.
        /// </summary>
        public Dictionary<string,string> descriptionCustom = new Dictionary<string,string>();

        /// <summary>
        /// Sprite shown in inventory, item list, and equipment slots.
        /// </summary>
        public Sprite sprite;
        /// <summary>
        /// How many starcards needed to unlock the item.
        /// </summary>
        public int starCards;
        /// <summary>
        /// Price of the item in Gold Gems.
        /// </summary>
        public int goldGemsPrice;
        /// <summary>
        /// Gem Bonus granted at the end of the stage.
        /// </summary>
        public float gemBonus = 0f;
        /// <summary>
        /// Which shop should the item appear in
        /// </summary>
        public IAddToShop shopLocation;

        internal int id = 0;
        //POTION SELLER ITEM
        internal bool specialWorkaround = false;

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
