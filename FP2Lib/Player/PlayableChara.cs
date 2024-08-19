using System;
using UnityEngine;

namespace FP2Lib.Player
{
    public class PlayableChara
    {
        internal int id;

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

        public Action AirMoves;
        public Action GroundMoves;
        public Action ItemFuelPickup;

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
        /// <param name="gender"></param>
        /// <param name="airMoves"></param>
        /// <param name="groundMoves"></param>
        /// <param name="prefab"></param>
        /// <param name="dataBundle"></param>
        public PlayableChara(string uid, string name, CharacterGender gender, Action airMoves, Action groundMoves, GameObject prefab, AssetBundle dataBundle)
        {
            this.uid = uid;
            Name = name;
            AirMoves = airMoves;
            GroundMoves = groundMoves;
            Gender = gender;
            this.prefab = prefab;
            this.registered = true;
            this.dataBundle = dataBundle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="gender"></param>
        public PlayableChara(string uid,string name, int id, CharacterGender gender)
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
