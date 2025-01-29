using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.Vinyl
{
    public static class VinylHandler
    {
        private static readonly ManualLogSource VinylLogSource = FP2Lib.logSource;

        //Get how many tracks are currently added in base game.
        public static readonly int baseTracks = Enum.GetValues(typeof(FPMusicTrack)).Length;

        internal static Dictionary<string, VinylData> Vinyls = [];
        internal static bool[] takenIDs = new bool[256];

        internal static void InitialiseHandler()
        {
            if (!File.Exists(GameInfo.getProfilePath() + "/VinylStore.json"))
                File.Create(GameInfo.getProfilePath() + "/VinylStore.json").Close();

            //Mark first ~90 vinyl ids as taken by base game.
            for (int i = 0; i < baseTracks; i++)
            {
                takenIDs[i] = true;
            }

            LoadFromStorage();
        }

        /// <summary>
        /// Register the vinyl into FP2Lib's database. Vinyls registered will be assigned internal game id, and added to the game.
        /// It can be later purchased at selected stores - or none, if so choosen, in which case You can add it manually to custom shops by the ID obtainable using <c>GetVinylDataByUid(uid)</c>.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name">Mame of the track</param>
        /// <param name="track"><c>AudioClip</c> with the track</param>
        /// <param name="shop">Which shop should it be added to</param>
        /// <param name="starCards">How many Star Cards are needed to unlock it</param>
        /// <param name="crystalsPrice">Price in crystals</param>
        /// <returns>Registered successfully?</returns>
        public static bool RegisterVinyl(string uid, string name, AudioClip track, VAddToShop shop = VAddToShop.Naomi, int starCards = 0, int crystalsPrice = 300)
        {
            if (!Vinyls.ContainsKey(uid))
            {
                VinylData data = new VinylData(uid, name, track, shop, starCards, crystalsPrice);
                data.id = AssignVinylID(data);
                Vinyls.Add(uid, data);
                return true;
            }
            else if (Vinyls.ContainsKey(uid) && Vinyls[uid].audioClip == null)
            {
                Vinyls[uid].name = name;
                Vinyls[uid].shopLocation = shop;
                Vinyls[uid].starCards = starCards;
                Vinyls[uid].crystalsPrice = crystalsPrice;
                Vinyls[uid].audioClip = track;
                Vinyls[uid].id = AssignVinylID(Vinyls[uid]);
                return true;
            }
            return false;
        }

        private static int AssignVinylID(VinylData vinyl)
        {
            //Vinyl already has ID
            if (vinyl.id != 0 && !takenIDs[vinyl.id])
            {
                //Extend array if needed
                if (vinyl.id > takenIDs.Length)
                    takenIDs = FPSaveManager.ExpandBoolArray(takenIDs, vinyl.id);
                //Mark id as taken
                takenIDs[vinyl.id] = true;
                VinylLogSource.LogDebug("Stored vinyl ID assigned (" + vinyl.uid + "): " + vinyl.id);
                return vinyl.id;
            }
            else
            {
                VinylLogSource.LogDebug("Vinyl with unassigned ID registered! Running assignment process for " + vinyl.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 64; i < takenIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenIDs[i])
                    {
                        vinyl.id = i;
                        takenIDs[i] = true;
                        VinylLogSource.LogDebug("ID assigned:" + vinyl.id);
                        //Will also break loop
                        return vinyl.id;
                    }
                }
            }
            VinylLogSource.LogWarning("Vinyl: " + vinyl.uid + " failed ID assignment! That's *very* bad!");
            return 0;
        }

        /// <summary>
        /// Returns the VinylData object for given uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>VinylData for given uid, or Null if none such exists.</returns>
        public static VinylData GetVinylDataByUid(string uid)
        {
            if (Vinyls.ContainsKey(uid))
                return Vinyls[uid];
            else return null;
        }

        //Did you know, Unity's JSON parser detonates if the root object is an array? And that it struggles _so much_ with arrays?
        //This cursed stuff is the easiest way to make it not break.
        private static void LoadFromStorage()
        {
            string json = File.ReadAllText(GameInfo.getProfilePath() + "/VinylStore.json");
            if (json.IsNullOrWhiteSpace()) return;

            string[] vinylJson = json.Split(new string[] { "<sep>" }, StringSplitOptions.None);
            foreach (string vinylString in vinylJson)
            {
                VinylData vinyl = VinylData.LoadFromJson(vinylString);

                VinylLogSource.LogDebug("Loaded Vinyl from storage: " + vinyl.name + "(" + vinyl.id + ")");
                if (!Vinyls.ContainsKey(vinyl.uid))
                {
                    Vinyls.Add(vinyl.uid, vinyl);
                }
            }
        }

        internal static void WriteToStorage()
        {
            if (Vinyls.Values.Count == 0) return;

            string json = "";

            foreach (VinylData vinyl in Vinyls.Values)
            {
                json += vinyl.WriteToJson();
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
                    "VinylStore",
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
