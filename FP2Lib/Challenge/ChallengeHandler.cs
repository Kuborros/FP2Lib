using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FP2Lib.Challenge
{
    public class ChallengeHandler
    {

        private static readonly ManualLogSource ArenaLogSource = FP2Lib.logSource;

        public static readonly int baseChallenges = 85;

        internal static Dictionary<string, ChallengeData> Challenges = [];
        internal static bool[] takenIDs = new bool[256];

        internal static void InitialiseHandler()
        {
            if (!File.Exists(GameInfo.getProfilePath() + "/ChallengeStore.json"))
                File.Create(GameInfo.getProfilePath() + "/ChallengeStore.json").Close();

            //Mark first 85 challenges as taken.
            for (int i = 0; i < baseChallenges; i++)
            {
                takenIDs[i] = true;
            }

            LoadFromStorage();
        }


        public bool RegisterChallenge()
        {
            return false;
        }

        public bool RegisterBoss()
        {
            return false;
        }

        public bool RegisterDojoBoss()
        {
            return false;
        }


        public bool RegisterChallengeDirect(ChallengeData challenge)
        {
            string uid = challenge.uid;
            if (!Challenges.ContainsKey(uid))
            {
                challenge.id = AssignChallengeID(challenge);
                Challenges.Add(uid, challenge);
                return true;
            }
            else if (Challenges.ContainsKey(uid) && (Challenges[uid].challengeIcon == null || Challenges[uid].bossIcon == null))
            {
                Challenges[uid] = challenge;
                Challenges[uid].id = AssignChallengeID(Challenges[uid]);
                return true;
            }
            return false;
        }

        private static int AssignChallengeID(ChallengeData challenge)
        {
            //Challenge already has ID
            if (challenge.id != 0 && Challenges.ContainsKey(challenge.uid))
            {
                ArenaLogSource.LogDebug("Stored challenge ID assigned (" + challenge.uid + "): " + challenge.id);
                return challenge.id;
            }
            else
            {
                ArenaLogSource.LogDebug("Challenge with unassigned ID registered! Running assignment process for " + challenge.uid);
                //Iterate over array, assign first non-taken slot
                for (int i = 85; i < takenIDs.Length; i++)
                {
                    //First slot with false = empty space
                    if (!takenIDs[i])
                    {
                        challenge.id = i;
                        takenIDs[i] = true;
                        ArenaLogSource.LogDebug("ID assigned:" + challenge.id);
                        //Will also break loop
                        return challenge.id;
                    }
                }
            }
            ArenaLogSource.LogWarning("Challenge: " + challenge.uid + " failed ID assignment! That's *very* bad!");
            return 0;
        }


        [CanBeNull]
        public ChallengeData GetChallengeDataByUID(string uid)
        {
            if (Challenges.ContainsKey(uid)) return Challenges[uid];
            else return null;
        }

        //Did you know, Unity's JSON parser detonates if the root object is an array? And that it struggles _so much_ with arrays?
        //This cursed stuff is the easiest way to make it not break.
        private static void LoadFromStorage()
        {
            string json = File.ReadAllText(GameInfo.getProfilePath() + "/ChallengeStore.json");
            if (json.IsNullOrWhiteSpace()) return;

            string[] challengeJson = json.Split(new string[] { "<sep>" }, StringSplitOptions.None);
            foreach (string challengeString in challengeJson)
            {
                ChallengeData challenge = ChallengeData.LoadFromJson(challengeString);

                ArenaLogSource.LogDebug("Loaded challenge from storage: " + challenge.name + "(" + challenge.id + ")");
                if (!Challenges.ContainsKey(challenge.uid))
                {
                    Challenges.Add(challenge.uid, challenge);
                    //Extend array if needed
                    if (challenge.id > takenIDs.Length)
                        takenIDs = FPSaveManager.ExpandBoolArray(takenIDs, challenge.id);
                    //Mark id as taken
                    takenIDs[challenge.id] = true;
                }
            }
        }

        internal static void WriteToStorage()
        {
            if (Challenges.Values.Count == 0) return;

            string json = "";

            foreach (ChallengeData challenge in Challenges.Values)
            {
                json += challenge.WriteToJson();
                json += "<sep>\n";
            }

            json = json.Remove(json.Length - 6);
            json += "";

            try
            {
                byte[] bytes = new UTF8Encoding().GetBytes(json);
                using FileStream fileStream = new(string.Concat(new object[]
                {
                    GameInfo.getProfilePath(),
                    "/",
                    "ChallengeStore",
                    ".json"
                }), FileMode.Create, FileAccess.Write, FileShare.Read, bytes.Length, FileOptions.WriteThrough);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
            catch (Exception e)
            {
                ArenaLogSource.LogError(e);
            }
        }

    }
}
