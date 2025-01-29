using HarmonyLib;
using System.Linq;
using UnityEngine.SceneManagement;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPStage
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
        static void PatchStart(ref FPPlayer[] ___playerList)
        {
            //Load each playable characters GameObject prefab here, append to ___playerList in order of IDs
            for (int i = 4; i < PlayerHandler.highestID; i++)
            {
                ___playerList = ___playerList.AddToArray(null);
            }

            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                if (chara.registered)
                {
                    ___playerList[chara.id] = chara.prefab.GetComponent<FPPlayer>();
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPStage), "Start", MethodType.Normal)]
        static void WeaponsCoreEnding(ref FPResultsMenuSceneChange[] ___adventureCutscene)
        {
            // Only apply this edit if the player is in Weapon's Core.
            if (SceneManager.GetActiveScene().name == "Bakunawa5")
            {
                // Loop through each custom player character.
                for (int i = 0; i < PlayerHandler.PlayableChars.Values.Count; i++)
                {
                    // Get the player value at this index.
                    var entry = PlayerHandler.PlayableChars.ElementAt(i).Value;

                    // Add an entry for this character to the Adventure Cutscene array to load the ending for them.
                    ___adventureCutscene = ___adventureCutscene.AddToArray(new()
                    {
                        character = (FPCharacterID)entry.id,
                        scene = "Cutscene_Ending2",
                        storyFlag = 46,
                        storyFlagValue = 1
                    });
                }
            }
        }
    }
}
