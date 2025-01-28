using BepInEx;
using BepInEx.Logging;
using FP2Lib.Stage;
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
                            //Set the parent ID to the location in mapScreens array
                            ___mapScreens[map.id].parentID = map.id;
                            //Deactivate the object, it will get activated by the map itself when needed.
                            ___mapScreens[map.id].gameObject.SetActive(false);
                        }
                    }
                }
                //We know we have at least one to go over.
                for (int i = 10; i < ___mapScreens.Length; ++i)
                {
                    //Skip if map not set up
                    //Map screen might also be broken and have no map, check to be sure.
                    if (___mapScreens[i] != null && ___mapScreens[i].map != null)
                    {
                        foreach (FPMapLocation location in ___mapScreens[i].map.locations)
                        {
                            //Skip processing invisible nodes, they should not have level/map exits on them
                            //Broken locations can exist if modder is not caucious, skip these to not break all the other maps.
                            if (location != null)
                            {
                                if (location.type != FPMapLocationType.NONE && location.icon != null)
                                {
                                    //Get the menuText
                                    MenuText menuText = location.icon.GetComponent<MenuText>();
                                    string destination;
                                    //Is it a stage/boss?
                                    //These have 3 lines of existing data, so target ends in 4th line
                                    if (menuText != null && (location.type == FPMapLocationType.STAGE || location.type == FPMapLocationType.BATTLE) && menuText.paragraph.Length > 3)
                                    {
                                        destination = menuText.paragraph[3];
                                        if (!destination.IsNullOrWhiteSpace())
                                        {
                                            MapLogSource.LogDebug("Creating link to stage with uid: " + destination + " for marker: " + location.icon.name);
                                            CustomStage target = StageHandler.getCustomStageByUid(destination);
                                            //Check if target stage exists.
                                            if (target != null)
                                            {
                                                location.pointers.stageID = target.id;
                                                //If we have a custom Vinyl present, replace the id so it shows properly on the map.
                                                if (!target.vinylUID.IsNullOrWhiteSpace())
                                                {
                                                    VinylData vinyl = VinylHandler.GetVinylDataByUid(target.vinylUID);
                                                    if (vinyl != null)
                                                        location.pointers.hudVinylID = vinyl.id;
                                                }
                                                //Same for item!
                                                if (!target.itemUID.IsNullOrWhiteSpace())
                                                {
                                                    //TODO: Add item code when that feature is done!
                                                }
                                            }
                                            else
                                            {
                                                MapLogSource.LogError("Found a link to non-existent target stage! Bad! Check if uid is valid: " + destination);
                                                //Fallback to Dragon Valley
                                                //Skipping all the item and vinyl logic because yea.
                                                location.pointers.stageID = 1;
                                            }
                                        }
                                    }
                                    //Not a stage - it's an exit or hub
                                    //Just their name here, so our target uid is in line 2
                                    else if (menuText != null && (location.type == FPMapLocationType.HUB) && menuText.paragraph.Length > 1)
                                    {
                                        destination = menuText.paragraph[1];
                                        if (!destination.IsNullOrWhiteSpace())
                                        {
                                            MapLogSource.LogDebug("Creating link to hub with uid: " + destination + " for marker: " + location.icon.name);
                                            CustomStage target = StageHandler.getCustomStageByUid(destination);
                                            //Make sure we actually found a valid stage!
                                            if (target != null)
                                            {
                                                location.pointers.stageID = StageHandler.getCustomStageByUid(destination).id;
                                            }
                                            else
                                            {
                                                MapLogSource.LogError("Found a link to non-existent target hub! Bad! Check if uid is valid: " + destination);
                                                //Fallback to first hub.
                                                location.pointers.stageID = 1;
                                            }
                                        }
                                    }
                                    //Exits lead to maps, so we need a map id
                                    else if (menuText != null && (location.type == FPMapLocationType.EXIT || location.type == FPMapLocationType.EXIT_PROMPT) && menuText.paragraph.Length > 1)
                                    {
                                        destination = menuText.paragraph[1];
                                        if (!destination.IsNullOrWhiteSpace())
                                        {
                                            MapLogSource.LogDebug("Creating link to map with uid: " + destination + " for exit: " + location.icon.name);
                                            MapData target = MapHandler.getWorldMapByUid(destination);
                                            if (target != null)
                                            {
                                                location.pointers.mapID = MapHandler.getWorldMapByUid(destination).id;
                                            }
                                            else
                                            {
                                                MapLogSource.LogError("Found a link to non-existent target map! That's no good! Check if uid is valid: " + destination);
                                                //Fallback to first map. Location id also has to be set to a number that exists there.
                                                location.pointers.mapID = 1;
                                                location.pointers.locationID = 0;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
