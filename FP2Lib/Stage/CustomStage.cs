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
        public Sprite preview;
        /// <summary>
        /// Is the stage fully initialised, or only pulled from storage?
        /// </summary>
        internal bool registered = false;

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="isHUB"></param>
        /// <param name="id"></param>
        public CustomStage(string uid, string name,bool isHUB, int id)
        {
            this.uid = uid;
            this.name = name;
            this.isHUB = isHUB;
            this.id = id;
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomStage() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StageData getStageData()
        {
            return new StageData(uid,name,isHUB,id);
        }
    }
}
