using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPHubNPCPlayer
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPHubNPC), "GetSpeakerName", MethodType.Normal)]
        static void PatchGetSpeakerName(ref string __result, NPCDialog[] ___dialog, int ___currentDialog, int ___currentLine, FPPlayer ___targetPlayer)
        {
            //Modded characters need to show their name here
            if(___dialog[___currentDialog].lines[___currentLine].playerIsSpeaking && ___targetPlayer.characterID > FPCharacterID.NEERA)
            {
                PlayableChara character = PlayerHandler.GetPlayableCharaByFPCharacterId(___targetPlayer.characterID);
                if (character != null)
                {
                    __result = character.Name;
                }
            }
        }
    }
}
