using System;
using UnityEngine;

namespace FP2Lib.NPC
{
    /*
     * {
     *  "UID":"",
     *  "name":"",
     *  "scene":"",
     *  "bundlePath":"",
     *  "species":0,
     *  "home":0,
     *  "dialogue":1
     * }
     */



    [Serializable]
    internal class EzModeData
    {
        public string UID;
        public string name;
        public string scene;
        public string bundlePath;
        public int species;
        public int home;
        public int dialogue;

        internal EzModeData(string uid, string name,string scene, string bundlePath, int species, int home, int dialogue)
        {
            this.UID = uid;
            this.name = name;
            this.scene = scene;
            this.bundlePath = bundlePath;
            this.species = species;
            this.home = home;
            this.dialogue = dialogue;
        }

        internal static EzModeData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<EzModeData>(json);
        }
    }
}
