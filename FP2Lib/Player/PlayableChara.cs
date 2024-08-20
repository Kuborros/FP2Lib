using System;
using UnityEngine;

namespace FP2Lib.Player
{
    public class PlayableChara
    {
        internal int id;
        internal int wheelId;

        public string uid;
        public string Name;
        public string TutorialScene = "Tutorial1";
        public string characterType;
        public string skill1;
        public string skill2;
        public string skill3;
        public string skill4;

        internal bool registered;
        public bool useOwnCutsceneActivators;
        public bool enabledInAventure;

        public Action AirMoves;
        public Action GroundMoves;
        public Action ItemFuelPickup;
        //public Action CutsceneActivator;
        public Action<FPEventSequence> EventSequenceStart;

        public FPCharacterID eventActivatorCharacter;
        public CharacterGender Gender;

        public Sprite profilePic;
        public Sprite keyArtSprite;
        public Sprite endingKeyArtSprite;
        public Sprite charSelectName;
        public Sprite piedSprite;
        public Sprite piedHurtSprite;
        public Sprite itemFuel;
        public Sprite worldMapPauseSprite;

        public Sprite[] livesIconAnim;
        public Sprite[] worldMapIdle;
        public Sprite[] worldMapWalk;

        public RuntimeAnimatorController sagaBlock;
        public RuntimeAnimatorController sagaBlockSyntax;

        public AudioClip resultsTrack;
        public AudioClip endingTrack;

        public MenuPhotoPose menuPhotoPose;

        public GameObject characterSelectPrefab;
        public GameObject prefab;
        internal GameObject runtimeObject;

        public AssetBundle dataBundle;

        /// <summary>
        /// Allows creating the object by hand without passing constructor params
        /// </summary>
        public PlayableChara() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="gender"></param>
        internal PlayableChara(string uid,string name, int id, CharacterGender gender)
        {
            this.uid = uid;
            Name = name;
            Gender = gender;
            this.id = id;
            registered = false;
        }

        internal CharacterData GetCharacterData()
        {
            return new CharacterData(uid, id, Name, Gender);
        }

    }
}
