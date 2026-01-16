using UnityEngine;

namespace FP2Lib.Stage
{
    public class CustomStage
    {
        /// <summary>
        /// Unique Identifier string for the stage.
        /// </summary>
        public string uid;
        /// <summary>
        /// Name of the stage.
        /// </summary>
        public string name;
        /// <summary>
        /// Description of the stage. Not shown in vanila game UI (it used to be during the beta!), but Zao's VR Arcade Mod (WIP) and other map loaders can show it.
        /// </summary>
        public string description;
        /// <summary>
        /// Name of the author. Same as description in terms of being shown.
        /// </summary>
        public string author;
        /// <summary>
        /// Version of the stage. Optional.
        /// </summary>
        public string version = "1.0.0";
        /// <summary>
        /// Should this stage be shown in other mods that list custom stages.
        /// It's not *enforced*, and is therefore just a suggestion to other mod that you wish to show this stage or not.
        /// </summary>
        public bool showInCustomStageLoaders = true;
        /// <summary>
        /// Par time for the stage.
        /// Stored in format used by FPStage.TimeToString();
        /// </summary>
        public int parTime;
        /// <summary>
        /// Name of the scene which contains the level.
        /// Must be a valid FP2 stage! Only minimal validation will be made!
        /// </summary>
        public string sceneName;
        /// <summary>
        /// Is the stage a HUB stage? These follow different rules with ID assignments.
        /// Hubs also do not track score, time, and such.
        /// </summary>
        public bool isHUB;
        /// <summary>
        /// The story flag belonging to the stage. Gets set on stage completion.
        /// </summary>
        public int storyFlag = 0;
        /// <summary>
        /// Stage ID used in save files and world map warps
        /// </summary>
        public int id = 0;
        /// <summary>
        /// Id of the vinyl collectable on the stage.
        /// <c>FPMusicTrack.NONE</c> for none.
        /// Ignored when vinylUID is set.
        /// </summary>
        public FPMusicTrack vinylID = FPMusicTrack.NONE;
        /// <summary>
        /// Item collectable on the stage.
        /// <c>FPPowerup.NONE</c> for no item.
        /// Ignored when itemUID is set.
        /// </summary>
        public FPPowerup itemID = FPPowerup.NONE;
        /// <summary>
        /// Set to UID of a custom track to automatically assign right vinylID at runtime.
        /// Has to be registered before the stage.
        /// </summary>
        public string vinylUID;
        /// <summary>
        /// Set to UID of a custom item to automatically assign right itemID at runtime.
        /// Has to be already registered.
        /// </summary>
        public string itemUID;
        /// <summary>
        /// Menu/Map preview sprite.
        /// </summary>
        public Sprite preview;
        /// <summary>
        /// Prefab used for the quickshop menu in the world maps. 
        /// Should be an object containing MenuClassicShopHub component and all it's sub-objects. 
        /// Despite the name it is also used in Adventure mode.
        /// Any menu-like GameObject will work here, so it doesn't have to actually be a Quick Shop menu if you want to be fancy.
        /// Null means no shop, or that you are using the NPC shop option below (shopkeeper).
        /// Used only for Hubs.
        /// </summary>
        public GameObject quickShop;
        /// <summary>
        /// On which story flag should the quickshop unlock.
        /// Default value of 0 will have it essentially always unlocked.
        /// If there is no shop set, this value is ignored.
        /// Used only for Hubs.
        /// </summary>
        public int quickShopStoryFlag = 0;
        /// <summary>
        /// NPC to take the shop from.
        /// This will also copy the shop over from the one in NPC's prefab.
        /// It directly opens the NPC's shop when clicking "Quick Shop" button on stage confirm menu, and sets up said menu for you.
        /// Used very scarcely (a single time) in the base game, as usually the shop menu is instanced from MenuClassicShopHub object instead.
        /// If you do that, then just leave this at Null.
        /// Null here and in quickShop means no shop.
        /// Used only for Hubs.
        /// </summary>
        public FPHubNPC shopkeeper;
        /// <summary>
        /// Is the stage fully initialised, or only pulled from storage?
        /// </summary>
        internal bool registered = false;

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="author"></param>
        /// <param name="version"></param>
        /// <param name="parTime"></param>
        /// <param name="sceneName"></param>
        /// <param name="isHUB"></param>
        /// <param name="id"></param>
        /// <param name="vinylID"></param>
        /// <param name="itemID"></param>
        /// <param name="preview"></param>
        public CustomStage(string uid, string name, string description, string author, string version, int parTime, string sceneName, bool isHUB, int id, FPMusicTrack vinylID, FPPowerup itemID, Sprite preview)
        {
            this.uid = uid;
            this.name = name;
            this.description = description;
            this.author = author;
            this.version = version;
            this.parTime = parTime;
            this.sceneName = sceneName;
            this.isHUB = isHUB;
            this.id = id;
            this.vinylID = vinylID;
            this.itemID = itemID;
            this.preview = preview;
        }

        /// <summary>
        /// Short constructor, for loading the basics from .json
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="isHUB"></param>
        /// <param name="id"></param>
        public CustomStage(string uid, string name, bool isHUB, int id)
        {
            this.uid = uid;
            this.name = name;
            this.isHUB = isHUB;
            this.id = id;
        }

        public CustomStage() { }

        /// <summary>
        /// Get StageData for this stage (used for storing stage details in json).
        /// </summary>
        /// <returns>StageData to be serialised</returns>
        public StageData getStageData()
        {
            return new StageData(uid, name, isHUB, id);
        }
    }
}
