using UnityEngine;

namespace FP2Lib.Stage
{
    internal class CustomStage
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
        /// Description of the stage. Not shown in vanila game UI, but Zao's VR Arcade Mod (WIP) and other map loaders can show it.
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
        /// Stage ID used in save files and world map warps
        /// </summary>
        public int id = 0;
        /// <summary>
        /// Id of the vinyl collectable on the stage.
        /// FPMusicTrack.NONE for none.
        /// Ignored when vinylUID is set.
        /// </summary>
        public FPMusicTrack vinylID;
        /// <summary>
        /// Item collectable on the stage.
        /// <c>FPPowerup.NONE</c> for no item.
        /// Ignored when itemUID is set.
        /// </summary>
        public FPPowerup itemID;
        /// <summary>
        /// Set to UID of a custom track to automatically assign right vinylID at runtime.
        /// </summary>
        public string vinylUID;
        /// <summary>
        /// Set to UID of a custom item to automatically assign right itemID at runtime. (WIP)
        /// </summary>
        public string itemUID;
        /// <summary>
        /// Menu/Map preview sprite.
        /// </summary>
        public Sprite sprite;
        /// <summary>
        /// Is the stage fully initialised, or only pulled from storage?
        /// </summary>
        internal bool registered = false;

        public CustomStage(string uid, string name, string description, string author, string version, int parTime, string sceneName, int id, FPMusicTrack vinylID, FPPowerup itemID, Sprite sprite)
        {
            this.uid = uid;
            this.name = name;
            this.description = description;
            this.author = author;
            this.version = version;
            this.parTime = parTime;
            this.sceneName = sceneName;
            this.id = id;
            this.vinylID = vinylID;
            this.itemID = itemID;
            this.sprite = sprite;
        }

        public CustomStage(string uid, string name, int id)
        {
            this.uid = uid;
            this.name = name;
            this.id = id;
        }

        public StageData getStageData()
        {
            return new StageData(uid,name,id);
        }
    }
}
