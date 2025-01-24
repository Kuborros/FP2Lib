using BepInEx.Logging;
using FP2Lib.Vinyl;
using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Map
{
    internal class MapPatches
    {
        private static readonly ManualLogSource MapLogSource = FP2Lib.logSource;

        //Patching in the new maps. Has to be ran before the init code in Start method.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "Start", MethodType.Normal)]
        static void PatchMenuWorldMap(ref FPMapScreen[] ___mapScreens)
        {
            //Don't run code if there is nothing to add.
            if (MapHandler.Maps.Count > 0)
            {
                //Load in custom World Maps and add them to the screens array.
                foreach (MapData map in MapHandler.Maps.Values)
                {
                    //Skip maps with no id assigned
                    if (map.id != 0)
                    {
                        //If the Map ID is higher than the current screen array size, extend it and fill the empty spaces.
                        //World map will handle null GameObjects fine.
                        if (map.id >= ___mapScreens.Length)
                        {
                            for (int i = ___mapScreens.Length; i <= (map.id); i++)
                                ___mapScreens = ___mapScreens.AddToArray(null);
                        }
                        //Add the map at it's place.
                        if (map.prefab != null)
                        {
                            ___mapScreens[map.id] = GameObject.Instantiate(map.prefab).GetComponent<FPMapScreen>();
                        }
                    }
                }
                //TODO: Once all maps are done loaded, iterate over maps to fix exit and stage pointers.
                //We know we have at least one to go over.
                for (int i = 10; i < ___mapScreens.Length; ++i)
                {
                    //Skip if map not set up
                    if (___mapScreens[i] != null)
                    {
                        foreach (FPMapLocation location in ___mapScreens[i].map.locations)
                        {

                        }
                    }
                }
                MapHandler.WriteToStorage();
            }
        }
    }
}
