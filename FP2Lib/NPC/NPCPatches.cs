using BepInEx;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.NPC
{
    internal class NPCPatches
    {
        static RuntimeAnimatorController npcRenderer;
        private static int selectedNPC = 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), nameof(FPSaveManager.LoadFromFile), MethodType.Normal)]
        static void PatchFPSaveManager(ref string[] ___npcNames)
        {
            //Save file loading from filesystem, running logic to append our NPC listing
            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                //If NPC got ID outside of current array size, expand it accordingly and populate it with default data
                if (npc.ID >= ___npcNames.Length)
                {
                    for (int i = ___npcNames.Length; i <= (npc.ID + 1); i++)
                        ___npcNames = ___npcNames.AddToArray("00 00 Data Missing");
                }

                if (npc.ID != 0)
                {
                    //Non-zero ID means NPC was initialised before (or already exists in the array) and already has ID assigned. Set it in the array.
                    //Also check if it uses placeholder (otherwise we migh have collision which need fixing)
                    if (___npcNames[npc.ID] == "00 00 Data Missing")
                    {
                        ___npcNames[npc.ID] = npc.getNpcString();
                    } 
                    else
                    {
                        //Collision found, set ID to 0 so the next step assigns us new one.
                        npc.ID = 0;
                    }
                }

                if (npc.ID == 0 && !(___npcNames.Contains(npc.getNpcString())))
                {
                    //Zero ID + Not existing within array == new NPC, add it at the end - this will assign new ID at the end of the array. Any holes in ID's are ignored.
                    ___npcNames = ___npcNames.AddToArray(npc.getNpcString());
                }
                npc.ID = FPSaveManager.GetNPCNumber(npc.Name);
            }

            //Extend other arrays accordingly
            if (FPSaveManager.npcFlag.Length < ___npcNames.Length)
                FPSaveManager.npcFlag = FPSaveManager.ExpandByteArray(FPSaveManager.npcFlag, ___npcNames.Length);
            if (FPSaveManager.npcDialogHistory.Length < ___npcNames.Length)
                FPSaveManager.npcDialogHistory = FPSaveManager.ExpandNPCDialogHistory(FPSaveManager.npcDialogHistory, ___npcNames.Length);

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
        [HarmonyPatch(typeof(MenuGlobalPause),"UpdateNPCList", MethodType.Normal)]
        static void PatchUpdateNPCList(MenuGlobalPause __instance, ref SpriteRenderer ___npcPreviewSprite, int ___currentNPC, int ___npcListOffset)
        {
            if (npcRenderer == null)
            {
                //save a reference to the default renderer
                npcRenderer = ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController;
            }

            //currentNPC (0-9, screen position) to NPC id conversion
            int[] array = new int[FPSaveManager.npcFlag.Length];
            int npcListLength = 0;
            for (int i = 1; i < FPSaveManager.npcFlag.Length; i++)
            {
                if (FPSaveManager.npcFlag[i] > 0)
                {
                    array[npcListLength] = i;
                    npcListLength++;
                }
            }
            int id = array[___currentNPC + ___npcListOffset];

            //If we swapped to different NPC than last frame
            if (id != selectedNPC)
            {
                ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController = npcRenderer;
                selectedNPC = id;

                Animator component = __instance.npcPreviewSprite.gameObject.GetComponent<Animator>();
                //Check if animator is initialised
                if (component.GetCurrentAnimatorClipInfoCount(0) > 0)
                {
                    //Check if we selected 'null' animation, if so replace with Default pose of the NPC (from the first prefab in case of multiple)
                    if (component.GetCurrentAnimatorClipInfo(0)[0].clip.name == "null")
                    {
                        foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
                        {
                            if (npc.ID == id)
                            {
                                ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController = npc.Prefabs.Values.First().GetComponent<Animator>().runtimeAnimatorController;
                                component.Play("Default", 0);
                            }
                        }
                    }
                }
            }
        }        
    }
}
