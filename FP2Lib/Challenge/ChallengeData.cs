using JetBrains.Annotations;
using System;
using UnityEngine;

namespace FP2Lib.Challenge
{

    public enum FPChallengeType
    {
        /// <summary>
        /// Bosses to be added in the Battlesphere.
        /// </summary>
        BOSS,
        /// <summary>
        /// Bosses to be added in Shang Tu Dojo. Best used for sparring matches.
        /// </summary>
        DOJO_BOSS,
        /// <summary>
        /// General battlesphere challenge, this is where most of them should go.
        /// These challenges are listed in the credits, alongside their ranks and times.
        /// </summary>
        CHALLENGE,
        /// <summary>
        /// Challenges destined for Dojo. This menu is non-standard, and will require manual patches.
        /// </summary>
        DOJO_CHALLENGE,
        /// <summary>
        /// Home Run challenges are delegated to the Home Run menu
        /// </summary>
        HOMERUN,
        /// <summary>
        /// Functionally same as challenge in base game due to removal of the separate race menu.
        /// </summary>
        RACE,
        /// <summary>
        /// For challenges that should not be automatically added anywhere.
        /// Use this if you implement your own challenge menu.
        /// </summary>
        OTHER
    }

    [Serializable]
    public class ChallengeData
    {
        /// <summary>
        /// Internal id of the challenge
        /// </summary>
        public int id;

        /// <summary>
        /// Unique identifier of the challenge
        /// </summary>
        public string uid;

        /// <summary>
        /// ID used internally to track local menu position.
        /// </summary>
        [NonSerialized]
        internal int localID;
        internal int slotID;

        /// <summary>
        /// Name of the challenge, or the boss.
        /// </summary>
        public string name;

        /// <summary>
        /// Type of the challenge.
        /// While only bosses have unique fields, Home Run gets their own menus. 
        /// Race used to have it's own menu, but it was removed in development and now it shares it with Challenges.
        /// </summary>
        public FPChallengeType type;

        /// <summary>
        /// Scene which should be loaded
        /// Needs to have a valid ArenaSpawner/ArenaController at the destination
        /// That's up to you to set up properly!
        /// </summary>
        [NonSerialized]
        public string destinationScene;

        /// <summary>
        /// Reward in crystals for beating the challenge.
        /// Once beaten once, this value is divided by 4 - so don't set it to lower than that! Bad things will happen!
        /// </summary>
        [NonSerialized]
        public int crystalReward = 100;

        /// <summary>
        /// Amount of Star Cards needed to unlock this challenge/boss.
        /// Set this to -1 for 'always unlocked'.
        /// </summary>
        [NonSerialized]
        public int unlockRequirement = -1;

        //Challenge Specific

        /// <summary>
        /// Description of the challenge
        /// </summary>
        public string challengeDescription;

        /// <summary>
        /// Sprite used for the challenge in the challenge list
        /// </summary>
        [NonSerialized]
        public Sprite challengeIcon;

        /// <summary>
        /// Reward a Time Capsule. 
        /// Here it primarily just sets up a sprite to show in the rewards field, and marks it as collected if you already have it.
        /// Requires you to set the same Time Capsule up in the Arena Controller in the destination scene in order to *actually* give it as a reward.
        /// </summary>
        [NonSerialized]
        public int timeCapsuleID = -1;

        /// <summary>
        /// Custom reward sprite (like one for VIP Room).
        /// Leave as null to show either a Time Capsule (when selected above), or nothing.
        /// </summary>
        [NonSerialized]
        public Sprite rewardSprite;

        //Dojo Challenge Specific

        /// <summary>
        /// GameObject containing the 'preview' of the challenge seen above the list.
        /// Used exclusively in Gong's Dojo, can contain arbitrary amount of child objects
        /// </summary>
        [NonSerialized]
        public GameObject dojoChallengePreview;

        //Boss Specific

        /// <summary>
        /// Location where the boss was originally encountered.
        /// Used in the "Location: " text in the UI
        /// </summary>
        public string bossHome;

        /// <summary>
        /// Should the boss be foreshadowed before being unlocked (renders their picture black on the list)
        /// This *only* works with custom unblock conditions.
        /// </summary>
        [NonSerialized]
        public bool foreshadow;

