using System.Collections.Generic;

namespace FP2Lib.Saves
{
    public class SaveRedirectHandler
    {
        public static string CurrentModRedirectProfile { get; internal set; }
        internal static int currentPriority;
        internal static Dictionary<string,int> modRedirects;


        internal static void InitialiseHandler() 
        {
            modRedirects = new Dictionary<string,int>();
            currentPriority = 0;
            CurrentModRedirectProfile = "";
        }

        /// <summary>
        /// Setup a Save Profile redirect. These take precedence over user-selected profile.
        /// This allows your mod to hold a completely separate profile with it's own item and badge IDs. 
        /// Useful for randomizers and similar mods which want to avoid ever messing with player's usual save profiles.
        /// Priority lets you decide how important your redirect is - if you want to be certain you get your folder picked, put a higher number here. Default is 100.
        /// </summary>
        /// <param name="profileName">Name of your save profile. The final folder name will be <c>Profile_YourText</c>.</param>
        /// <param name="priority">Priority of your profile. Higher number means higher priority.</param>
        /// <returns></returns>
        public static bool RequestProfileRedirect(string profileName, int priority = 100)
        {
            modRedirects.Add(profileName, priority);
            if (currentPriority <= priority)
            {
                CurrentModRedirectProfile = profileName;
                FP2Lib.logSource.LogDebug("Set new save profile redirect to: " + profileName);
                return true;
            }
            else
            {
                FP2Lib.logSource.LogDebug("Profile " + profileName + " has lower priority than current one, skipping.");
                return false;
            }
        }
    }
}
