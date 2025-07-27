using BepInEx;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace FP2Lib.NPC
{
    internal class NPCPatches
    {
        static RuntimeAnimatorController npcRenderer;
        private static int selectedNPC = 0;

        private static string customSpeciesDisplay = "";
        private static string customHomeDisplay = "";

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.LoadFromFile), MethodType.Normal)]
        static void PatchFPSaveManager(ref string[] ___npcNames)
        {
            //Save file loading from filesystem, running logic to append our NPC listing
            //Loop for NPC with existing ID's
            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                //If NPC got ID outside of current array size, expand it accordingly and populate it with default data
                if (npc.ID >= ___npcNames.Length)
                {
                    for (int i = ___npcNames.Length; i <= (npc.ID); i++)
                        ___npcNames = ___npcNames.AddToArray("00 00 Data Missing");
                }

                if (npc.ID != 0)
                {
                    //Non-zero ID means NPC was initialised before (or already exists in the array) and already has ID assigned. Set it in the array.
                    //Also check if it uses placeholder (otherwise we migh have collision which need fixing)
                    if (___npcNames[npc.ID] == "00 00 Data Missing" || ___npcNames[npc.ID] == npc.getNpcString())
                    {
                        ___npcNames[npc.ID] = npc.getNpcString();
                        FP2Lib.logSource.LogDebug("NPC " + npc.UID + " restored from storage with ID = " + npc.ID);
                    }
                    else
                    {
                        //Collision found, set ID to 0 so the next step assigns us new one.
                        FP2Lib.logSource.LogDebug("NPC " + npc.UID + " conflicts with ID = " + npc.ID);
                        npc.ID = 0;
                    }
                }
            }

            //Handling of NPC with no ID assigned
            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                if (npc.ID == 0 && !(___npcNames.Contains(npc.getNpcString())))
                {
                    FP2Lib.logSource.LogDebug("Found NPC with no ID: " + npc.UID);
                    //Zero ID + Not existing within array == new NPC, add it.
                    //First try to repurpose blank ID's
                    for (int i = 1; i < ___npcNames.Length; i++)
                    {
                        if (___npcNames[i] == "00 00 Data Missing")
                        {
                            ___npcNames[i] = npc.getNpcString();
                            FP2Lib.logSource.LogDebug("Assigned empty ID = " + i);
                            //Cursed but does what needed.
                            goto END;
                        }
                    }
                    //If nothing was found, append at the end
                    FP2Lib.logSource.LogDebug("No empty ID found, adding at end of array.");
                    ___npcNames = ___npcNames.AddToArray(npc.getNpcString());
                }
            END:
                npc.ID = FPSaveManager.GetNPCNumber(npc.Name);
                FP2Lib.logSource.LogDebug("ID for NPC " + npc.Name + " = " + npc.ID);
            }



            //Resize other arrays accordingly

            //Resize up
            if (FPSaveManager.npcFlag.Length < ___npcNames.Length)
                FPSaveManager.npcFlag = FPSaveManager.ExpandByteArray(FPSaveManager.npcFlag, ___npcNames.Length);
            if (FPSaveManager.npcDialogHistory.Length < ___npcNames.Length)
                FPSaveManager.npcDialogHistory = FPSaveManager.ExpandNPCDialogHistory(FPSaveManager.npcDialogHistory, ___npcNames.Length);
            //Trim if too long (occurs when NPC gets fully wiped but has 'seen' flag, causes issues in Citiziens tab)
            if (FPSaveManager.npcFlag.Length > ___npcNames.Length)
                FPSaveManager.npcFlag = FPSaveManager.npcFlag.Take(___npcNames.Length).ToArray();
            if (FPSaveManager.npcDialogHistory.Length > ___npcNames.Length)
                FPSaveManager.npcDialogHistory = FPSaveManager.npcDialogHistory.Take(___npcNames.Length).ToArray();

            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                if (npc.ID != 0 && FPSaveManager.npcDialogHistory[npc.ID].dialog == null)
                {
                    //Initialise dialog history
                    FPSaveManager.npcDialogHistory[npc.ID].dialog = new bool[npc.DialogueTopics];
                }
                else if (npc.ID != 0 && FPSaveManager.npcDialogHistory[npc.ID].dialog.Length < npc.DialogueTopics)
                {
                    //Current dialog array is too small, id shift? Resetting it to sane default.
                    FPSaveManager.npcDialogHistory[npc.ID].dialog = new bool[npc.DialogueTopics];
                }
            }
            NPCHandler.WriteToStorage();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Start", MethodType.Normal)]
        static void PatchFPPlayer()
        {
            string stageName = FPStage.stageNameString;

            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                if (npc.Prefabs.ContainsKey(stageName))
                {
                    //NPC is to be loaded in this stage, instance it and save reference to instanced object.
                    npc.RuntimeObject = GameObject.Instantiate(npc.Prefabs[stageName]);
                }
                else npc.RuntimeObject = null;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPStage), "UpdateObjectActivation", MethodType.Normal)]
        static void PatchFPStage()
        {
            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                if (npc.RuntimeObject != null)
                {
                    //If we have runtime object, it means we are loading the NPC on this level. Add it to StageList to properly initialise it within the stage.
                    FPStage.ValidateStageListPos(npc.RuntimeObject.GetComponent<FPHubNPC>());
                }
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuGlobalPause), "UpdateNPCList", MethodType.Normal)]
        static void PatchUpdateNPCList(MenuGlobalPause __instance, ref SpriteRenderer ___npcPreviewSprite, int ___currentNPC, int ___npcListOffset, ref int ___npcListLength, ref MenuText ___npcSpecies, ref MenuText ___npcHome)
        {
            if (npcRenderer == null)
            {
                //save a reference to the default renderer
                npcRenderer = ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController;
            }

            //currentNPC (0-9, screen position) to NPC id conversion
            int[] array = new int[FPSaveManager.npcFlag.Length];
            ___npcListLength = 0;
            for (int i = 1; i < FPSaveManager.npcFlag.Length; i++)
            {
                if (FPSaveManager.npcFlag[i] > 0)
                {
                    array[___npcListLength] = i;
                    ___npcListLength++;
                }
            }

            int id;
            //1.2.4 and newer sort the list, patch accordingly.
            if (FP2Lib.gameInfo.gameVersion.CompareTo(new Version("1.2.3")) > 0)
            {

                int[] array2 = new int[FPSaveManager.npcFlag.Length];
                array.CopyTo(array2, 0);
                for (int j = 0; j < ___npcListLength; j++)
                {
                    bool flag = false;
                    int num = j;
                    while (!flag && j > 0)
                    {
                        int num2 = array2[num];
                        int num3 = array2[num - 1];
                        string text = FPSaveManager.GetNPCName(num2);
                        string text2 = FPSaveManager.GetNPCName(num3);
                        if (text == "Dr Tuvluv")
                        {
                            text = "Tuvluv";
                        }
                        if (text2 == "Dr Tuvluv")
                        {
                            text2 = "Tuvluv";
                        }
                        int num4 = text.CompareTo(text2);
                        if (num4 < 0)
                        {
                            array2[num - 1] = num2;
                            array2[num] = num3;
                            num--;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
                id = array2[___currentNPC + ___npcListOffset];
            }
            else
            {
                id = array[___currentNPC + ___npcListOffset];
            }

            //If we swapped to different NPC than last frame
            if (id != selectedNPC)
            {
                ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController = npcRenderer;
                selectedNPC = id;
                customHomeDisplay = string.Empty;
                customSpeciesDisplay = string.Empty;

                Animator component = __instance.npcPreviewSprite.gameObject.GetComponent<Animator>();

                foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
                {
                    if (npc.ID == id)
                    {
                        //Set custom preview
                        ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController = npc.Prefabs.Values.First().GetComponent<Animator>().runtimeAnimatorController;
                        component.Play("Default", 0);
                        //Set custom home and species if needed.
                        if (!npc.customSpecies.IsNullOrWhiteSpace())
                        {
                            customSpeciesDisplay = npc.customSpecies;
                        }
                        if (!npc.customHome.IsNullOrWhiteSpace())
                        {
                            customHomeDisplay = npc.customHome;
                        }
                        return;
                    }
                }
            }
            if (!customSpeciesDisplay.IsNullOrWhiteSpace())
            {
                ___npcSpecies.GetComponent<TextMesh>().text = customSpeciesDisplay;
            }
            if (!customHomeDisplay.IsNullOrWhiteSpace())
            {
                ___npcHome.GetComponent<TextMesh>().text = customHomeDisplay;
            }
        }
    }
}
