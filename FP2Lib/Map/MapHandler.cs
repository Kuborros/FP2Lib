using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.Map
{
    public class MapHandler
    {
        private static readonly ManualLogSource MapLogSource = FP2Lib.logSource;

        public static Dictionary<string, MapData> Maps = [];
        //We can go wild, as it's just an int being stored in the save file.
        internal static bool[] takenIDs = new bool[256];

        internal static void InitialiseHandler()
        {
            //Load storage data
            if (!File.Exists(GameInfo.getProfilePath() + "/WorldMapStore.json"))
                File.Create(GameInfo.getProfilePath() + "/WorldMapStore.json").Close();

            //Mark first 10 map ids as taken by base game.
            //First free one is map 10. Map 0, while not used, is an actual map within the game.
            for (int i = 0; i < 10; i++)
            {
                takenIDs[i] = true;
            }

            LoadFromStorage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid">Unique Identifier for the map.</param>
        /// <param name="name">Name of the map.</param>
        /// <param name="prefab">GameObject containing the map itself.</param>
        /// <returns></returns>
        public static bool RegisterWorldMap(string uid, string name, GameObject prefab)
        {
            if (!Maps.ContainsKey(uid))
            {
                MapData data = new MapData(uid, name, prefab);
                data.id = AssignMapID(data);
                Maps.Add(uid, data);
                return true;
            }
            else if (Maps.ContainsKey(uid) && Maps[uid].prefab == null)
            {
                Maps[uid].name = name;
                Maps[uid].prefab = prefab;
                Maps[uid].id = AssignMapID(Maps[uid]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static MapData getWorldMapByUid(string uid)
        {
            if (Maps.ContainsKey(uid))
                return Maps[uid];
            else return null;
        }

        private static int AssignMapID(MapData map)
        {
            //Map already has ID, which is not taken
            if (map.id != 0 && !takenIDs[map.id])
            {
                //Extend array if needed
                //I question how we would get over 256 world maps, but better make sure it wont explode.
                if (map.id > takenIDs.Length)
                    takenIDs = FPSaveManager.ExpandBoolArray(takenIDs, map.id);
                //Mark id as taken
                takenIDs[map.id] = true;
                MapLogSource.LogDebug("Stored World Map ID assigned (" + map.uid + "): " + map.id);
                return map.id;
            }
            else
            {
                MapLogSource.LogDebug("World Map with unassigned ID registered! Running assignment process for " + map.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 10; i < takenIDs.Length; i++)
                {
                    //First slot with false = empty space
                    //Filling empty id holes should rarely happen as that would mean uninstall of a mod AND messing with .json
                    if (!takenIDs[i])
                    {
                        map.id = i;
                        takenIDs[i] = true;
                        MapLogSource.LogDebug("ID assigned:" + map.id);
                        //Will also break loop
                        return map.id;
                    }
                }
            }
            MapLogSource.LogWarning("Map: " + map.uid + " failed ID assignment! That's *very* bad, make a bug report! Falling back to ID 0!");
            return 0;
        }


        //Did you know, Unity's JSON parser detonates if the root object is an array? And that it struggles _so much_ with arrays?
        //This cursed stuff is the easiest way to make it not break.
        //TODO: 3 classes use this same code, maybe deduplicate it?
        private static void LoadFromStorage()
        {
            string json = File.ReadAllText(GameInfo.getProfilePath() + "/WorldMapStore.json");
            if (json.IsNullOrWhiteSpace()) return;

            string[] mapJson = json.Split(new string[] { "<sep>" }, StringSplitOptions.None);
            foreach (string mapString in mapJson)
            {
                MapData map = MapData.LoadFromJson(mapString);

                MapLogSource.LogDebug("Loaded World Map from storage: " + map.name + "(" + map.uid + ")");
                if (!Maps.ContainsKey(map.uid))
                {
                    Maps.Add(map.uid, map);
                }
            }
        }

        internal static void WriteToStorage()
        {
            if (Maps.Values.Count == 0) return;

            string json = "";

            foreach (MapData map in Maps.Values)
            {
                json += map.WriteToJson();
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
                    "WorldMapStore",
                    ".json"
                }), FileMode.Create, FileAccess.Write, FileShare.Read, bytes.Length, FileOptions.WriteThrough);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
            catch (Exception e)
            {
                MapLogSource.LogError(e);
            }
        }

    }
}
