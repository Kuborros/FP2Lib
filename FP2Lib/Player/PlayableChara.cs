using System;
using UnityEngine;

namespace FP2Lib.Player
{
    public class PlayableChara
    {
        public string Uid;
        public string Name;
        public CharacterGender Gender;
        public bool registered;

        internal Delegate AirMoves;
        internal Delegate GroundMoves;

        public bool useOwnCutsceneActivators;
        internal FPCharacterID eventActivatorCharacter;

        public Sprite profilePic;
        public Sprite keyArtSprite;
        public Sprite[] livesIconAnim;

        public AudioClip resultsTrack;
        public AudioClip endingTrack;

        public int id;
        internal GameObject prefab;
        internal GameObject runtimeObject;
        internal AssetBundle dataBundle;

        public PlayableChara(string uid, string name, CharacterGender gender, Delegate airMoves, Delegate groundMoves, GameObject prefab, AssetBundle dataBundle)
        {
            Uid = uid;
            Name = name;
            AirMoves = airMoves;
            GroundMoves = groundMoves;
            Gender = gender;
            this.prefab = prefab;
            this.registered = true;
            this.dataBundle = dataBundle;
        }

        public PlayableChara(string uid,string name, int id, CharacterGender gender)
        {
            Uid = uid;
            Name = name;
            Gender = gender;
            this.id = id;
            registered = false;
        }

        internal CharacterData GetCharacterData()
        {
            return new CharacterData(Uid, id, Name, Gender);
        }

    }
}
