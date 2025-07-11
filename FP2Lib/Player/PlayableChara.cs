﻿using System;
using UnityEngine;

namespace FP2Lib.Player
{
    public class PlayableChara
    {
        /// <summary>
        /// FPCharacterID. 
        /// </summary>
        public int id { get; internal set; }
        /// <summary>
        /// ID used for the character select wheel. Internal, not set by modder.
        /// </summary>
        internal int wheelId;
        /// <summary>
        /// 0 - Zao's Aiship, 1 - Cargo Airship,
        /// Anything else - Sprite provided by modder and added to vehicle array under said id.
        /// </summary>
        public int airshipSprite = 0;

        /// <summary>
        /// Unique Identifier
        /// </summary>
        public string uid;
        /// <summary>
        /// Character name
        /// </summary>
        public string Name = "";
        /// <summary>
        /// Tutorial scene name. Can be any of the built-in ones, but all non-Lilac ones require character-specific attacks to proceed.
        /// Can be a name of a custom scene loaded by the mod. As long as it has all the flags marking it as tutorial, all will behave as intended.
        /// </summary>
        public string TutorialScene = "Tutorial1";
        /// <summary>
        /// Description field for "SOMETHING Type"
        /// </summary>
        public string characterType;
        /// <summary>
        /// First button skill
        /// </summary>
        public string skill1;
        /// <summary>
        /// Second button skill
        /// </summary>
        public string skill2;
        /// <summary>
        /// Third button skill
        /// </summary>
        public string skill3;
        /// <summary>
        /// Fourth button skill
        /// </summary>
        public string skill4;
        /// <summary>
        /// Description shown in the item menus for the "Powerup Start" item.
        /// </summary>
        public string powerupStartDescription = "You begin the stage with your character's unique powerup.";

        /// <summary>
        /// Has the character been fully loaded? Internal, not set by modder.
        /// </summary>
        internal bool registered = false;
        /// <summary>
        /// Should the mod use it's own code and objects for all the cutscene, stage dialogue, and activator logic. 
        /// Enabling this disables automatic rerouting of all these functions to character selected in <c>eventActivatorCharacter</c>
        /// </summary>
        public bool useOwnCutsceneActivators;
        /// <summary>
        /// Should the character be listed in Adventure Mode character select.
        /// </summary>
        public bool enabledInAventure;
        /// <summary>
        /// Should the character be listed in Classic Mode character select. Good for characters you want injected into game code, but not available to manually choose.
        /// Defaults to true.
        /// </summary>
        public bool enabledInClassic = true;

        /// <summary>
        /// Method delegate which will get called on <c>Action_Character_AirMoves</c> in <c>FPPlayer</c>
        /// Perform all your airtime player logic here.
        /// </summary>
        public Action AirMoves;
        /// <summary>
        /// Method delegate which will get called on <c>Action_Character_GroundMoves</c> in <c>FPPlayer</c>
        /// Perform all your grounded player logic here.
        /// </summary>
        public Action GroundMoves;
        /// <summary>
        /// Method delegate which will get called on <c>ItemPickup</c> in <c>ItemFuel</c>
        /// Perform the code related to the powerup logic here.
        /// </summary>
        public Action ItemFuelPickup;
        //public Action CutsceneActivator;
        /// <summary>
        /// Method delegate called when <c>useOwnCutsceneActivators</c> is enabled, and <c>FPEventSequence</c> fires it's event start logic.
        /// Use this to run your own event logic.
        /// Current instance of <c>FPEventSequence</c> the method was fired in is provided for convenienience as first method argument.
        /// </summary>
        public Action<FPEventSequence> EventSequenceStart;

        /// <summary>
        /// ID of the *built-in* character which we should pretend to be for all of the event/cutscene/dialogue logic.
        /// Not used if <c>useOwnCutsceneActivators</c> is set to <c>True</c>.
        /// </summary>
        public FPCharacterID eventActivatorCharacter;
        /// <summary>
        /// Gender identity of your character, for purpose of dialogue and other mod logic. 
        /// By default it changes how the Battlesphere Announcer refers to your character.
        /// Defaults to Non-Binary they/them.
        /// </summary>
        public CharacterGender Gender = CharacterGender.NON_BINARY;
        /// <summary>
        /// Element your character represents. 
        /// Affects what damage type Element Burst generates.
        /// Defaults to "Fire" as this is the base game behaviour.
        /// </summary>
        public CharacterElement element = CharacterElement.FIRE;

