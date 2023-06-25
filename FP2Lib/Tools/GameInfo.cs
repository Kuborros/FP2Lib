using System;
using UnityEngine;

namespace FP2Lib.Tools
{
    public enum GameRelease
    {
        SAMPLE,
        RELEASE,
        BETA
    }

    public class GameInfo
    {
        public GameRelease build;
        public Version gameVersion;


        public GameInfo() 
        {
            string version = Application.version;

            gameVersion = new Version(version.Remove(version.Length -1));
            
            switch (version.Substring(version.Length - 1))
            {
                case "r":
                    build = GameRelease.RELEASE; break;
                case "s":
                case "d":
                    build = GameRelease.SAMPLE; break;
                case "b":
                    build = GameRelease.BETA; break;
                default:
                    build = GameRelease.RELEASE; break;
            }
        }

        public string getVersionString()
        {
            string buildName = build switch
            {
                GameRelease.RELEASE => "Release",
                GameRelease.SAMPLE => "Sample",
                GameRelease.BETA => "Beta",
                _ => "Unknown",
            };
            return string.Format("{0} {1}",gameVersion.ToString(),buildName);
        }
    }
}
