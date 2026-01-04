using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.Potion
{
    public class PotionHandler
    {
        private static readonly ManualLogSource PotionLogSource = FP2Lib.logSource;
        internal static bool isPotionSellerInstalled = false;

        //First empty slot with Potion Seller installed is 11
        public static readonly int basePotions = 11;

        internal static Dictionary<string, PotionData> Potions = [];
        internal static bool[] takenIDs = new bool[100];

        internal static void InitialiseHandler()
        {
            if (!File.Exists(GameInfo.getProfilePath() + "/PotionStore.json"))
                File.Create(GameInfo.getProfilePath() + "/PotionStore.json").Close();

            //Mark first 10 potion ids as taken by base game + Potion Seller.
            for (int i = 0; i < basePotions; i++)
            {
                takenIDs[i] = true;
            }

            LoadFromStorage();
        }

        /// <summary>
        /// Register the potion into FP2Lib's database. Potions registered will be assigned internal game id, and added to the game.
        /// It can be later purchased at selected stores - or none, if so choosen, in which case You can add it manually to custom shops by the ID obtainable using <c>GetPotionDataByUid(uid)</c>.
        /// To specify other parameters (ie. different descriptions per character), you can use RegisterPotionDirect() with pre-cooked PotionData.
        /// </summary>
        /// <param name="uid">Unique ID of the potion</param>
        /// <param name="name">Name of the potion</param>
        /// <param name="inventorySprite">Inventory sprite for the potion</param>
        /// <param name="spriteBottleBottom">Sprite used in bottle previev (bottom)</param>
        /// <param name="spriteBottleMid">Sprite used in bottle previev (middle)</param>
        /// <param name="spriteBottleTop">Sprite used in bottle previev (top)</param>
        /// <param name="inventorySprite">Inventory sprite for the potion</param>
        /// <param name="shop">Which shop should it be added to</param>
        /// <param name="starCards">How many Star Cards are needed to unlock it</param>
        /// <param name="goldGemsPrice">Price in Gold Gems</param>
        /// <returns>Registered successfully?</returns>
        public static bool RegisterPotion(string uid, string name, Sprite inventorySprite, string description, Sprite spriteBottleBottom, Sprite spriteBottleMid, Sprite spriteBottleTop, PAddToShop shop = PAddToShop.Milla, int starCards = 0, int goldGemsPrice = 1)
        {
            if (!Potions.ContainsKey(uid))
            {
                PotionData data = new PotionData(uid, name, description, inventorySprite, spriteBottleMid, spriteBottleTop, spriteBottleBottom, starCards, goldGemsPrice, shop);
                data.id = AssignPotionID(data);
                Potions.Add(uid, data);
                return true;
            }
            else if (Potions.ContainsKey(uid) && Potions[uid].inventorySprite == null)
            {
                Potions[uid].name = name;
                Potions[uid].shopLocation = shop;
                Potions[uid].starCards = starCards;
                Potions[uid].goldGemsPrice = goldGemsPrice;
                Potions[uid].inventorySprite = inventorySprite;
                Potions[uid].spriteBottleBottom = spriteBottleBottom;
                Potions[uid].spriteBottleMid = spriteBottleMid;
                Potions[uid].spriteBottleTop = spriteBottleTop;
                Potions[uid].description = description;
                Potions[uid].id = AssignPotionID(Potions[uid]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Register the potion into FP2Lib's database. Potions registered will be assigned internal game id, and added to the game.
        /// It can be later purchased at selected stores - or none, if so choosen, in which case You can add it manually to custom shops by the ID obtainable using <c>GetPotionDataByUid(uid)</c>.
        /// This method takes pre-made PotionData object.
        /// </summary>
        /// <param name="potion"></param>
        /// <returns>Registered successfully?</returns>
        public static bool RegisterPotionDirect(PotionData potion)
        {
            string uid = potion.uid;
            if (!Potions.ContainsKey(uid))
            {
                potion.id = AssignPotionID(potion);
                Potions.Add(uid, potion);
                return true;
            }
            else if (Potions.ContainsKey(uid) && Potions[uid].inventorySprite == null)
            {
                Potions[uid] = potion;
                Potions[uid].id = AssignPotionID(Potions[uid]);
                return true;
            }
            return false;
        }

        private static int AssignPotionID(PotionData potion)
        {
            //Vinyl already has ID
            if (potion.id != 0 && Potions.ContainsKey(potion.uid))
            {
                PotionLogSource.LogDebug("Stored potion ID assigned (" + potion.uid + "): " + potion.id);
                return potion.id;
            }
            else
            {
                PotionLogSource.LogDebug("Potion with unassigned ID registered! Running assignment process for " + potion.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 64; i < takenIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenIDs[i])
                    {
                        potion.id = i;
                        takenIDs[i] = true;
                        PotionLogSource.LogDebug("ID assigned:" + potion.id);
                        //Will also break loop
                        return potion.id;
                    }
                }
            }
            PotionLogSource.LogWarning("Potion: " + potion.uid + " failed ID assignment! That's *very* bad!");
            return 0;
        }

        /// <summary>
        /// Returns the PotionData object for given uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>PotionData for given uid, or Null if none such exists.</returns>
        public static PotionData GetPotionDataByUid(string uid)
        {
            if (Potions.ContainsKey(uid))
                return Potions[uid];
            else return null;
        }

        //Did you know, Unity's JSON parser detonates if the root object is an array? And that it struggles _so much_ with arrays?
        //This cursed stuff is the easiest way to make it not break.
        private static void LoadFromStorage()
        {
            string json = File.ReadAllText(GameInfo.getProfilePath() + "/PotionStore.json");
            if (json.IsNullOrWhiteSpace()) return;

            string[] potionJson = json.Split(new string[] { "<sep>" }, StringSplitOptions.None);
            foreach (string potionString in potionJson)
            {
                PotionData potion = PotionData.LoadFromJson(potionString);

                PotionLogSource.LogDebug("Loaded Potion from storage: " + potion.name + "(" + potion.id + ")");
                if (!Potions.ContainsKey(potion.uid))
                {
                    Potions.Add(potion.uid, potion);
                    //Extend array if needed
                    if (potion.id > takenIDs.Length)
                        takenIDs = FPSaveManager.ExpandBoolArray(takenIDs, potion.id);
                    //Mark id as taken
                    takenIDs[potion.id] = true;
                }
            }
        }

        internal static void WriteToStorage()
        {
            if (Potions.Values.Count == 0) return;

            string json = "";

            foreach (PotionData potion in Potions.Values)
            {
                json += potion.WriteToJson();
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
                    "PotionStore",
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
