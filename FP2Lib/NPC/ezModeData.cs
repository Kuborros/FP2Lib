using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FP2Lib.NPC
{
    [System.Serializable]
    internal class ezModeData
    {
        public string UID;
        public string name;
        public string scene;
        public string bundlePath;
        public int species;
        public int home;
        public int dialogue;

        internal ezModeData(string uid, string name,string scene, string bundlePath, int species, int home, int dialogue)
        {
            this.UID = uid;
            this.name = name;
            this.scene = scene;
            this.bundlePath = bundlePath;
            this.species = species;
            this.home = home;
            this.dialogue = dialogue;



        }

        internal static ezModeData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<ezModeData>(json);
        }
    }
}
