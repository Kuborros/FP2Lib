using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FP2Lib.Player
{
    public static class PlayerHandler
    {
        internal static Dictionary<string, PlayableChara> PlayableChars = new();
        internal static bool[] takenIDs = new bool[256];
        internal static int highestID = 4;
        private static string storePath;
        public static PlayableChara currentCharacter;

        private static readonly ManualLogSource PlayerLogSource = new ManualLogSource("FP2Lib-Player");

        public static void InitialiseHandler()
        {
            //5 base characters
            for (int i = 0; i < 5; i++)
            {
                takenIDs[i] = true;
            }

            storePath = Path.Combine(Paths.ConfigPath, "CharaLibStore");
            Directory.CreateDirectory(storePath);

            LoadFromStorage();
        }

        public static bool RegisterPlayableCharacter(string uid, string name, CharacterGender gender, Action airMoves, Action groundMoves, GameObject prefab,AssetBundle assets)
        {
            if (!PlayableChars.ContainsKey(uid))
            {
                PlayableChara chara = new PlayableChara(uid, name,gender, airMoves, groundMoves, prefab,assets);
                PlayerLogSource.LogInfo("Registering character with no ID, assigned ID:");
                chara.id = AssignPlayerID(chara);
                PlayerLogSource.LogInfo(chara.id);
                PlayableChars.Add(uid, chara);
                return true;
            }
            if (PlayableChars.ContainsKey(uid) && PlayableChars[uid].prefab == null)
            {
                PlayableChara chara = new PlayableChara(uid, name,gender, airMoves, groundMoves, prefab,assets);
                PlayerLogSource.LogInfo("Registering character with existing ID, assigned ID:");
                chara.id = AssignPlayerID(chara);
                PlayerLogSource.LogInfo(chara.id);
                PlayableChars.Add(uid, chara);
                return true;
            }
            return false;
        }

        //Scan for VERY BAD scenario. We do _not_ want this to happen.
        public static bool doWeHaveHolesInIds()
        {
            for (int i = 0; i <= highestID;i++)
            {
                //VERY BAD
                if (!takenIDs[i])
                {
                    return true;
                }
            }
            return false;
        }


        internal static int GetRealTotalCharacterNumber()
        {
            return takenIDs.Count(c => c);
        }

        private static int AssignPlayerID(PlayableChara character)
        {
            //Character has ID
            if (character.id != 0)
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
                for (int i = 64; i < takenIDs.Length; i++)
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

        public static PlayableChara GetPlayableCharaByUid(string uid)
        {
            return PlayableChars[uid];
        }

        private static void LoadFromStorage()
        {
            foreach (string js in Directory.GetFiles(storePath))
            {
                if (js.EndsWith(".json"))
                {
                    CharacterData data = CharacterData.LoadFromJson(File.ReadAllText(js));
                    PlayerLogSource.LogInfo("Loaded Character from storage: " + data.name + "(" + data.UID + ")");
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
