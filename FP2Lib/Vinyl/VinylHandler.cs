using BepInEx;
using BepInEx.Logging;
using FP2Lib.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FP2Lib.Vinyl
{
    public static class VinylHandler
    {
        private static readonly ManualLogSource VinylLogSource = FP2Lib.logSource;

        //Get how many tracks are currently added.
        private static readonly int baseTracks = Enum.GetValues(typeof(FPMusicTrack)).Length;
        public static int totalTracks = baseTracks - 1;

        internal static Dictionary<string, VinylData> Vinyls = [];


        internal static void InitialiseHandler()
        {
            //File.Create(Paths.ConfigPath + "/VinylStore.json").Close();
            //LoadFromStorage();
        }

        public static bool RegisterVinyl(string name,AudioClip track, VAddToShop shop)
        {
            if (!Vinyls.ContainsKey(name))
            {
                totalTracks++;
                VinylData data = new VinylData(name,track,totalTracks,shop);
                Vinyls.Add(name,data);
                return true;
            } 
            else if (Vinyls.ContainsKey(name) && Vinyls[name].audioClip == null)
            {
                Vinyls[name].audioClip = track;
                return true;
            }
            return false;
        }

        public static VinylData GetVinylDataByName(string name)
        {
            return Vinyls[name];
        } 

        /*
        private static void LoadFromStorage()
        {
            string json = File.ReadAllText(Paths.ConfigPath + "/VinylStore.json");
            if (json.IsNullOrWhiteSpace()) return;
            VinylData[] vinylInfo = JsonHelper.FromJson<VinylData>(json);
            if (vinylInfo == null) return;
            foreach (VinylData vinyl in vinylInfo)
            {
                VinylLogSource.LogDebug("Loaded Vinyl from storage: " + vinyl.name + "(" + vinyl.id + ")");
                if (!Vinyls.ContainsKey(vinyl.name))
                {
                    totalTracks++;
                    Vinyls.Add(vinyl.name, vinyl);
                }
            }
        }

        internal static void WriteToStorage()
        {
            string json = "{\"Items\":[\n";

            foreach (VinylData vinyl in Vinyls.Values)
            {
                json += vinyl.WriteToJson();
                json += ",\n";
            }

            json = json.Remove(json.Length - 2);
            json += "\n]}";

            try
            {
                byte[] bytes = new UTF8Encoding().GetBytes(json);
                using FileStream fileStream = new(string.Concat(new object[]
                {
                    Paths.ConfigPath,
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
        */
    }
}
