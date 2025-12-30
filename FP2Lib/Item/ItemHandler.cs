using BepInEx.Logging;
using FP2Lib.Tools;
using System.Collections.Generic;
using System.IO;

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

            //LoadFromStorage();
        }

    }
}
