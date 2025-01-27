using HarmonyLib;
using System.Linq;
using UnityEngine.SceneManagement;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchPlayerBossMerga
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerBossMerga), "Start", MethodType.Normal)]
        static void MergaEnding(ref FPResultsMenuSceneChange[] ___adventureCutscene)
        {
            // Only apply this edit if the player is in Merga's actual boss fight.
            if (SceneManager.GetActiveScene().name == "Bakunawa4Boss")
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
