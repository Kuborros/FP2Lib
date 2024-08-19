using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuCharacterSelect
    {
        internal static readonly MethodInfo m_getRealTotalCharacterNumber = SymbolExtensions.GetMethodInfo(() => PlayerHandler.GetRealTotalCharacterNumber());

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCharacterSelect), "Start", MethodType.Normal)]
        static void PatchCharacterSelectStart(MenuCharacterSelect __instance, ref MenuCharacterWheel[] ___characterSprites, ref Sprite[] ___nameLabelSprites, ref MenuText[] ___infoText)
        {
            //Calculate offset
            int totalCharacters = PlayerHandler.GetTotalActiveCharacters();
            int offset = 360 / totalCharacters;


            //Extend arrays
            for (int i = 5; i < PlayerHandler.highestID; i++)
            {
                ___characterSprites = ___characterSprites.AddToArray(null);
                ___nameLabelSprites = ___nameLabelSprites.AddToArray(null);
                ___infoText[0].paragraph = ___infoText[0].paragraph.AddToArray("");
                ___infoText[1].paragraph = ___infoText[1].paragraph.AddToArray("");
                ___infoText[2].paragraph = ___infoText[2].paragraph.AddToArray("");
                ___infoText[3].paragraph = ___infoText[3].paragraph.AddToArray("");
                ___infoText[4].paragraph = ___infoText[4].paragraph.AddToArray("");
            }

            //Inject all characters
            foreach (PlayableChara chara in PlayerHandler.PlayableChars.Values)
            {
                //No character file, skip them.
                if (chara.prefab == null) continue;

                GameObject charSelector = GameObject.Instantiate(chara.characterSelectPrefab);
                MenuCharacterWheel wheel = charSelector.GetComponent<MenuCharacterWheel>();
                //Calculate offset for the wheel.
                //Maybe this will just work?
                wheel.rotationOffset = 180 - (offset * chara.id);

                wheel.parentObject = __instance;
                wheel.gameObject.transform.parent = __instance.transform;
                ___characterSprites = ___characterSprites.AddToArray(wheel);

                ___nameLabelSprites = ___nameLabelSprites.AddToArray(chara.charSelectName);

                ___infoText[0].paragraph[chara.id] = chara.characterType;
                ___infoText[1].paragraph[chara.id] = chara.skill1;
                ___infoText[2].paragraph[chara.id] = chara.skill2;
                ___infoText[3].paragraph[chara.id] = chara.skill3;
                ___infoText[4].paragraph[chara.id] = chara.skill4;

            }
        }

        //Bump the maximum number of characters
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuCharacterSelect), "State_SelectCharacter", MethodType.Normal)]
        static IEnumerable<CodeInstruction> CharacterSelectTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_3)
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, m_getRealTotalCharacterNumber);
                }
            }
            return codes;
        }


        //Patch digitframes to show live icon of selected character
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuCharacterSelect), "State_CharacterConfirm", MethodType.Normal)]
        static void PatchCharacterConfirm(MenuCharacterSelect __instance, ref FPHudDigit ___characterIcon)
        {
            //Since BikeCarol is not counted here, all id's need to be +1'd
            if (__instance.character >= 4)
            {
                ___characterIcon.digitFrames = ___characterIcon.digitFrames.AddToArray(PlayerHandler.GetPlayableCharaByRuntimeId(__instance.character + 1).livesIconAnim[0]);
                ___characterIcon.SetDigitValue(16);
            }
        }

        /*
         * Might not be needed? Instead of editing the data _before_ the jump to State_Go, maybe just edit it in Prefix?
         * 
        //Patch code to write proper character ID to file
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuCharacterSelect), "State_CharacterConfirm", MethodType.Normal)]
        static IEnumerable<CodeInstruction> CharacterConfirmTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label spadeSelect = il.DefineLabel();
            Label endBr = il.DefineLabel();

            System.Object staticVal = null;
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                //Save file character id write
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_S)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    staticVal = codes[i + 3].operand;
                    codes[i + 1].labels.Add(endBr);
                    targets = targets.AddItem(spadeSelect).ToArray();
                    codes[i].operand = targets;
                }

            }

            //SaveFile magic
            CodeInstruction idCodeStart = new CodeInstruction(OpCodes.Ldc_I4_5);
            idCodeStart.labels.Add(spadeSelect);

            codes.Add(idCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Stsfld, staticVal));
            codes.Add(new CodeInstruction(OpCodes.Br, endBr));

            return codes;

        }
        */
        //Might just work like this instead?
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCharacterSelect), "State_Go", MethodType.Normal)]
        static void PatchMenuCharacterSelectGo(int ___character)
        {
            if (___character >= 4)
            {
                FPSaveManager.character = (FPCharacterID)(___character + 1);
                PlayerHandler.currentCharacter = PlayerHandler.GetPlayableCharaByFPCharacterId(FPSaveManager.character);
            }
        }
    }
}
