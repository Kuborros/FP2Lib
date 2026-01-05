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
    public class ItemHandler
    {
        private static readonly ManualLogSource ItemLogSource = FP2Lib.logSource;
        internal static bool isPotionSellerInstalled = false;

        //First empty slot with Potion Seller installed is 100
        //Without it, it's slot 45
        public static readonly int baseItems = 100;

        //First empty slot with Potion Seller installed is 11
        public static readonly int basePotions = 11;

        internal static Dictionary<string, ItemData> Items = [];
        internal static bool[] takenItemIDs = new bool[256];
        internal static bool[] takenPotionIDs = new bool[100];

        internal static int itemCount = 0;
        internal static int potionCount = 0;

        internal static void InitialiseHandler()
        {
            if (!File.Exists(GameInfo.getProfilePath() + "/ItemStore.json"))
                File.Create(GameInfo.getProfilePath() + "/ItemStore.json").Close();

            //Mark first 100 item ids as taken by base game + Potion Seller.
            for (int i = 0; i < baseItems; i++)
            {
                takenItemIDs[i] = true;
            }

            if (!File.Exists(GameInfo.getProfilePath() + "/PotionStore.json"))
                File.Create(GameInfo.getProfilePath() + "/PotionStore.json").Close();

            //Mark first 10 potion ids as taken by base game + Potion Seller.
            for (int i = 0; i < basePotions; i++)
            {
                takenPotionIDs[i] = true;
            }

            LoadItemsFromStorage();
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
                data.itemID = AssignItemID(data);
                Items.Add(uid, data);
                itemCount++;
                return true;
            }
            else if (Items.ContainsKey(uid) && Items[uid].sprite == null)
            {
                Items[uid].name = name;
                Items[uid].itemShopLocation = shop;
                Items[uid].starCards = starCards;
                Items[uid].goldGemsPrice = goldGemsPrice;
                Items[uid].sprite = inventorySprite;
                Items[uid].descriptionGeneric = description;
                Items[uid].gemBonus = gemsBonus;
                Items[uid].itemID = AssignItemID(Items[uid]);
                itemCount++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Register a potion into FP2Lib's database. Items registered will be assigned internal game id, and added to the game.
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
        public static bool RegisterPotion(string uid, string name, Sprite inventorySprite, Sprite spriteBottleBottom, Sprite spriteBottleMid, Sprite spriteBottleTop,
            string description, string effectDescription, PAddToShop shop = PAddToShop.Milla, int starCards = 0, int goldGemsPrice = 1, float effectPercent = 0f)
        {
            if (!Items.ContainsKey(uid))
            {
                ItemData data = new ItemData(uid, name, description, inventorySprite, starCards, goldGemsPrice, true, effectPercent, effectDescription, spriteBottleMid, spriteBottleTop, spriteBottleBottom, shop);
                data.itemID = AssignItemID(data);
                data.potionID = AssignPotionID(data);
                Items.Add(uid, data);
                itemCount++;
                potionCount++;
                return true;
            }
            else if (Items.ContainsKey(uid) && Items[uid].sprite == null)
            {
                Items[uid].name = name;
                Items[uid].potionShopLocation = shop;
                Items[uid].starCards = starCards;
                Items[uid].goldGemsPrice = goldGemsPrice;
                Items[uid].sprite = inventorySprite;
                Items[uid].spriteBottleBottom = spriteBottleBottom;
                Items[uid].spriteBottleMid = spriteBottleMid;
                Items[uid].spriteBottleTop = spriteBottleTop;
                Items[uid].effectPercentage = effectPercent;
                Items[uid].effect = effectDescription;
                Items[uid].descriptionGeneric = description;
                Items[uid].itemID = AssignItemID(Items[uid]);
                Items[uid].potionID = AssignPotionID(Items[uid]);
                itemCount++;
                potionCount++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Register the item/potion into FP2Lib's database. Items (or Potions) registered will be assigned internal game id, and added to the game.
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
                item.itemID = AssignItemID(item);
                Items.Add(uid, item);
                itemCount++;
                if (item.isPotion)
                {
                    item.potionID = AssignPotionID(item);
                    potionCount++;
                }
                return true;
            }
            else if (Items.ContainsKey(uid) && Items[uid].sprite == null)
            {
                Items[uid] = item;
                Items[uid].itemID = AssignItemID(Items[uid]);
                itemCount++;
                if (Items[uid].isPotion)
                {
                    Items[uid].potionID = AssignPotionID(item);
                    potionCount++;
                }
                return true;
            }
            return false;
        }

        private static int AssignItemID(ItemData item)
        {
            //Item already has ID
            if (item.itemID != 0 && Items.ContainsKey(item.uid))
            {
                ItemLogSource.LogDebug("Stored item ID assigned (" + item.uid + "): " + item.itemID);
                return item.itemID;
            }
            else
            {
                ItemLogSource.LogDebug("Item with unassigned ID registered! Running assignment process for " + item.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 99; i < takenItemIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenItemIDs[i])
                    {
                        item.itemID = i;
                        takenItemIDs[i] = true;
                        ItemLogSource.LogDebug("ID assigned:" + item.itemID);
                        //Will also break loop
                        return item.itemID;
                    }
                }
            }
            ItemLogSource.LogWarning("Item: " + item.uid + " failed ID assignment! That's *very* bad!");
            return 0;
        }

        private static int AssignPotionID(ItemData item)
        {
            //Item already has ID
            if (item.potionID != 0 && Items.ContainsKey(item.uid))
            {
                ItemLogSource.LogDebug("Stored potion ID assigned (" + item.uid + "): " + item.itemID);
                return item.itemID;
            }
            else
            {
                ItemLogSource.LogDebug("Potion with unassigned potion ID registered! Running assignment process for " + item.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 10; i < takenPotionIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenPotionIDs[i])
                    {
                        item.potionID = i;
                        takenPotionIDs[i] = true;
                        ItemLogSource.LogDebug("ID assigned:" + item.potionID);
                        //Will also break loop
                        return item.potionID;
                    }
                }
            }
            ItemLogSource.LogWarning("Item: " + item.uid + " failed potion ID assignment! That's *very* bad!");
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

        /// <summary>
        /// Returns the ItemData object for given item ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ItemData for given id, or Null if none such exists.</returns>
        public static ItemData GetItemDataByRuntimeItemID(int id)
        {
            foreach (ItemData data in Items.Values)
            {
                if (data.itemID == id) return data;
            }
            return null;
        }

        /// <summary>
        /// Returns the ItemData object for given potion ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ItemData for given id, or Null if none such exists.</returns>
        public static ItemData GetItemDataByRuntimePotionID(int id)
        {
            foreach (ItemData data in Items.Values)
            {
                if (data.potionID == id && data.isPotion) return data;
            }
            return null;
        }

        //Did you know, Unity's JSON parser detonates if the root object is an array? And that it struggles _so much_ with arrays?
        //This cursed stuff is the easiest way to make it not break.
        private static void LoadItemsFromStorage()
        {
            string json = File.ReadAllText(GameInfo.getProfilePath() + "/ItemStore.json");
            if (json.IsNullOrWhiteSpace()) return;

            string[] itemJson = json.Split(new string[] { "<sep>" }, StringSplitOptions.None);
            foreach (string itemString in itemJson)
            {
                ItemData item = ItemData.LoadFromJson(itemString);

                ItemLogSource.LogDebug("Loaded Item from storage: " + item.name + "(" + item.itemID + ")");
                if (!Items.ContainsKey(item.uid))
                {
                    Items.Add(item.uid, item);
                    //Extend array if needed
                    if (item.itemID > takenItemIDs.Length)
                        takenItemIDs = FPSaveManager.ExpandBoolArray(takenItemIDs, item.itemID);
                    //Mark id as taken
                    takenItemIDs[item.itemID] = true;
                }
            }
        }

        internal static void WriteItemsToStorage()
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
