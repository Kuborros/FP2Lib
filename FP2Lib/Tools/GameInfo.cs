using System;
using System.IO;
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
        /// <summary>
        /// Release of the game - Beta, Sample (Demos), Release.
        /// </summary>
        public GameRelease build;
        /// <summary>
        /// Game's version in form of C# Version object.
        /// </summary>
        public Version gameVersion;

        public GameInfo()
        {
            string version = Application.version;

            gameVersion = new Version(version.Remove(version.Length - 1));

            build = version.Substring(version.Length - 1) switch
            {
                "r" => GameRelease.RELEASE,
                "s" or "d" => GameRelease.SAMPLE,
                "b" => GameRelease.BETA,
                _ => GameRelease.RELEASE,
            };
        }

        /// <summary>
        /// Generates a string containing game version information.
        /// </summary>
        /// <returns>Game Version formatted into user-readable string</returns>
        public string getVersionString()
        {
            string buildName = build switch
            {
                GameRelease.RELEASE => "Release",
                GameRelease.SAMPLE => "Sample",
                GameRelease.BETA => "Beta",
                _ => "Unknown",
            };
            return string.Format("{0} {1}", gameVersion.ToString(), buildName);
        }

        /// <summary>
        /// Returns a path to a directory where save profile specific config files can be stored.
        /// Profile 0 (Default saves stored in AppData) will place a directory in the 'Saves\Profile0' instead of AppData, to ensure that these configs are easily user-accessible.
        /// </summary>
        /// <returns>Path to current save profile path for mod config data</returns>
        public static string getProfilePath()
        {
            Directory.CreateDirectory("Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString() + "\\ModData");
            return "Saves\\Profile" + FP2Lib.configSaveProfile.Value.ToString() + "\\ModData";

        }

    }
}
