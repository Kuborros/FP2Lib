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

            build = version.Substring(version.Length - 1) switch
            {
                "r" => GameRelease.RELEASE,
                "s" or "d" => GameRelease.SAMPLE,
                "b" => GameRelease.BETA,
                _ => GameRelease.RELEASE,
            };
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
