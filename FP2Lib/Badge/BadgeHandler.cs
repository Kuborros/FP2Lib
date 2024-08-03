using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.Badge
{
    
    public class BadgeHandler
    {
        private static readonly ManualLogSource BadgeLogSource = FP2Lib.logSource;

        //64 is the top badge id, for now. The save manager allocates 99 badge slots by default.

        public static Dictionary<string,BadgeData> Badges = new Dictionary<string,BadgeData>();
        internal static bool[] takenIDs = new bool[256];

        internal static void InitialiseHandler()
        {
            //Load storage data
            //TODO: Move to per-profile path
            if (!File.Exists(Paths.ConfigPath + "/BadgeStore.json"))
                File.Create(Paths.ConfigPath + "/BadgeStore.json").Close();

            //Mark first 64 badge ids as taken by base game.
            for (int i = 0; i <= 64; i++)
            {
                takenIDs[i] = true;
            }

            LoadFromStorage();
        }

        /// <summary>
        /// Register the badge into FP2Lib's database. Badges registered will be assigned internal game id, and added to the game.
        /// You can then use <c>UnlockBadge(uid)</c> method to unlock the badge when needed.
        /// </summary>
        /// <param name="uid">Badge's Unique Identifier string</param>
        /// <param name="name">Name of the Badge</param>
        /// <param name="description">Description of the Badge</param>
        /// <param name="sprite">Sprite of the Badge</param>
        /// <param name="type">Type of the badge (Gold/Silver)</param>
        /// <param name="visibility">Visibility of the badge (should description be shown before unlock).</param>
        /// <returns></returns>
        public static bool RegisterBadge(string uid, string name, string description, Sprite sprite, FPBadgeType type = FPBadgeType.SILVER, FPBadgeVisible visibility = FPBadgeVisible.ALWAYS)
        {
            if (!Badges.ContainsKey(uid))
            {
                BadgeData badgeData = new BadgeData(uid ,name, description, type, visibility, sprite);
                badgeData.id = AssignBadgeID(badgeData);
                Badges.Add(uid, badgeData);
                WriteToStorage();
                return true;
            }
            else if (Badges.ContainsKey(uid) && Badges[uid].sprite == null)
            {
                Badges[uid].sprite = sprite;
                Badges[uid].type = type;
                Badges[uid].visibility = visibility;
                Badges[uid].name = name;
                Badges[uid].description = description;
                Badges[uid].id = AssignBadgeID(Badges[uid]);
                WriteToStorage();
                return true;
            }
            return false;
        }

        private static int AssignBadgeID(BadgeData badge)
        {
            //Badge already has ID
            if (badge.id != 0)
            {
                //Extend array if needed
                if (badge.id > takenIDs.Length)
                    takenIDs = FPSaveManager.ExpandBoolArray(takenIDs, badge.id);
                takenIDs[badge.id] = true;
                BadgeLogSource.LogDebug("Stored badge ID assigned (" + badge.uid + "): " + badge.id);
                return badge.id;
            } 
            else
            {
                BadgeLogSource.LogDebug("Badge with unassigned ID registered! Running assignment process for " + badge.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 64; i < takenIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenIDs[i])
                    {
                        badge.id = i;
                        takenIDs[i] = true;
                        BadgeLogSource.LogDebug("ID assigned:" + badge.id);
                        //Will also break loop
                        return badge.id;
                    }
                }
            }
            BadgeLogSource.LogWarning("Badge: " + badge.uid + " failed ID assignment! That's bad!");
            return 0;
        }

        /// <summary>
        /// Get BadgeData by uid
        /// </summary>
        /// <param name="uid">Badge's Unique Identifier string</param>
        /// <returns>BadgeData object</returns>
        public static BadgeData GetBadgeDataByUid(string uid)
        {
            return Badges[uid];
        }

        /// <summary>
        /// Unlock badge by uid
        /// </summary>
        /// <param name="uid">Badge's Unique Identifier string</param>
        /// <returns>Unlock success</returns>
        public static bool UnlockBadge(string uid)
        {
            if (Badges.ContainsKey(uid))
            {
                int id = Badges[uid].id;
                return UnlockBadge(id);
            }
            else return false;
        }

        /// <summary>
        /// Unlock badge by numeric id
        /// </summary>
        /// <param name="id">Badge's numeric ID</param>
        /// <returns>Unlock success</returns>
        public static bool UnlockBadge(int id)
        {
            if (FPSaveManager.badges[id] == 0 && !FPSaveManager.demoMode)
            {
                FPSaveManager.ForceBadgeUnlock(id);
                return true;
            } 
            else return false;
        }


        private static void LoadFromStorage()
        {
            string json = File.ReadAllText(Paths.ConfigPath + "/BadgeStore.json");
            if (json.IsNullOrWhiteSpace()) return;

            string[] badgeJson = json.Split(new string[] { "<sep>" }, StringSplitOptions.None);
            foreach (string badgeString in badgeJson)
            {
                BadgeData badge = BadgeData.LoadFromJson(badgeString);

                BadgeLogSource.LogDebug("Loaded Badge from storage: " + badge.name + "(" + badge.id + ")");
                if (!Badges.ContainsKey(badge.uid))
                {
                    Badges.Add(badge.uid, badge);
                }
            }
        }

        internal static void WriteToStorage()
        {
            if (Badges.Values.Count == 0) return;

            string json = "";

            foreach (BadgeData badge in Badges.Values)
            {
                json += badge.WriteToJson();
                json += "<sep>\n";
            }

            json = json.Remove(json.Length - 6);
            json += "";

            try
            {
                byte[] bytes = new UTF8Encoding().GetBytes(json);
                using FileStream fileStream = new(string.Concat(new object[]
                {
                    Paths.ConfigPath,
                    "/",
                    "BadgeStore",
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
