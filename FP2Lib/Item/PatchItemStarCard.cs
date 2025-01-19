using FP2Lib.Player;
using HarmonyLib;
using System.Linq;

namespace FP2Lib.Item
{
    internal class PatchItemStarCard
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemStarCard), "Start", MethodType.Normal)]
        static void PatchStart(ref FPCharacterID[] ___disableForCharacter)
        {
            // Only run this check if the Star Card is set to be disabled for at least one character.
            if (___disableForCharacter.Length == 0)
                return;

            // Loop through each character. If they're registered, check if the original disabled array contains their event base. If so, add them to the list.
            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
                if (chara.registered)
                    if (___disableForCharacter.Contains(chara.eventActivatorCharacter))
                        ___disableForCharacter = ___disableForCharacter.AddToArray((FPCharacterID)chara.id);
        }
    }
}
