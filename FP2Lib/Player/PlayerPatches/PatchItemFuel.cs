using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchItemFuel
    {

        internal static readonly MethodInfo m_ItemFuelHandler = SymbolExtensions.GetMethodInfo(() => ItemFuelHandler());

        private static void ItemFuelHandler()
        {
            PlayerHandler.currentCharacter.ItemFuelPickup?.Invoke();
        }

        //ItemFuel needs sprite and code per each character.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemFuel), "Start", MethodType.Normal)]
        static void PatchStart(ref Sprite[] ___iconSprite)
        {
            for (int i = 4; i < PlayerHandler.highestID; i++)
            {
                ___iconSprite = ___iconSprite.AddToArray(null);
            }

            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                if (chara.registered)
                    ___iconSprite[chara.id] = chara.itemFuel;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(ItemFuel), "CollisionCheck", MethodType.Normal)]
        static IEnumerable<CodeInstruction> ItemFuelTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label entryLabel = il.DefineLabel();
            Label exitLabel = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_2)
                {
                    exitLabel = (Label)codes[i + 1].operand;
                    codes[i + 1].operand = entryLabel;
                }
            }
            CodeInstruction entry = new CodeInstruction(OpCodes.Ldarg_0);
            entry.labels.Add(entryLabel);
            codes.Add(entry);
            codes.Add(new CodeInstruction(OpCodes.Callvirt, m_ItemFuelHandler));
            codes.Add(new CodeInstruction(OpCodes.Br, exitLabel));
            return codes;
        }
    }
}
