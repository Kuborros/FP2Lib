using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace FP2Lib.NPC
{
    internal class NPCPatches
    {
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
            FP2Lib.npcHandler.writeToStorage();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Start", MethodType.Normal)]
        static void PatchFPPlayer()
        {
            string stageName = FPStage.stageNameString;

            foreach (HubNPC npc in NPCHandler.HubNPCs.Values)
            {
                if (npc.HomeScene == stageName)
                {
                    npc.RuntimeObject = GameObject.Instantiate(npc.Prefab);
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
    }
}
