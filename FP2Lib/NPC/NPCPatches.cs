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
            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                if (npc.ID >= ___npcNames.Length)
                {
                    ___npcNames = ___npcNames.AddRangeToArray(new string[(npc.ID + 1) - ___npcNames.Length]);
                }

                if (npc.ID != 0)
                {
                    ___npcNames[npc.ID] = npc.getNpcString();
                }

                if (npc.ID == 0 && !(___npcNames.Contains(npc.getNpcString())))
                {
                    ___npcNames = ___npcNames.AddToArray(npc.getNpcString());
                }
                npc.ID = FPSaveManager.GetNPCNumber(npc.Name);
            }

            if (FPSaveManager.npcFlag.Length < ___npcNames.Length)
                FPSaveManager.npcFlag = FPSaveManager.ExpandByteArray(FPSaveManager.npcFlag, ___npcNames.Length);
            if (FPSaveManager.npcDialogHistory.Length < ___npcNames.Length)
                FPSaveManager.npcDialogHistory = FPSaveManager.ExpandNPCDialogHistory(FPSaveManager.npcDialogHistory, ___npcNames.Length);

            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                if (npc.ID != 0 && FPSaveManager.npcDialogHistory[npc.ID].dialog == null)
                {
                    FPSaveManager.npcDialogHistory[npc.ID].dialog = new bool[npc.DialogueTopics];
                }
                else if (npc.ID != 0 && FPSaveManager.npcDialogHistory[npc.ID].dialog.Length < npc.DialogueTopics)
                {
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
                npcRenderer = ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController;
            }

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

            if (id != selectedNPC)
            {
                ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController = npcRenderer;
                selectedNPC = id;

                Animator component = __instance.npcPreviewSprite.gameObject.GetComponent<Animator>();
                if (component.GetCurrentAnimatorClipInfoCount(0) > 0)
                {
                    if (component.GetCurrentAnimatorClipInfo(0)[0].clip.name == "null")
                    {
                        FileLog.Log("Found NPC missing animation in animator. ID=" + id);
                        foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
                        {
                            if (npc.ID == id)
                            {
                                FileLog.Log("Found matchin NPC object with UID=" + npc.UID);
                                ___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController = npc.Prefabs.Values.First().GetComponent<Animator>().runtimeAnimatorController;
                                FileLog.Log(___npcPreviewSprite.GetComponent<Animator>().runtimeAnimatorController.name);
                                component.Play("Default", 0);
                            }
                        }
                    }
                }
            }
        }        
    }
}
