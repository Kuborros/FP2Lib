using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuWorldMap
    {
        public static bool CutsceneCheckExtended(MenuWorldMap instance, int i, int l)
        {
            PlayableChara character = PlayerHandler.currentCharacter;
            if (character != null)
            {
                if (character.useOwnCutsceneActivators)
                {
                    if (instance.cutscenes[i].dialogSequence[l].characters.Length >= character.id)
                        return instance.cutscenes[i].dialogSequence[l].characters[character.id];
                    else return false;
                }
                else
                {
                    return instance.cutscenes[i].dialogSequence[l].characters[(int)character.eventActivatorCharacter];
                }
            }
            return false;
        }

        public static bool CutsceneCheckCharBase()
        {
            if (FPSaveManager.character > FPCharacterID.NEERA) return true;
            else return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuWorldMap), "SetPlayerSprite", MethodType.Normal)]
        private static bool PatchSetPlayerSprite(bool walking, bool ___vehicleMode, ref SpriteRenderer ___playerSpriteRenderer, ref SpriteRenderer ___playerShadowRenderer, FPMap[] ___renderedMap, int ___currentMap, float ___animTimer)
        {
            //Return for non-customs
            if (FPSaveManager.character < (FPCharacterID)5) return true;

            PlayableChara chara = PlayerHandler.currentCharacter;

            if (___vehicleMode)
            {
                ___playerSpriteRenderer.sprite = ___renderedMap[___currentMap].vehicle[chara.airshipSprite];
                if (___renderedMap[___currentMap].waterVehicle)
                {
                    ___playerShadowRenderer.sprite = null;
                    ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    ___playerShadowRenderer.sprite = ___playerSpriteRenderer.sprite;
                    ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, Mathf.Sin(0.017453292f * FPStage.platformTimer * 2f) * 4f, 0f);
                }
            }
            else if (walking)
            {
                ___playerSpriteRenderer.sprite = chara.worldMapWalk[((int)(___animTimer) % chara.worldMapWalk.Length)];
                ___playerShadowRenderer.sprite = null;
                ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            else
            {
                ___playerSpriteRenderer.sprite = chara.worldMapIdle[Mathf.Min((int)((___animTimer) % chara.worldMapIdle.Length), chara.worldMapIdle.Length - 1)];
                ___playerShadowRenderer.sprite = null;
                ___playerSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
            }

            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuWorldMap), "SearchForDialog", MethodType.Normal)]
        static void PatchSearchForDialog(FPMapLocation current, FPMapDialog[] dialogArray, ref MenuWorldMap __instance)
        {
            if (FPSaveManager.character <= FPCharacterID.NEERA || PlayerHandler.currentCharacter == null) return;

            if (dialogArray.Length >= current.pointers.stageID)
            {
                DialogCall[] dialogSequence = dialogArray[current.pointers.stageID].dialogSequence;
                int sceneID = dialogArray[current.pointers.stageID].sceneID;
                bool flag = false;
                if (FPSaveManager.storyFlag[FPSaveManager.GetStoryFlag(current.pointers.stageID)] <= 1 && dialogSequence.Length > 0)
                {
                    flag = true;
                }
                if (current.type == FPMapLocationType.HUB)
                {
                    flag = true;
                }
                if (dialogArray[current.pointers.stageID].disableAtStoryFlag > 0 && FPSaveManager.storyFlag[dialogArray[current.pointers.stageID].disableAtStoryFlag] >= dialogArray[current.pointers.stageID].storyFlagValue)
                {
                    flag = false;
                }
                if (flag)
                {
                    for (int i = 0; i < dialogSequence.Length; i++)
                    {
                        if (PlayerHandler.currentCharacter.useOwnCutsceneActivators)
                        {
                            if (dialogSequence[i].characters[PlayerHandler.currentCharacter.id])
                            {
                                Dialog(__instance, sceneID, dialogSequence[i].ID);
                                for (int j = i + 1; j < dialogSequence.Length; j++)
                                {
                                    if (dialogSequence[j].characters[PlayerHandler.currentCharacter.id])
                                    {
                                        Dialog(__instance, sceneID, dialogSequence[j].ID);
                                    }
                                }
                                break;
                            }
                        }
                        else
                        {
                            if (dialogSequence[i].characters[(int)PlayerHandler.currentCharacter.eventActivatorCharacter])
                            {
                                Dialog(__instance, sceneID, dialogSequence[i].ID);
                                for (int j = i + 1; j < dialogSequence.Length; j++)
                                {
                                    if (dialogSequence[j].characters[(int)PlayerHandler.currentCharacter.eventActivatorCharacter])
                                    {
                                        Dialog(__instance, sceneID, dialogSequence[j].ID);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MenuWorldMap), "CutsceneCheck", MethodType.Normal)]
        static IEnumerable<CodeInstruction> CutsceneCheckTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label patchStart = il.DefineLabel();
            Label loopEnd = il.DefineLabel();
            Label lilacTarget = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 1; i < codes.Count; i++)
            {
                //Original:
                /*
			    IL_021B: ldlen
			    IL_021C: conv.i4
			    IL_021D: blt       IL_011C
                IL_0222: ldloc.s 4

                The jump code of the loop. Located below all the loop's code but ran first.
                */

                if (codes[i - 1].opcode == OpCodes.Conv_I4 && codes[i].opcode == OpCodes.Blt && codes[i + 1].opcode == OpCodes.Ldloc_S)
                {
                    //Reroute start of the loop to our own code
                    lilacTarget = (Label)codes[i].operand;
                    loopEnd = (Label)codes[i - 13].operand;
                    codes[i].operand = patchStart;
                    break;
                }
            }

            //Our stuff
            /*
             * Ldarg_0 (load 'this' into argument stack)
             * Call (Fancy call constructor util is used here)
             * Bfalse lilacTarget (jump to lilacTarget if method returned false)
             * Ldarg_0 (load 'this' into argument stack)
             * Ldarg_0 (load 'this' into argument stack - again)
             * Ldloc_0 (load 'i')
             * Ldloc_S 5 (load 'l')
             * Call (the sequel)
             * StLoc_2 (load the result into val 2 - in our case 'flag')
             * Br loopEnd (break the loop)
             */
            //C# result:
            /*
                if (FPSaveManager.character > FPCharacterID.NEERA)
			    {
			        flag = CutsceneCheckExtended(this);
			        break;
			    }
            */


            CodeInstruction patchCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            patchCodeStart.labels.Add(patchStart);

            codes.Add(patchCodeStart);
            codes.Add(Transpilers.EmitDelegate(CutsceneCheckCharBase));
            codes.Add(new CodeInstruction(OpCodes.Brfalse, lilacTarget));
            codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
            codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
            codes.Add(new CodeInstruction(OpCodes.Ldloc_0));
            codes.Add(new CodeInstruction(OpCodes.Ldloc_S, 5));
            codes.Add(Transpilers.EmitDelegate(CutsceneCheckExtended));
            codes.Add(new CodeInstruction(OpCodes.Stloc_S, 4));
            codes.Add(new CodeInstruction(OpCodes.Br, loopEnd));

            return codes;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(MenuWorldMap), "Dialog", MethodType.Normal)]
        public static void Dialog(object instance, int scene, int line)
        {
            throw new NotImplementedException("Reverse Patch failed!");
        }
    }

}