        /// <summary>
        /// Character profile picture shown in File Select, Menu, and Pause screen
        /// </summary>
        public Sprite profilePic;
        /// <summary>
        /// Big Sprite used on character select wheel.
        /// Technically optional as you provide your own GameObject there, but some mods might wish to be able to load that sprite.
        /// </summary>
        public Sprite keyArtSprite = null;
        /// <summary>
        /// Big Sprite used in the credits.
        /// </summary>
        public Sprite endingKeyArtSprite;
        /// <summary>
        /// Sprite containing the name and "The something" for your character
        /// </summary>
        public Sprite charSelectName;
        /// <summary>
        /// Sprite for being stuck in Acrabelle's Pie
        /// </summary>
        public Sprite piedSprite;
        /// <summary>
        /// Sprite for being damaged in Acrabelle's Pie
        /// </summary>
        public Sprite piedHurtSprite;
        /// <summary>
        /// Sprite used for Zaoland baseball.
        /// </summary>
        public Sprite zaoBaseballSprite;
        /// <summary>
        /// Sprite used for Fuel Powerup.
        /// </summary>
        public Sprite itemFuel;
        /// <summary>
        /// Sprite shown next to character name in World Map's pause screen.
        /// </summary>
        public Sprite worldMapPauseSprite;

        /// <summary>
        /// 3 Frames for live icon animation. Frame 0 is shown for most of the time, frames 1 and 2 are used for the blinking animation.
        /// Frame 0 is also used for all the locations where icon of your character is shown (Menus, Arena Race, Shops, Character Select, File Menu, etc.)
        /// </summary>
        public Sprite[] livesIconAnim;
        /// <summary>
        /// World Map idle animation.
        /// </summary>
        public Sprite[] worldMapIdle;
        /// <summary>
        /// World Map walking animation.
        /// </summary>
        public Sprite[] worldMapWalk;

        /// <summary>
        /// Animator for Saga block capturing the player. It has to include animation called "TrapPlayer" which plays your character being trapped.
        /// </summary>
        public RuntimeAnimatorController sagaBlock;
        /// <summary>
        /// Animator for Syntax Saga block capturing the player. It has to include animation called "TrapPlayer" which plays your character being trapped.
        /// </summary>
        public RuntimeAnimatorController sagaBlockSyntax;

        /// <summary>
        /// Music track to play at the end of the stage.
        /// </summary>
        public AudioClip resultsTrack;
        /// <summary>
        /// Music track to play during the credits.
        /// </summary>
        public AudioClip endingTrack;

        /// <summary>
        /// MenuPhotoPose object with all your photo pose sprites set-up.
        /// </summary>
        public MenuPhotoPose menuPhotoPose;

        /// <summary>
        /// GameObject containing the character select wheel preview
        /// </summary>
        public GameObject characterSelectPrefab;

        /// <summary>
        /// Game object containing your playable FPPlayer character.
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// Default acceleration value for your character. Should be the same as set in the prefab.
        /// All base characters but Neera use 0.110625 here. Can be usually left at default.
        /// </summary>
        public float StatDefaultAcceleration = 0.110625f;
        /// <summary>
        /// Default air acceleration for your character. Should be the same as set in the prefab.
        /// All base-game characters use 0.22125 here. Can be usually left at default.
        /// </summary>
        public float StatDefaultAirAcceleration = 0.22125f;
        /// <summary>
        /// Default deceleration of your character. Should be the same as set in the prefab.
        /// The most common value is 0.110625. Can be usually left at default.
        /// </summary>
        public float StatDefaultDeceleration = 0.110625f;
        /// <summary>
        /// Default initial jump speed. Should be the same as set in the prefab.
        /// All base-game characters use 4.5 here. Can be usually left at default.
        /// </summary>
        public float StatDefaultJumpRelease = 4.5f;
        /// <summary>
        /// Set the default jump strength of your character here.
        /// All base-game characters use 10.5 here. Can be usually left at default.
        /// </summary>
        public float StatDefaultJumpStrength = 10.5f;
        /// <summary>
        /// Set the top speed value for your character. Should be the same as set in the prefab.
        /// All base-game characters use 7.5 here. Can be usually left at default.
        /// </summary>
        public float StatDefaultTopSpeed = 7.5f;

        /// <summary>
        /// Asset Bundle containing all the character data
        /// </summary>
        public AssetBundle dataBundle;


        public PlayableChara() { }
        internal PlayableChara(string uid, string name, int id, CharacterGender gender)
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
