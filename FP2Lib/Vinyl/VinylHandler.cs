using System;
using System.Collections.Generic;
using UnityEngine;

namespace FP2Lib.Vinyl
{
    internal class VinylHandler
    {

        //Get how many tracks are currently added.
        private static readonly int baseTracks = Enum.GetValues(typeof(FPMusicTrack)).Length;
        public static int totalTracks = baseTracks;

        internal static Dictionary<string, VinylData> Vinyls = [];


        internal static void InitialiseHandler()
        {
            LoadFromStorage();
        }

        public static bool RegisterVinyl(string name,AudioClip track)
        {


            return false;
        }


        public VinylData GetVinylDataByName(string name)
        {
            return Vinyls[name];
        } 

        private static void LoadFromStorage()
        {
           
        }

        internal static void WriteToStorage()
        {
        
        }

    }
}
