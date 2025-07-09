using BepInEx.Logging;
using FP2Lib.Tools;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace FP2Lib.Player
{
    public static class PlayerHandler
    {
        public static Dictionary<string, PlayableChara> PlayableChars { get; internal set; } = new();
        //The wheel can only handle 24 positions without becoming wonky, so this is the soft limit for now.
        internal static bool[] takenIDs = new bool[23];
        internal static int highestID = 4;
        private static string storePath;
        /// <summary>
        /// Current custom playable character. Will be null if the character played is a built-in one.
        /// </summary>
        [CanBeNull] public static PlayableChara currentCharacter;

        internal static readonly ManualLogSource PlayerLogSource = FP2Lib.logSource;

        internal static void InitialiseHandler()
        {
            //5 base characters
            for (int i = 0; i <= 4; i++)
            {
                takenIDs[i] = true;
            }

            storePath = Path.Combine(GameInfo.getProfilePath(), "CharaLibStore");
            Directory.CreateDirectory(storePath);

            LoadFromStorage();
        }

        /// <summary>
        /// Register a playable character into the system. Takes already cooked PlayableChara object.
        /// Functionally identical to <c>RegisterPlayableCharacterDirect</c> but with naming convention of other RegisterX
        /// </summary>
        /// <param name="character">Prepared PlayableChara object</param>
        /// <returns></returns>
        public static bool RegisterPlayableCharacter(PlayableChara character)
        {
            return RegisterPlayableCharacterDirect(character);
        }

        /// <summary>
        /// Register a playable character into the system. Takes already cooked PlayableChara object.
        /// </summary>
        /// <param name="character">Prepared PlayableChara object</param>
        /// <returns></returns>
        public static bool RegisterPlayableCharacterDirect(PlayableChara character)
        {
            if (!PlayableChars.ContainsKey(character.uid))
            {
                character.id = AssignPlayerID(character);
                //Write the assigned ID onto the prefab
                character.prefab.GetComponent<FPPlayer>().characterID = (FPCharacterID)character.id;
                character.registered = true;
                PlayableChars.Add(character.uid, character);
                return true;
            }
            if (PlayableChars.ContainsKey(character.uid) && PlayableChars[character.uid].prefab == null)
            {
                //Load up stored ID.
                character.id = PlayableChars[character.uid].id;
                //Check if assigned ID is valid.
                character.id = AssignPlayerID(character);
                //Write the assigned ID onto the prefab
                character.prefab.GetComponent<FPPlayer>().characterID = (FPCharacterID)character.id;
                character.registered = true;
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
            for (int i = 0; i <= highestID; i++)
            {
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
        /// <param name="uid">Character's UID</param>
        /// <returns></returns>
        public static PlayableChara GetPlayableCharaByUid(string uid)
        {
            return PlayableChars[uid];
        }

        /// <summary>
        /// Get playable character object by it's FPCharacterID.
        /// </summary>
        /// <param name="id">Character's FPCharacterID</param>
        /// <returns></returns>
        public static PlayableChara GetPlayableCharaByFPCharacterId(FPCharacterID id)
        {
            return GetPlayableCharaByRuntimeId((int)id);
        }

        /// <summary>
        /// Get playable character object by it's character id.
        /// </summary>
        /// <param name="id">Character's assigned ID</param>
        /// <returns>Character object, or null if none found</returns>
        [CanBeNull]
        public static PlayableChara GetPlayableCharaByRuntimeId(int id)
        {
            foreach (PlayableChara chara in PlayableChars.Values)
            {
                if (chara.id == id) return chara;
            }
            return null;
        }

        /// <summary>
        /// Get playable character object by it's character id.
        /// </summary>
        /// <param name="id">Character's assigned ID</param>
        /// <returns>Character object, or empty version of it if none found</returns>
        public static PlayableChara GetPlayableCharaByRuntimeIdSafe(int id)
        {
            foreach (PlayableChara chara in PlayableChars.Values)
            {
                if (chara.id == id) return chara;
            }
            //Return non-null value with placeholder data. Technically should be never triggered in normal scenarios but..
            return new PlayableChara();
        }

        /// <summary>
        /// Gets the character based on the character wheel postion in the main menu. Useful if you need to get that data _now_, the moment the player hovers over the character sprite.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Character object, or null if none found</returns>
        public static PlayableChara GetPlayableCharaByWheelId(int id)
        {
            foreach (PlayableChara chara in PlayableChars.Values)
            {
                if (chara.wheelId == id) return chara;
            }
            return null;
        }

        /// <summary>
        /// Switches the current character in the running save file to the specified custom character.
        /// You should *really* force a scene reset afterwards to maintain a sane gamestate. 
        /// Please be aware that it's not a guaranteed solution - some mods like Sonic initialize their properties at initial file load and don't expect to be loaded in later on.
        /// If you are making a character mod, you might wish to consider handling such scenario. I also take suggestions on handling this better on fp2lib's side.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>Returns false if the switch fails. 
        /// This can happen either by the UID not being valid, or the target character not being initialised.</returns>
        public static bool SwitchToCharacterByUID(string uid)
        {
            if (PlayableChars.ContainsKey(uid))
            {
                PlayableChara character = PlayableChars[uid];
                if (character.registered)
                {
                    FPSaveManager.character = (FPCharacterID)character.id;
                    if (FPSaveManager.targetPlayer != null)
                        FPSaveManager.targetPlayer.characterID = (FPCharacterID)character.id;
                    currentCharacter = character;
                    return true;
                }
            }
            return false;
        }

        private static int AssignPlayerID(PlayableChara character)
        {
            //Character has ID
            if (character.id != 0 && PlayableChars.ContainsKey(character.uid))
            {
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
                        PlayableChara chara = new PlayableChara(data.UID, data.name, data.runtimeID, data.gender);
                        takenIDs[data.runtimeID] = true;
                        chara.registered = false;
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
