using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FP2Lib.Challenge
{
    internal class ChallengePatches
    {
        private static readonly ManualLogSource ArenaLogSource = FP2Lib.logSource;
        private static int[] idLookup;
        private static Dictionary<int,Sprite> spriteLookup = [];
        private static int totalChallenges = 0, totalHomeRuns = -1, totalBosses = 0, totalDojoBosses = 0, totalDojoChallenges = -1;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad(ref byte[] ___challengeRank, ref int[] ___challengeRecord)
        {
            //Calculate how many challenges we have
            int totalArenaChallenges = ChallengeHandler.baseChallenges + ChallengeHandler.Challenges.Count + 4;

            //Add slots in file for extra challenges.
            ___challengeRank = FPSaveManager.ExpandByteArray(___challengeRank, totalArenaChallenges);
            ___challengeRecord = FPSaveManager.ExpandIntArray(___challengeRecord, totalArenaChallenges);

            foreach (ChallengeData challenge in ChallengeHandler.Challenges.Values)
            {
                //Highest ID is the track number
                if (challenge.id > totalArenaChallenges)
                {
                    totalArenaChallenges = challenge.id;
                    ArenaLogSource.LogDebug("Detected gap in challenge ids!!");
                }
                //Skip uninitialised challenges.
                if (challenge.destinationScene.IsNullOrWhiteSpace()) continue;

                //Count each type up
                switch (challenge.type)
                {
                    case FPChallengeType.BOSS:
                        challenge.localID = totalBosses;
                        totalBosses++;
                        break;
                    case FPChallengeType.CHALLENGE:
                    case FPChallengeType.RACE:
                        challenge.localID = totalChallenges;
                        totalChallenges++;
                        break;
                    case FPChallengeType.HOMERUN:
                        challenge.localID = totalHomeRuns;
                        totalHomeRuns++;
                        break;
                    case FPChallengeType.DOJO_BOSS:
                        challenge.localID = totalDojoBosses;
                        totalDojoBosses++;
                        break;
                    case FPChallengeType.DOJO_CHALLENGE:
                        challenge.localID = totalDojoChallenges;
                        totalDojoChallenges++;
                        break;
                }
            }
            ChallengeHandler.WriteToStorage();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuArena),"Start",MethodType.Normal)]
        static void PatchMenuArena(ref int ___completedArenas, ref int ___unlockedArenas, ref int ___completedBosses, ref int ___unlockedBosses, ref int ___completedHomeRun, ref int ___unlockedHomeRun)
        {
            foreach (ChallengeData challenge in ChallengeHandler.Challenges.Values)
            {
                //Skip uninitialised challenges.
                if (challenge.destinationScene.IsNullOrWhiteSpace()) continue;

                //Update the completed/unlocked challenge counts.
                switch (challenge.type)
                {
                    case FPChallengeType.BOSS:
                        if (challenge.unlockRequirement == -1) ___unlockedBosses++;
                        else if (FPSaveManager.timeRecord[challenge.unlockRequirement] > 0) ___unlockedBosses++;
                        if (FPSaveManager.challengeRecord[challenge.id] > 0) ___completedBosses++;
                        break;
                    case FPChallengeType.CHALLENGE:
                    case FPChallengeType.RACE:
                        if (challenge.unlockRequirement == -1) ___unlockedArenas++;
                        else if (FPSaveManager.timeRecord[challenge.unlockRequirement] > 0) ___unlockedArenas++;
                        if (FPSaveManager.challengeRecord[challenge.id] > 0) ___completedArenas++;
                        break;
                    case FPChallengeType.HOMERUN:
                        if (challenge.unlockRequirement == -1) ___unlockedHomeRun++;
                        else if (FPSaveManager.timeRecord[challenge.unlockRequirement] > 0) ___unlockedHomeRun++;
                        if (FPSaveManager.challengeRecord[challenge.id] > 0) ___completedHomeRun++;
                        break;
                }
            }
        }

        //Boss Stuff
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuArenaBossSelect), "Start", MethodType.Normal)]
        static void PatchArenaBossSelectPre(MenuArenaBossSelect __instance, ref FPHudDigit ___pfBossLabel, ref MenuText ___bossNames)
        {
            int bossSceneOffset = __instance.bossScenes.Length;
            int bossUnlockOffset = __instance.bossUnlockRequirement.Length;
            int bossSpawnIDOffset = __instance.bossSpawnID.Length;

            //Extend arrays
            ___pfBossLabel.digitFrames = Utils.ExpandSpriteArray(___pfBossLabel.digitFrames, ChallengeHandler.baseChallenges + ChallengeHandler.Challenges.Count, null);
            ___bossNames.paragraph = Utils.ExpandStringArray(___bossNames.paragraph, ChallengeHandler.baseChallenges + ChallengeHandler.Challenges.Count);

            foreach (ChallengeData challenge in ChallengeHandler.Challenges.Values)
            {
                //Skip uninitialised ones.
                if (challenge.destinationScene.IsNullOrWhiteSpace()) continue;

                switch (challenge.type)
                {
                    case FPChallengeType.BOSS:
                        if (SceneManager.GetActiveScene().name == "ArenaMenu")
                        {
                            __instance.bossScenes = Utils.ExpandStringArray(__instance.bossScenes, bossSceneOffset + totalBosses);
                            __instance.bossScenes[bossSceneOffset + challenge.localID] = challenge.destinationScene;

                            __instance.bossUnlockRequirement = FPSaveManager.ExpandIntArray(__instance.bossUnlockRequirement, bossUnlockOffset + totalBosses);
                            __instance.bossUnlockRequirement[bossUnlockOffset + challenge.localID] = challenge.unlockRequirement;

                            __instance.bossSpawnID = FPSaveManager.ExpandIntArray(__instance.bossSpawnID, bossSpawnIDOffset + totalBosses);
                            __instance.bossSpawnID[bossSpawnIDOffset + challenge.localID] = challenge.id;

                            ___pfBossLabel.digitFrames[bossSceneOffset + challenge.localID + 1] = challenge.bossIcon;
                            ___bossNames.paragraph[bossSceneOffset + challenge.localID + 1] = challenge.name;
                        }
                        break;
                    case FPChallengeType.DOJO_BOSS:
                        if (FPStage.stageNameString == "Royal Palace")
                        {
                            __instance.bossScenes = Utils.ExpandStringArray(__instance.bossScenes, bossSceneOffset + totalDojoBosses);
                            __instance.bossScenes[bossSceneOffset + challenge.localID] = challenge.destinationScene;

                            __instance.bossUnlockRequirement = FPSaveManager.ExpandIntArray(__instance.bossUnlockRequirement, bossUnlockOffset + totalDojoBosses);
                            __instance.bossUnlockRequirement[bossUnlockOffset + challenge.localID] = challenge.unlockRequirement;

                            __instance.bossSpawnID = FPSaveManager.ExpandIntArray(__instance.bossSpawnID, bossSpawnIDOffset + totalDojoBosses);
                            __instance.bossSpawnID[bossSpawnIDOffset + challenge.localID] = challenge.id;

                            //Mirror Match
                            if (challenge.bossCharacterID == FPSaveManager.character)
                            {
                                ___pfBossLabel.digitFrames[bossSceneOffset + challenge.localID + 1] = ___pfBossLabel.digitFrames[0];
                                ___bossNames.paragraph[bossSceneOffset + challenge.localID + 1] = "Pangu Hologram";
                            }
                            else
                            {
                                ___pfBossLabel.digitFrames[bossSceneOffset + challenge.localID + 1] = challenge.bossIcon;
                                ___bossNames.paragraph[bossSceneOffset + challenge.localID + 1] = challenge.name;
                            }

                            //Allow scrolling when more than 1 boss is added
                            if (totalDojoBosses > 1) __instance.disableScrolling = false;
                        }
                        break;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuArenaBossSelect), "Start", MethodType.Normal)]
        static void PatchArenaBossSelect(ref FPHudDigit[] ___bossList, ref int[] ___internalSpawnID)
        {
            if (totalDojoBosses > 0 || totalBosses > 0)
            {
                for (int i = 0; i < ___internalSpawnID.Length; i++)
                {
                    //Run only for moddes stuff
                    if (___internalSpawnID[i] > 85)
                    {
                        ChallengeData data = ChallengeHandler.GetChallengeDataByRuntimeID(i);
                        if (data != null)
                        {
                            //Foreshadowing
                            if (ShouldBossBeForeshadowed(data))
                            {
                                ___bossList[i].GetRenderer().color = new Color(0f, 0f, 0f);
                                ___bossList[i].GetRenderer().material = FPResources.material[6];
                                SpriteOutline spriteOutline = ___bossList[i].gameObject.AddComponent<SpriteOutline>();
                                spriteOutline.color = new Color(0f, 0.54509807f, 1f);
                                spriteOutline.outlineSize = 0;
                            }
                        }
                    }
                }
            }
        }

        private static bool ShouldBossBeForeshadowed(ChallengeData challenge)
        {
            if (challenge.foreshadow)
            {
                if (challenge.CustomBossUnlockCheck != null)
                {
                    //Foreshadow only when not unblocked - this really only makes sense with custom unblock conditions, as this is only scenario where a both is visible but not selectable.
                    return !challenge.CustomBossUnlockCheck();
                }
            }
            //Otherwise disable foreshadowing
            return false;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetBossHome", MethodType.Normal)]
        static void PatchGetBossHome(int challenge, ref string __result)
        {
            if (challenge >= 90)
            {
                ChallengeData data = ChallengeHandler.GetChallengeDataByRuntimeID(challenge);
                if (data != null)
                {
                    __result = data.bossHome;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuArenaBossSelect), "CheckAdditionalBossRequirements", MethodType.Normal)]
        static void PatchCheckAdditionalBossRequirements(int bossSlotID, ref bool __result)
        {
            if (bossSlotID >= 90)
            {
                ChallengeData challenge = ChallengeHandler.GetChallengeDataByRuntimeID(bossSlotID);
                if (challenge != null)
                {
                    if (challenge.CustomBossUnlockCheck != null)
                    {
                        __result = challenge.CustomBossUnlockCheck();
                    }
                }
            }
        }

        //Challenge Stuff

        [HarmonyPrefix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "Start", MethodType.Normal)]
        static void PatchArenaChallengeSelectPre(MenuArenaChallengeSelect __instance, ref MenuText ___challengeNames, ref MenuText ___pfChallengeLabel, ref MenuText ___textDescription, ref GameObject[] ___graphicPanels)
        {
            int challengeSceneOffset = __instance.challengeScene.Length + 1;
            int challengeUnlockOffset = __instance.challengeUnlockRequirement.Length + 1;
            int challengeSpawnIDOffset = __instance.challengeSpawnID.Length + 1;
            int challengeRewardIDOffset = __instance.challengeRewards.Length + 1;

            idLookup = new int[__instance.challengeScene.Length];
            spriteLookup = [];

            //Fuckery to force-ignore slot 21
            if (__instance.name.Contains("ArenaChallengeSelect"))
            {
                __instance.challengeScene = __instance.challengeScene.AddToArray("Battlesphere_Arena");
                __instance.challengeUnlockRequirement = __instance.challengeUnlockRequirement.AddToArray(30);
                __instance.challengeSpawnID = __instance.challengeSpawnID.AddToArray(-1);
                __instance.challengeRewards = __instance.challengeRewards.AddToArray(1000);
            }

            //Extend arrays
            if (__instance.name.Contains("ArenaChallengeSelect"))
            {
                ___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames =
                    Utils.ExpandSpriteArray(___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames, challengeSceneOffset + totalChallenges + 1,
                    ___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames[0]);
            }
            if (__instance.name.Contains("ArenaHomeRunSelect"))
            {
                ___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames =
                    Utils.ExpandSpriteArray(___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames, challengeSceneOffset + totalHomeRuns,
                    ___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames[0]);
            }
            if (__instance.name.Contains("TrainingChallengeSelect"))
            {
                ___graphicPanels = ___graphicPanels.AddRangeToArray<GameObject>(new GameObject[totalDojoChallenges + 1]);
            }

            foreach (ChallengeData challenge in ChallengeHandler.Challenges.Values)
            {
                //Skip uninitialised ones.
                if (challenge.destinationScene.IsNullOrWhiteSpace()) continue;

                switch (challenge.type)
                {
                    case FPChallengeType.CHALLENGE:
                    case FPChallengeType.RACE:

                        if (__instance.name.Contains("ArenaChallengeSelect"))
                        {
                            __instance.challengeScene = Utils.ExpandStringArray(__instance.challengeScene, challengeSceneOffset + totalChallenges);
                            __instance.challengeScene[challengeSceneOffset + challenge.localID] = challenge.destinationScene;

                            __instance.challengeUnlockRequirement = FPSaveManager.ExpandIntArray(__instance.challengeUnlockRequirement, challengeUnlockOffset + totalChallenges);
                            __instance.challengeUnlockRequirement[challengeUnlockOffset + challenge.localID] = challenge.unlockRequirement;

                            __instance.challengeSpawnID = FPSaveManager.ExpandIntArray(__instance.challengeSpawnID, challengeSpawnIDOffset + totalChallenges);
                            __instance.challengeSpawnID[challengeSpawnIDOffset + challenge.localID] = challenge.id;

                            __instance.challengeRewards = FPSaveManager.ExpandIntArray(__instance.challengeRewards, challengeRewardIDOffset + totalChallenges);
                            __instance.challengeRewards[challengeRewardIDOffset + challenge.localID] = challenge.crystalReward;

                            __instance.timeCapsuleID = FPSaveManager.ExpandIntArray(__instance.timeCapsuleID, challengeRewardIDOffset + totalChallenges);
                            __instance.timeCapsuleID[challengeRewardIDOffset + challenge.localID] = challenge.timeCapsuleID;

                            ___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames[challengeSceneOffset + challenge.localID] = challenge.challengeIcon;

                            ___challengeNames.paragraph = Utils.ExpandStringArray(___challengeNames.paragraph, challengeSceneOffset + totalChallenges + 1);
                            ___textDescription.paragraph = Utils.ExpandStringArray(___textDescription.paragraph, challengeSceneOffset + totalChallenges + 1);

                            ___challengeNames.paragraph[challengeSceneOffset + challenge.localID] = challenge.name;
                            ___textDescription.paragraph[challengeSceneOffset + challenge.localID] = challenge.challengeDescription;

                            idLookup = FPSaveManager.ExpandIntArray(idLookup, challengeSceneOffset + totalChallenges + 1);

                        }
                        break;
                    case FPChallengeType.DOJO_CHALLENGE:
                        //It used hand-made GameObjects for the preview graphics.
                        if (__instance.name.Contains("TrainingChallengeSelect"))
                        {
                            __instance.challengeScene = Utils.ExpandStringArray(__instance.challengeScene, challengeSceneOffset + totalDojoChallenges);
                            __instance.challengeScene[challengeSceneOffset + challenge.localID] = challenge.destinationScene;

                            __instance.challengeUnlockRequirement = FPSaveManager.ExpandIntArray(__instance.challengeUnlockRequirement, challengeUnlockOffset + totalDojoChallenges);
                            __instance.challengeUnlockRequirement[challengeUnlockOffset + challenge.localID] = challenge.unlockRequirement;

                            __instance.challengeSpawnID = FPSaveManager.ExpandIntArray(__instance.challengeSpawnID, challengeSpawnIDOffset + totalDojoChallenges);
                            __instance.challengeSpawnID[challengeSpawnIDOffset + challenge.localID] = challenge.id;

                            __instance.challengeRewards = FPSaveManager.ExpandIntArray(__instance.challengeRewards, challengeRewardIDOffset + totalDojoChallenges);
                            __instance.challengeRewards[challengeRewardIDOffset + challenge.localID] = challenge.crystalReward;

                            __instance.timeCapsuleID = FPSaveManager.ExpandIntArray(__instance.timeCapsuleID, challengeRewardIDOffset + totalChallenges);
                            __instance.timeCapsuleID[challengeRewardIDOffset + challenge.localID] = challenge.timeCapsuleID;

                            //Dojo lacks custom labels, its all one sprite.
                            ___challengeNames.paragraph = Utils.ExpandStringArray(___challengeNames.paragraph, challengeSceneOffset + totalDojoChallenges + 1);
                            ___challengeNames.paragraph[challengeSceneOffset + challenge.localID] = challenge.name;

                            ___textDescription.paragraph = Utils.ExpandStringArray(___textDescription.paragraph, challengeSceneOffset + totalDojoChallenges + 1);
                            ___textDescription.paragraph[challengeSceneOffset + challenge.localID] = challenge.challengeDescription;

                            if (challenge.dojoChallengePreview != null)
                                ___graphicPanels[challengeSceneOffset + challenge.localID] = challenge.dojoChallengePreview;
                            else
                                ___graphicPanels[challengeSceneOffset + challenge.localID] = ___graphicPanels[0];

                            idLookup = FPSaveManager.ExpandIntArray(idLookup, challengeSceneOffset + challenge.localID + 1);

                        }
                        break;
                    case FPChallengeType.HOMERUN:

                        //Home Run seems to have once had a different menu object, which got at the later time smashed into the ArenaChallengeSelect
                        //It shares only some of the fields with Arena UI, despite being the same object. It also renders it's labels in another way.
                        //This one uses 'challengeNames' object to pull names from
                        //Both objects draw to both TextMesh _and_ SuperTextMesh variants of the labels.

                        if (__instance.name.Contains("ArenaHomeRunSelect"))
                        {
                            __instance.challengeScene = Utils.ExpandStringArray(__instance.challengeScene, challengeSceneOffset + totalHomeRuns);
                            __instance.challengeScene[challengeSceneOffset + challenge.localID] = challenge.destinationScene;

                            __instance.challengeUnlockRequirement = FPSaveManager.ExpandIntArray(__instance.challengeUnlockRequirement, challengeUnlockOffset + totalHomeRuns);
                            __instance.challengeUnlockRequirement[challengeUnlockOffset + challenge.localID] = challenge.unlockRequirement;

                            __instance.challengeSpawnID = FPSaveManager.ExpandIntArray(__instance.challengeSpawnID, challengeSpawnIDOffset + totalHomeRuns);
                            __instance.challengeSpawnID[challengeSpawnIDOffset + challenge.localID] = challenge.id;

                            __instance.challengeRewards = FPSaveManager.ExpandIntArray(__instance.challengeRewards, challengeRewardIDOffset + totalHomeRuns);
                            __instance.challengeRewards[challengeRewardIDOffset + challenge.localID] = challenge.crystalReward;

                            __instance.timeCapsuleID = FPSaveManager.ExpandIntArray(__instance.timeCapsuleID, challengeRewardIDOffset + totalHomeRuns);
                            __instance.timeCapsuleID[challengeRewardIDOffset + challenge.localID] = challenge.timeCapsuleID;

                            ___pfChallengeLabel.transform.GetChild(0).GetComponent<FPHudDigit>().digitFrames[challengeSceneOffset + challenge.localID] = challenge.challengeIcon;

                            ___challengeNames.paragraph = Utils.ExpandStringArray(___challengeNames.paragraph, challengeSceneOffset + totalHomeRuns);
                            ___challengeNames.paragraph[challengeSceneOffset + challenge.localID] = challenge.name;

                            ___textDescription.paragraph = Utils.ExpandStringArray(___textDescription.paragraph, challengeSceneOffset + totalHomeRuns);

                            idLookup = FPSaveManager.ExpandIntArray(idLookup, challengeSceneOffset + challenge.localID + 1);

                            if (totalHomeRuns >= 0) __instance.scrolling = true;
                        }
                        break;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "Start", MethodType.Normal)]
        static void PatchArenaChallengeSelect(MenuArenaChallengeSelect __instance, ref MenuText[] ___challengeList, ref int[] ___challengeSpawnID, ref bool[] ___challengeUnlocked, ref int[] ___slotID)
        {
            if (totalChallenges > 0 || totalHomeRuns >= 0)
            {
                for (int i = 0; i < ___challengeSpawnID.Length; i++)
                {
                    //Run only for modded stuff
                    if (___challengeSpawnID[i] >= 85 && ___challengeUnlocked[i])
                    {
                        ChallengeData data = ChallengeHandler.GetChallengeDataByRuntimeID(___challengeSpawnID[i]);
                        if (data != null)
                        {
                            //Overwrite default behaviour of turning everything over ID 19 into Dragon Circuit
                            if (___challengeList[i].GetComponent<SuperTextMesh>() != null)
                                ___challengeList[i].GetComponent<SuperTextMesh>().text = data.name;
                            if (___challengeList[i].GetComponent<TextMesh>() != null)
                                ___challengeList[i].GetComponent<TextMesh>().text = data.name;

                            if (___challengeList[i].transform.childCount > 1)
                            {
                                if (FPSaveManager.challengeRecord[___challengeSpawnID[i]] > 0) ___challengeList[i].transform.GetChild(1).gameObject.SetActive(true);
                            }

                            if (__instance.name.Contains("ArenaChallengeSelect"))
                            {
                                data.slotID = i - 2;
                            }
                            else data.slotID = i;

                            ___slotID[data.slotID] = i;
                            idLookup[data.slotID] = ___challengeSpawnID[i];
                            if (data.rewardSprite != null) spriteLookup.Add(data.id, data.rewardSprite);

                            //Scrolling is broken.
                            //Scroll if there is more than one challenge.
                            if (totalDojoChallenges > 0 && __instance.name.Contains("TrainingChallengeSelect"))
                            {
                                ArenaLogSource.LogInfo("Enabling horrible hack. Yes the display is missing *ON PURPOSE*. Don't report it lol.");
                                if (__instance.transform.childCount > 1 && __instance.transform.GetChild(1).childCount > 7)
                                {
                                    __instance.transform.GetChild(1).GetChild(7).gameObject.SetActive(false);
                                }
                                __instance.scrolling = true;
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(MenuArenaChallengeSelect), "State_Challenge", MethodType.Normal)]
        static void PatchArenaChallengeSelect(MenuArenaChallengeSelect __instance, int ___challengeSelection)
        {
            if (totalChallenges > 0 || totalHomeRuns >= 0)
            {
                if (___challengeSelection >= 20 && __instance.name.Contains("ArenaChallengeSelect"))
                {
                    if (__instance.textReward.textMesh != null)
                    {
                        if (FPSaveManager.challengeRecord[idLookup[___challengeSelection]] == 0)
                            __instance.textReward.textMesh.text = FPSaveManager.GetChallengeReward(idLookup[___challengeSelection]).ToString();
                        else
                            __instance.textReward.textMesh.text = (FPSaveManager.GetChallengeReward(idLookup[___challengeSelection]) / 4).ToString();
                    }
                    if (__instance.textRecord.textMesh != null)
                    {
                        __instance.textRecord.textMesh.text = FPStage.TimeToString(FPSaveManager.challengeRecord[idLookup[___challengeSelection]]);
                        if (__instance.rankIcon != null)
                        {
                            __instance.rankIcon.SetDigitValue(FPSaveManager.challengeRank[idLookup[___challengeSelection]]);
                        }
                    }
                    if (__instance.rewardItem != null && __instance.rewardCheckmark != null)
                    {
                        if (spriteLookup.ContainsKey(idLookup[___challengeSelection]))
                            __instance.rewardItem.sprite = spriteLookup[idLookup[___challengeSelection]];
                        if (FPSaveManager.challengeRecord[idLookup[___challengeSelection]] > 0)
                            __instance.rewardCheckmark.SetActive(true);
                    }
                }
                else if (___challengeSelection >= 3 && __instance.name.Contains("ArenaHomeRunSelect")) 
                {
                    //Home Run Specific
                    if (__instance.textRecordLocal != null)
                    {
                        __instance.textRecord.textMesh.text = FPSaveManager.challengeRecord[idLookup[___challengeSelection]].ToString(); //"Total"
                        __instance.textRecordLocal.textMesh.text = FPSaveManager.challengeRecord[idLookup[___challengeSelection] + 3].ToString(); //"Best Round"
                    }
                }
                else if (___challengeSelection >= 3 && __instance.name.Contains("TrainingChallengeSelect"))
                {
                    if (__instance.textReward.textMesh != null)
                    {
                        if (FPSaveManager.challengeRecord[idLookup[___challengeSelection]] == 0)
                            __instance.textReward.textMesh.text = FPSaveManager.GetChallengeReward(idLookup[___challengeSelection]).ToString();
                        else
                            __instance.textReward.textMesh.text = (FPSaveManager.GetChallengeReward(idLookup[___challengeSelection]) / 4).ToString();
                    }
                    if (__instance.textRecord.textMesh != null)
                    {
                        __instance.textRecord.textMesh.text = FPStage.TimeToString(FPSaveManager.challengeRecord[idLookup[___challengeSelection]]);
                        if (__instance.rankIcon != null)
                        {
                            __instance.rankIcon.SetDigitValue(FPSaveManager.challengeRank[idLookup[___challengeSelection]]);
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetChallengeName", MethodType.Normal)]
        static void PatchGetChallengeName(int challenge, ref string __result)
        {
            if (challenge >= 90)
            {
                ChallengeData data = ChallengeHandler.GetChallengeDataByRuntimeID(challenge);
                if (data != null)
                {
                    __result = data.name;
                }
                else __result = "Dragon Circuit";
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetChallengeReward", MethodType.Normal)]
        static void PatchGetChallengeReward(int challenge, ref int __result)
        {
            if (challenge >= 90)
            {
                ChallengeData data = ChallengeHandler.GetChallengeDataByRuntimeID(challenge);
                if (data != null)
                {
                    __result = data.crystalReward;
                }
            }
        }
    }
}