        /// <summary>
        /// ID of the playable character this boss represents.
        /// Used for mirror mode UI changes like replacing their icon with Pangu.
        /// Leave it empty if you are not interested in this effect, and for non-player bosses.
        /// </summary>
        [NonSerialized]
        public FPCharacterID bossCharacterID;

        /// <summary>
        /// Sprite for the boss on the boss list
        /// Follows the same sizing format as character profile pictures, but with a different pivot point.
        /// </summary>
        [NonSerialized]
        public Sprite bossIcon;

        /// <summary>
        /// This method will be fired when checking for extra unlock requirements, other than the Star Cards one. 
        /// Use this if you want to, for example, do something like Askal's fight does.
        /// Return true for Unlocked, false for Locked.
        /// </summary>
        [NonSerialized]
        [CanBeNull]
        public Func<bool> CustomBossUnlockCheck;

        //Challenge Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="destinationScene"></param>
        /// <param name="crystalReward"></param>
        /// <param name="unlockRequirement"></param>
        /// <param name="challengeDescription"></param>
        /// <param name="challengeIcon"></param>
        /// <param name="timeCapsuleID"></param>
        /// <param name="rewardSprite"></param>
        public ChallengeData(string uid, string name, FPChallengeType type, string destinationScene, int crystalReward, int unlockRequirement, string challengeDescription, Sprite challengeIcon, int timeCapsuleID, Sprite rewardSprite)
        {
            this.uid = uid;
            this.name = name;
            this.type = type;
            this.destinationScene = destinationScene;
            this.crystalReward = crystalReward;
            this.unlockRequirement = unlockRequirement;
            this.challengeDescription = challengeDescription;
            this.challengeIcon = challengeIcon;
            this.timeCapsuleID = timeCapsuleID;
            this.rewardSprite = rewardSprite;
        }

        //Boss Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="destinationScene"></param>
        /// <param name="crystalReward"></param>
        /// <param name="unlockRequirement"></param>
        /// <param name="bossHome"></param>
        /// <param name="bossIcon"></param>
        public ChallengeData(string uid, string name, FPChallengeType type, string destinationScene, int crystalReward, int unlockRequirement, string bossHome, Sprite bossIcon)
        {
            this.uid = uid;
            this.name = name;
            this.type = type;
            this.destinationScene = destinationScene;
            this.crystalReward = crystalReward;
            this.unlockRequirement = unlockRequirement;
            this.bossHome = bossHome;
            this.bossIcon = bossIcon;
        }

        //Dojo Boss Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="destinationScene"></param>
        /// <param name="crystalReward"></param>
        /// <param name="unlockRequirement"></param>
        /// <param name="bossHome"></param>
        /// <param name="bossCharacterID"></param>
        /// <param name="bossIcon"></param>
        public ChallengeData(string uid, string name, FPChallengeType type, string destinationScene, int crystalReward, int unlockRequirement, string bossHome, FPCharacterID bossCharacterID, Sprite bossIcon)
        {
            this.uid = uid;
            this.name = name;
            this.type = type;
            this.destinationScene = destinationScene;
            this.crystalReward = crystalReward;
            this.unlockRequirement = unlockRequirement;
            this.bossHome = bossHome;
            this.bossCharacterID = bossCharacterID;
            this.bossIcon = bossIcon;
        }

        //Dojo Challenge Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="destinationScene"></param>
        /// <param name="crystalReward"></param>
        /// <param name="challengeDescription"></param>
        /// <param name="challengeIcon"></param>
        /// <param name="timeCapsuleID"></param>
        /// <param name="dojoPreview"></param>
        public ChallengeData(string uid, string name, FPChallengeType type, string destinationScene, int crystalReward, string challengeDescription, Sprite challengeIcon, int timeCapsuleID, GameObject dojoPreview)
        {
            this.uid = uid;
            this.name = name;
            this.type = type;
            this.destinationScene = destinationScene;
            this.crystalReward = crystalReward;
            this.challengeDescription = challengeDescription;
            this.challengeIcon = challengeIcon;
            this.timeCapsuleID = timeCapsuleID;
            this.dojoChallengePreview = dojoPreview;
        }

        public ChallengeData() {}

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
