
using HarmonyLib;
using System;

namespace FP2Lib.Player.PlayerPatches
{
    class PatchPlayerDialogZone
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerDialogZone), "StartDialog", MethodType.Normal)]
        static bool PatchPDZStart(PlayerDialogZone __instance, ref bool ___playerInRange, ref int ___currentLine)
        {
            if (!___playerInRange && FPSaveManager.character >= (FPCharacterID)5)
            {
                ___playerInRange = true;
                FPSaveManager.activatedDialogZones[__instance.zoneID] = true;
                if (__instance.menuCutscene != null)
                {
                    CutsceneDialog cutsceneDialog = UnityEngine.Object.Instantiate<CutsceneDialog>(__instance.menuCutscene);
                    cutsceneDialog.currentScene = 0;
                    cutsceneDialog.dialogSystem = __instance.dialogSystem;
                    cutsceneDialog.dialogSequence = __instance.dialogSequence;
                    ___currentLine = __instance.dialogSequence.Length;
                }
                else if (__instance.dialogBox != null)
                {
                    for (int i = 0; i < __instance.dialogSequence.Length; i++)
                    {
                        if (!PlayerHandler.currentCharacter.useOwnCutsceneActivators)
                        {
                            if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.LILAC && __instance.dialogSequence[i].characters[0])
                            {
                                Dialog(__instance, 0, __instance.dialogSequence[i].ID);
                                ___currentLine = i;
                                break;
                            }
                            if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.CAROL && __instance.dialogSequence[i].characters[1])
                            {
                                Dialog(__instance, 0, __instance.dialogSequence[i].ID);
                                ___currentLine = i;
                                break;
                            }
                            if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.BIKECAROL && __instance.dialogSequence[i].characters[1])
                            {
                                Dialog(__instance, 0, __instance.dialogSequence[i].ID);
                                ___currentLine = i;
                                break;
                            }
                            if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.MILLA && __instance.dialogSequence[i].characters[2])
                            {
                                Dialog(__instance, 0, __instance.dialogSequence[i].ID);
                                ___currentLine = i;
                                break;
                            }
                            if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.NEERA && __instance.dialogSequence[i].characters[3])
                            {
                                Dialog(__instance, 0, __instance.dialogSequence[i].ID);
                                ___currentLine = i;
                                break;
                            }
                        }
                        else
                        {
                            if (__instance.dialogSequence[i].characters[PlayerHandler.currentCharacter.id])
                            {
                                Dialog(__instance, 0, __instance.dialogSequence[i].ID);
                                ___currentLine = i;
                                break;
                            }
                        }
                    }
                }
                return false;
            }
            else return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerDialogZone), "LateUpdate", MethodType.Normal)]
        static void PatchPDZUpdate(PlayerDialogZone __instance, ref bool ___playerInRange, ref int ___currentLine)
        {
            if (FPSaveManager.gameMode != FPGameMode.CLASSIC || __instance.playInClassicMode)
            {
                if (PlayerHandler.currentCharacter != null && ___playerInRange && ___currentLine <= __instance.dialogSequence.Length - 1 && __instance.dialogSequence.Length != 1)
                {
                    if (!PlayerHandler.currentCharacter.useOwnCutsceneActivators)
                    {
                        if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.LILAC && __instance.dialogSequence[___currentLine].characters[0])
                        {
                            Dialog(__instance, 0, __instance.dialogSequence[___currentLine].ID);
                        }
                        else if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.CAROL && __instance.dialogSequence[___currentLine].characters[1])
                        {
                            Dialog(__instance, 0, __instance.dialogSequence[___currentLine].ID);
                        }
                        else if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.BIKECAROL && __instance.dialogSequence[___currentLine].characters[1])
                        {
                            Dialog(__instance, 0, __instance.dialogSequence[___currentLine].ID);
                        }
                        else if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.MILLA && __instance.dialogSequence[___currentLine].characters[2])
                        {
                            Dialog(__instance, 0, __instance.dialogSequence[___currentLine].ID);
                        }
                        else if (PlayerHandler.currentCharacter.eventActivatorCharacter == FPCharacterID.NEERA && __instance.dialogSequence[___currentLine].characters[3])
                        {
                            Dialog(__instance, 0, __instance.dialogSequence[___currentLine].ID);
                        }
                    }
                    else
                    {
                        if (__instance.dialogSequence[___currentLine].characters[PlayerHandler.currentCharacter.id])
                        {
                            Dialog(__instance, 0, __instance.dialogSequence[___currentLine].ID);
                        }
                    }
                    if (___currentLine == __instance.dialogSequence.Length - 1) ___currentLine++;
                }
            }
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PlayerDialogZone), "Dialog", MethodType.Normal)]
        public static void Dialog(PlayerDialogZone instance, int scene, int line)
        {
            // Replaced at runtime with reverse patch
            throw new NotImplementedException("Method failed to reverse patch!");
        }
    }
}
