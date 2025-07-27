using UnityEngine;

namespace FP2Lib.Item
{
    public class ItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public string uid;
        /// <summary>
        /// 
        /// </summary>
        public string name;
        /// <summary>
        /// 
        /// </summary>
        //TODO: Maybe separate descriptions for each char?
        public string description;
        /// <summary>
        /// 
        /// </summary>
        public Sprite sprite;
        /// <summary>
        /// 
        /// </summary>
        public int starCards;
        /// <summary>
        /// 
        /// </summary>
        public int goldGemsPrice;

        internal int id = 0;
    }
}
