﻿using System;
using UnityEngine;

namespace FP2Lib.Player
{
    public class PlayableChara
    {
        public string Uid;
        public string Name;
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

        public PlayableChara(string uid, string name, Delegate airMoves, Delegate groundMoves, GameObject prefab)
        {
            Uid = uid;
            Name = name;
            AirMoves = airMoves;
            GroundMoves = groundMoves;
            this.prefab = prefab;
            this.registered = true;
        }

        public PlayableChara(string uid,string name, int id)
        {
            Uid = uid;
            Name = name;
            this.id = id;
            registered = false;
        }

        internal CharacterData GetCharacterData()
        {
            return new CharacterData(Uid, id, Name);
        }

    }
}
