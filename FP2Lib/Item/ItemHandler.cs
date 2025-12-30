using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.Item
{
    internal class ItemHandler
    {
        private static readonly ManualLogSource ItemLogSource = FP2Lib.logSource;

        //First empty slot with Potion Seller installed is 100
        public static readonly int baseItems = 100;

        internal static Dictionary<string, ItemData> Items = [];
        internal static bool[] takenIDs = new bool[256];

        internal static void InitialiseHandler()
        {
            if (!File.Exists(GameInfo.getProfilePath() + "/ItemStore.json"))
                File.Create(GameInfo.getProfilePath() + "/ItemStore.json").Close();

            //Mark first 100 item ids as taken by base game + Potion Seller.
            for (int i = 0; i < baseItems; i++)
            {
                takenIDs[i] = true;
            }

            LoadFromStorage();
        }

        /// <summary>
        /// Register the item into FP2Lib's database. Items registered will be assigned internal game id, and added to the game.
        /// It can be later purchased at selected stores - or none, if so choosen, in which case You can add it manually to custom shops by the ID obtainable using <c>GetItemDataByUid(uid)</c>.
        /// To specify other parameters (ie. different descriptions per character), you can use RegisterItemDirect() with pre-cooked ItemData.
        /// </summary>
        /// <param name="uid">Unique ID of the item</param>
        /// <param name="name">Name of the item</param>
        /// <param name="inventorySprite">Inventory sprite for the item</param>
        /// <param name="shop">Which shop should it be added to</param>
        /// <param name="starCards">How many Star Cards are needed to unlock it</param>
        /// <param name="goldGemsPrice">Price in Gold Gems</param>
        /// <param name="gemsBonus">% gem bonus</param>
        /// <returns>Registered successfully?</returns>
        public static bool RegisterItem(string uid, string name, Sprite inventorySprite, string description, IAddToShop shop = IAddToShop.Yuni, int starCards = 0, int goldGemsPrice = 1, float gemsBonus = 0f)
        {
            if (!Items.ContainsKey(uid))
            {
                ItemData data = new ItemData(uid, name, description, inventorySprite, starCards, goldGemsPrice, gemsBonus, shop);
                data.id = AssignItemID(data);
                Items.Add(uid, data);
                return true;
            }
            else if (Items.ContainsKey(uid) && Items[uid].sprite == null)
            {
                Items[uid].name = name;
                Items[uid].shopLocation = shop;
                Items[uid].starCards = starCards;
                Items[uid].goldGemsPrice = goldGemsPrice;
                Items[uid].sprite = inventorySprite;
                Items[uid].descriptionGeneric = description;
                Items[uid].gemBonus = gemsBonus;
                Items[uid].id = AssignItemID(Items[uid]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Register the item into FP2Lib's database. Items registered will be assigned internal game id, and added to the game.
        /// It can be later purchased at selected stores - or none, if so choosen, in which case You can add it manually to custom shops by the ID obtainable using <c>GetItemDataByUid(uid)</c>.
        /// This method takes pre-made ItemData object.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Registered successfully?</returns>
        public static bool RegisterItemDirect(ItemData item)
        {
            string uid = item.uid;
            if (!Items.ContainsKey(uid))
            {
                item.id = AssignItemID(item);
                Items.Add(uid, item);
                return true;
            }
            else if (Items.ContainsKey(uid) && Items[uid].sprite == null)
            {
                Items[uid] = item;
                Items[uid].id = AssignItemID(Items[uid]);
                return true;
            }
            return false;
        }

        private static int AssignItemID(ItemData item)
        {
            //Vinyl already has ID
            if (item.id != 0 && Items.ContainsKey(item.uid))
            {
                ItemLogSource.LogDebug("Stored vinyl ID assigned (" + item.uid + "): " + item.id);
                return item.id;
            }
            else
            {
                ItemLogSource.LogDebug("Vinyl with unassigned ID registered! Running assignment process for " + item.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 64; i < takenIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenIDs[i])
                    {
                        item.id = i;
                        takenIDs[i] = true;
                        ItemLogSource.LogDebug("ID assigned:" + item.id);
                        //Will also break loop
                        return item.id;
                    }
                }
            }
            ItemLogSource.LogWarning("Item: " + item.uid + " failed ID assignment! That's *very* bad!");
            return 0;
        }

        /// <summary>
        /// Returns the ItemData object for given uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>ItemData for given uid, or Null if none such exists.</returns>
        public static ItemData GetItemDataByUid(string uid)
        {
            if (Items.ContainsKey(uid))
                return Items[uid];
            else return null;
        }

        //Did you know, Unity's JSON parser detonates if the root object is an array? And that it struggles _so much_ with arrays?
        //This cursed stuff is the easiest way to make it not break.
        private static void LoadFromStorage()
        {
            string json = File.ReadAllText(GameInfo.getProfilePath() + "/ItemStore.json");
            if (json.IsNullOrWhiteSpace()) return;

            string[] itemJson = json.Split(new string[] { "<sep>" }, StringSplitOptions.None);
            foreach (string itemString in itemJson)
            {
                ItemData item = ItemData.LoadFromJson(itemString);

                ItemLogSource.LogDebug("Loaded Item from storage: " + item.name + "(" + item.id + ")");
                if (!Items.ContainsKey(item.uid))
                {
                    Items.Add(item.uid, item);
                    //Extend array if needed
                    if (item.id > takenIDs.Length)
                        takenIDs = FPSaveManager.ExpandBoolArray(takenIDs, item.id);
                    //Mark id as taken
                    takenIDs[item.id] = true;
                }
            }
        }

        internal static void WriteToStorage()
        {
            if (Items.Values.Count == 0) return;

            string json = "";

            foreach (ItemData item in Items.Values)
            {
                json += item.WriteToJson();
                json += "<sep>\n";
            }

            json = json.Remove(json.Length - 6);
            json += "";

            try
            {
                byte[] bytes = new UTF8Encoding().GetBytes(json);
                using FileStream fileStream = new(string.Concat(new object[]
                {
                    GameInfo.getProfilePath(),
                    "/",
                    "ItemStore",
                    ".json"
                }), FileMode.Create, FileAccess.Write, FileShare.Read, bytes.Length, FileOptions.WriteThrough);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

    }
}
