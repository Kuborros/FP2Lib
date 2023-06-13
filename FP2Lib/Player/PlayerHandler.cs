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
        private static string storePath;
        public static PlayableChara currentCharacter;

        private static readonly ManualLogSource PlayerLogSource = new ManualLogSource("FP2Lib-Player");

        public static void InitialiseHandler()
        {
            storePath = Path.Combine(Paths.ConfigPath, "CharaLibStore");
            Directory.CreateDirectory(storePath);

            LoadFromStorage();
        }

        public static bool RegisterPlayableCharacter(string uid, string name, Delegate airMoves, Delegate groundMoves, GameObject prefab)
        {
            if (!PlayableChars.ContainsKey(uid))
            {
                PlayableChara chara = new PlayableChara(uid, name, airMoves, groundMoves, prefab);
                PlayerLogSource.LogInfo("Registering chara with no ID, assigned ID:");
                chara.id = GetNextFreeID();
                PlayerLogSource.LogInfo(chara.id);
                PlayableChars.Add(uid, chara);
                return true;
            }
            if (PlayableChars.ContainsKey(uid) && PlayableChars[uid].prefab == null)
            {
                PlayableChara chara = new PlayableChara(uid, name, airMoves, groundMoves, prefab);
                PlayerLogSource.LogInfo("Registering chara with existing ID, assigned ID:");
                chara.id = PlayableChars[uid].id;
                PlayerLogSource.LogInfo(chara.id);
                PlayableChars.Add(uid, chara);
                return true;
            }
            return false;
        }

        private static int GetNextFreeID()
        {
            int freeID = 5;
            foreach (PlayableChara chara in PlayableChars.Values)
            {
                if (chara.id >= freeID) freeID = chara.id + 1;
            }
            return freeID;
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
                        PlayableChara chara = new PlayableChara(data.UID, data.name, data.runtimeID);
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
                    chara.Uid,
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
