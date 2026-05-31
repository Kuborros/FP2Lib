using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FP2Lib.Challenge
{

    public enum FPChallengeType
    {
        BOSS,
        CHALLENGE,
        HOMERUN,
        RACE
    }

    [Serializable]
    class ChallengeData
    {
        /// <summary>
        /// 
        /// </summary>
        public int id;

        /// <summary>
        /// Unique identifier of the challenge
        /// </summary>
        public string uid;

        /// <summary>
        /// 
        /// </summary>
        public string name;

        /// <summary>
        /// Scene which should be loaded
        /// Needs to have a valid ArenaSpawner/ArenaController at the destination
        /// </summary>
        public string destinationScene;

        /// <summary>
        /// 
        /// </summary>
        public int crystalReward = 100;

        /// <summary>
        /// 
        /// </summary>
        public bool rewardTimeCapsule;

        /// <summary>
        /// 
        /// </summary>
        public int timeCapsuleID;

        /// <summary>
        /// 
        /// </summary>
        public bool spawnAllies;

        /// <summary>
        /// 
        /// </summary>
        public bool alliesAreHostile;

        /// <summary>
        /// 
        /// </summary>
        public bool disableCorePickups;

        /// <summary>
        /// 
        /// </summary>
        public FPBaseObject[] spawnAtStart;

        /// <summary>
        /// 
        /// </summary>
        public ArenaRoundSpawnList[] roundObjectList;

        /// <summary>
        /// 
        /// </summary>
        public float[] spawnDelay;

        /// <summary>
        /// 
        /// </summary>
        public string endCutscene;

        /// <summary>
        /// 
        /// </summary>
        public float victoryDelayOffset;

        //Challenge Specific

        /// <summary>
        /// Description of the challenge
        /// </summary>
        public string description;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public Sprite challengeIcon;

        //Boss Specific

        /// <summary>
        /// 
        /// </summary>
        public string bossName;

        /// <summary>
        /// 
        /// </summary>
        public string bossHome;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public Sprite bossPFP;







        internal static ChallengeData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<ChallengeData>(json);
        }

        internal string WriteToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
