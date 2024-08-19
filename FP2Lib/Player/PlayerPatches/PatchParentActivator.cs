using HarmonyLib;
using System;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchParentActivator
    {

        /// <summary>
        /// Which characters should it be active for? Put true/false under your character's ID here!
        /// </summary>
        public static bool[] activeCharacters;

        //Full Rewrite of the Parent Activator starter. Base game uses 4 booleans to decide who gets to see it.
        //Instead, we use an array with bool for each character. 
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ParentActivator), "Start", MethodType.Normal)]
        static bool PatchPAStart(ParentActivator __instance)
        {
            //If no extra characters are loaded, abort patch.
            if (PlayerHandler.PlayableChars.Count == 0)
            {
                return true;
            }
            
            //Clear static array. 
            activeCharacters = new bool[PlayerHandler.highestID];

            if (__instance.disableInClassicMode && FPSaveManager.gameMode == FPGameMode.CLASSIC)
            {
                DisableObject(__instance);
            }

            //Code to populate default characters
            activeCharacters[0] = __instance.lilac;
            activeCharacters[1] = __instance.carol;
            activeCharacters[2] = __instance.carol; //Bike Carol
            activeCharacters[3] = __instance.milla;
            activeCharacters[4] = __instance.neera;

            foreach (PlayableChara character in PlayerHandler.PlayableChars.Values)
            {
                if (!character.useOwnCutsceneActivators)
                {
                    switch (character.eventActivatorCharacter)
                    {
                        case FPCharacterID.LILAC:
                            if (activeCharacters[0]) activeCharacters[character.id] = true;
                            break;
                        case FPCharacterID.CAROL:
                        case FPCharacterID.BIKECAROL:
                            if (activeCharacters[1] || activeCharacters[2]) activeCharacters[character.id] = true;
                            break;
                        case FPCharacterID.MILLA:
                            if (activeCharacters[3]) activeCharacters[character.id] = true;
                            break;
                        case FPCharacterID.NEERA:
                            if (activeCharacters[4]) activeCharacters[character.id] = true;
                            break;
                        default:
                            activeCharacters[character.id] = false; 
                            break;
                    }
                }
            }
            //If characterID has false set, deactivate the activator.
            if (!activeCharacters[(int)FPSaveManager.character])
            {
                DisableObject(__instance);
            }

            //If story disables the object, nuke it
            if (__instance.requiredStoryFlag > 0 && FPSaveManager.storyFlag[__instance.requiredStoryFlag] <= 0)
            {
                if (!(__instance.restoreAtStoryFlag > 0 && FPSaveManager.storyFlag[__instance.restoreAtStoryFlag] > 0))
                {
                    if (__instance.destroyForCharactersOnly)
                    {
                        __instance.activationMode = FPActivationMode.NEVER_ACTIVE;
                    }
                    else
                    {
                        DisableObject(__instance);
                    }
                }
            }
            if (__instance.disableAtStoryFlag > 0 && FPSaveManager.storyFlag[__instance.disableAtStoryFlag] > 0)
            {
                if (!(__instance.restoreAtStoryFlag > 0 && FPSaveManager.storyFlag[__instance.restoreAtStoryFlag] > 0))
                {
                    if (__instance.destroyForCharactersOnly)
                    {
                        __instance.activationMode = FPActivationMode.NEVER_ACTIVE;
                    }
                    else
                    {
                        DisableObject(__instance);
                    }
                }
            }
            //Start(__instance); 
            ParentActivator.classID = FPStage.RegisterObjectType(__instance, __instance.GetType(), 0);
            __instance.objectID = ParentActivator.classID;

            return false;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ParentActivator), "DisableObject", MethodType.Normal)]
        public static void DisableObject(ParentActivator instance)
        {
            // Replaced at runtime with reverse patch
            throw new NotImplementedException("Method failed to reverse patch!");
        }
    }
}
