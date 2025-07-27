using BepInEx;
using FP2Lib.Saves;
using HarmonyLib;
using System;
using System.IO;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{

    [Serializable]
    internal class PlayerData
    {
        public float versionID = 1;
        public FPGameMode gameMode = FPGameMode.ADVENTURE;
        public int character = 0;
    }

    internal class PatchMenuFile
    {
        //If some mod wants to add their own character manually, they can add their id here to unlock it.
        public static bool[] enabledChars;


        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuFile), "Start", MethodType.Normal)]
        static void PatchMenuFileStart()
        {
            //Reset the current char to null.
            //In case player loads built-in char and some overeager mod checks only the currentCharacter and not ID.
            PlayerHandler.currentCharacter = null;

            enabledChars = new bool[255];
            //Set built-in characters
            enabledChars[0] = true;
            enabledChars[1] = true;
            enabledChars[2] = true;
            enabledChars[3] = true;
            enabledChars[4] = true;
            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                enabledChars[chara.id] = chara.registered;
            }
        }

        //Set the proper icons
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuFile), "GetFileInfo", MethodType.Normal)]
        static void PatchMenuFileInfo(int fileSlot, ref FPHudDigit[] ___characterIcons)
        {
            if (___characterIcons[fileSlot - 1].digitFrames.Length < 8)
            {
                Sprite heart = ___characterIcons[fileSlot - 1].digitFrames[6];

                for (int i = 5; i < PlayerHandler.highestID; i++)
                {
                    ___characterIcons[fileSlot - 1].digitFrames = ___characterIcons[fileSlot - 1].digitFrames.AddToArray(heart);
                }

                foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                {
                    if (chara.registered)
                        ___characterIcons[fileSlot - 1].digitFrames[chara.id + 1] = chara.livesIconAnim[0];
                }
                ___characterIcons[fileSlot - 1].digitFrames = ___characterIcons[fileSlot - 1].digitFrames.AddToArray(heart);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuFile), "GetFileInfo", MethodType.Normal)]
        static void PatchMenuFileInfoPost(int fileSlot, MenuFile __instance)
        {

            if (__instance != null)
            {
                int fpcharacterID = 0;
                //Read the save file. Yes.
                //There's no other easy way to read the ID this early - and by the time the game itself would load it, its already too late.
                //The custom PlayerData object can be used to smuggle extra fields onto player data.
                try
                {
                    string json = string.Empty;
                    if (File.Exists(SavePatches.getSavesPath() + "/" + fileSlot + ".json"))
                    {
                        using (StreamReader streamReader = new StreamReader(SavePatches.getSavesPath() + "/" + fileSlot + ".json"))
                        {
                            json = streamReader.ReadToEnd();
                        }
                        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                        float versionID = playerData.versionID;
                        fpcharacterID = playerData.character;
                    }
                }
                catch (Exception ex)
                {
                    PlayerHandler.PlayerLogSource.LogError(string.Format("An exception of type '{0}' has been caught!\nMessage: {1}\nStack Trace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace));
                }

                //If we found a file with an ID of a character whose mod is not installed, block off that file.
                //We have a local copy of basic character data used for ID assignments, the included name will be available even when mod is uninstalled so we can point the player to the right mod
                if (!enabledChars[fpcharacterID])
                {
                    MenuFilePanel menuFilePanel = __instance.files[fileSlot - 1]; ;
                    menuFilePanel.off.SetActive(false);
                    menuFilePanel.on.SetActive(false);
                    foreach (FPHudDigit digit in menuFilePanel.itemIcon)
                    {
                        digit.gameObject.SetActive(false);
                    }
                    menuFilePanel.error.SetActive(true);
                    //Set placeholder name string
                    //This is used when we have *no data at all* about the character ID in question. This should happen only on manual tinkering with files.
                    string name = "Data Deleted!\n(did you mess with the .json files?)";
                    //Load the actual name
                    if (!PlayerHandler.GetPlayableCharaByRuntimeIdSafe(fpcharacterID).Name.IsNullOrWhiteSpace())
                        name = PlayerHandler.GetPlayableCharaByRuntimeId(fpcharacterID).Name;

                    menuFilePanel.error.GetComponentInChildren<TextMesh>().text = ("This character mod is not installed.\nPlease reinstall it in order to play this file.\nCharacter: " + name);
                }
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuFile), "State_Transition", MethodType.Normal)]
        static void PatchMenuTransition()
        {
            //Set the selected character as current character
            PlayerHandler.currentCharacter = PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character);
            PlayerHandler.WriteToStorage();
        }
    }
}
