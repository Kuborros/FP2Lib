using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FP2Lib.Player
{
    public static class PlayerHandler
    {
        internal static Dictionary<string, PlayableChara> PlayableChars = new();
        //The wheel can only handle 24 positions without becoming wonky, so this is the soft limit for now.
        internal static bool[] takenIDs = new bool[23];
        internal static int highestID = 4;
        private static string storePath;
        public static PlayableChara currentCharacter;

        internal static readonly ManualLogSource PlayerLogSource = FP2Lib.logSource;

        public static void InitialiseHandler()
        {
            //5 base characters
            for (int i = 0; i <= 4; i++)
            {
                takenIDs[i] = true;
            }

            storePath = Path.Combine(Paths.ConfigPath, "CharaLibStore");
            Directory.CreateDirectory(storePath);

            LoadFromStorage();
        }

        /// <summary>
        /// "Build it yourself" variant.
        /// </summary>
        /// <param name="character">Prepared PlayableChara object</param>
        /// <returns></returns>
        public static bool RegisterPlayableCharacterDirect(PlayableChara character)
        {
            if (!PlayableChars.ContainsKey(character.uid))
            {
                character.id = AssignPlayerID(character);
                PlayableChars.Add(character.uid, character);
                return true;
            }
            if (PlayableChars.ContainsKey(character.uid) && PlayableChars[character.uid].prefab == null)
            {
                //Load up stored ID.
                character.id = PlayableChars[character.uid].id;
                //Check if assigned ID is valid.
                character.id = AssignPlayerID(character);
                PlayableChars[character.uid] = character;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks for any holes in our ID order. Usually caused only by manual edits to the files, but it will screw things up if it happens.
        /// </summary>
        /// <returns>Have any holes been found.</returns>
        public static bool doWeHaveHolesInIds()
        {
            for (int i = 0; i <= highestID;i++)
            {
                //VERY BAD
                if (!takenIDs[i])
                {
                    PlayerLogSource.LogError("Very bad thing boss, there's a hole in the Player ID's! Investigate!");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get playable character object by it's uid string.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static PlayableChara GetPlayableCharaByUid(string uid)
        {
            return PlayableChars[uid];
        }

        /// <summary>
        /// Get playable character object by it's FPCharacterID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PlayableChara GetPlayableCharaByFPCharacterId(FPCharacterID id)
        {
            return GetPlayableCharaByRuntimeId((int)id);
        }

        /// <summary>
        /// Get playable character object by it's character id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PlayableChara GetPlayableCharaByRuntimeId(int id)
        {
            foreach (PlayableChara chara in PlayableChars.Values)
            {
                if (chara.id == id) return chara; 
            }
            return null;
        }

        /// <summary>
        /// Get amount of total characters in-game which are in functional state
        /// </summary>
        /// <returns></returns>
        internal static int GetTotalActiveCharacters()
        {
            int totalActiveCharacters = 4;
            foreach (PlayableChara chara in PlayableChars.Values )
            {
                if (chara.prefab != null) totalActiveCharacters++;
            }
            return totalActiveCharacters;
        }

        private static int AssignPlayerID(PlayableChara character)
        {
            //Character has ID
            if (character.id != 0 && !takenIDs[character.id])
            {
                takenIDs[character.id] = true;
                PlayerLogSource.LogDebug("Stored playable character ID assigned (" + character.uid + "): " + character.id);
                if (character.id > highestID) highestID = character.id;
                return character.id;
            }
            else
            {
                PlayerLogSource.LogDebug("Character with unassigned ID registered! Running assignment process for " + character.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 4; i < takenIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenIDs[i])
                    {
                        character.id = i;
                        takenIDs[i] = true;
                        PlayerLogSource.LogDebug("Assigned ID: " + character.id);
                        if (character.id > highestID) highestID = character.id;
                        //Will also break loop
                        return character.id;
                    }
                }
            }
            PlayerLogSource.LogWarning("Character: " + character.uid + " failed ID assignment! That's *very* bad!");
            return 0;
        }

        private static void LoadFromStorage()
        {
            foreach (string js in Directory.GetFiles(storePath))
            {
                if (js.EndsWith(".json"))
                {
                    CharacterData data = CharacterData.LoadFromJson(File.ReadAllText(js));
                    PlayerLogSource.LogDebug("Loaded Character from storage: " + data.name + "(" + data.UID + ")");
                    if (!PlayableChars.ContainsKey(data.UID))
                    {
                        PlayableChara chara = new PlayableChara(data.UID, data.name, data.runtimeID,data.gender);
                        PlayableChars.Add(data.UID, chara);
                    }
                }
            }
        }

        internal static void WriteToStorage()
        {
            foreach (PlayableChara chara in PlayableChars.Values)
            {
                if (chara.id >= 5)
                {
                    String json = chara.GetCharacterData().WriteToJson();

                    try
                    {
                        byte[] bytes = new UTF8Encoding().GetBytes(json);
                        using (FileStream fileStream = new FileStream(string.Concat(new object[]
                        {
                    storePath,
                    "/",
                    chara.uid,
                    ".json"
                        }), FileMode.Create, FileAccess.Write, FileShare.Read, bytes.Length, FileOptions.WriteThrough))
                        {
                            fileStream.Write(bytes, 0, bytes.Length);
                            fileStream.Flush();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }

    }
}
