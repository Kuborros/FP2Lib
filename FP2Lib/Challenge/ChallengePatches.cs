using BepInEx;
using BepInEx.Logging;
using FP2Lib.Tools;
using HarmonyLib;
using System;

namespace FP2Lib.Challenge
{
    internal class ChallengePatches
    {
        private static readonly ManualLogSource ArenaLogSource = FP2Lib.logSource;
        private static int totalChallenges = 0, totalHomeRuns = 0, totalBosses = 0, totalDojoBosses = 0, totalDojoChallenges = 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "LoadFromFile", MethodType.Normal)]
        static void PatchFPSaveManagerLoad(ref byte[] ___challengeRank, ref int[] ___challengeRecord)
        {
            //Calculate how many challenges we have
            int totalArenaChallenges = ChallengeHandler.baseChallenges + ChallengeHandler.Challenges.Count;
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
            //Add slots in file for extra challenges.
            ___challengeRank = FPSaveManager.ExpandByteArray(___challengeRank, totalArenaChallenges);
            ___challengeRecord = FPSaveManager.ExpandIntArray(___challengeRecord, totalArenaChallenges);

            //TODO: Test if vinyl-like trimming is needed!!!
            //if (___musicTracks.Length > totalTracks - 1)
            //    ___musicTracks = ___musicTracks.Take(totalTracks).ToArray();

            ChallengeHandler.WriteToStorage();
        }

        //Boss Stuff
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuArenaBossSelect), "Start", MethodType.Normal)]
        static void PatchArenaBossSelectPre(MenuArenaBossSelect __instance, ref FPHudDigit ___pfBossLabel, ref bool[] ___bossForeshadowed, ref MenuText ___bossNames)
        {
            int bossSceneOffset = __instance.bossScenes.Length;
            int bossUnlockOffset = __instance.bossUnlockRequirement.Length;
            int bossSpawnIDOffset = __instance.bossSpawnID.Length;
            int bossForeshadowOffset = ___bossForeshadowed.Length;

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
                        if (FPStage.stageNameString == "The Battlesphere")
                        {
                            __instance.bossScenes = Utils.ExpandStringArray(__instance.bossScenes, bossSceneOffset + totalBosses);
                            __instance.bossScenes[bossSceneOffset + challenge.localID] = challenge.destinationScene;

                            __instance.bossUnlockRequirement = FPSaveManager.ExpandIntArray(__instance.bossUnlockRequirement, bossUnlockOffset + totalBosses);
                            __instance.bossUnlockRequirement[bossUnlockOffset + challenge.localID] = challenge.unlockRequirement;

                            __instance.bossSpawnID = FPSaveManager.ExpandIntArray(__instance.bossSpawnID, bossSpawnIDOffset + totalBosses);
                            __instance.bossSpawnID[bossSpawnIDOffset + challenge.localID] = challenge.id;

                            ___bossForeshadowed = FPSaveManager.ExpandBoolArray(___bossForeshadowed, bossForeshadowOffset + totalBosses);
                            ___bossForeshadowed[bossForeshadowOffset + challenge.localID] = ShouldBossBeForeshadowed(challenge);

                            ___pfBossLabel.digitFrames[challenge.id] = challenge.bossIcon;
                            ___bossNames.paragraph[challenge.id] = challenge.name;
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

                            ___bossForeshadowed = FPSaveManager.ExpandBoolArray(___bossForeshadowed, bossForeshadowOffset + totalDojoBosses);
                            ___bossForeshadowed[bossForeshadowOffset + challenge.localID] = ShouldBossBeForeshadowed(challenge);

                            ___pfBossLabel.digitFrames[challenge.id] = challenge.bossIcon;
                            ___bossNames.paragraph[challenge.id] = challenge.name;
                        }
                        break;
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
            if (challenge >= 85)
            {
                ChallengeData data = ChallengeHandler.GetChallengeDataByRuntimeID(challenge);
                if (data != null)
                {
                    __result = data.bossHome;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuArenaBossSelect), "CheckAdditionalBossRequirements",MethodType.Normal)]
        static void PatchCheckAdditionalBossRequirements(int bossSlotID, ref bool __result)
        {
            if (bossSlotID >= 85)
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetChallengeName", MethodType.Normal)]
        static void PatchGetChallengeName(int challenge, ref string __result)
        {
            if (challenge >= 85)
            {
                ChallengeData data = ChallengeHandler.GetChallengeDataByRuntimeID(challenge);
                if (data != null)
                {
                    __result = data.name;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSaveManager), "GetChallengeReward", MethodType.Normal)]
        static void PatchGetChallengeReward(int challenge, ref int __result)
        {
            if (challenge >= 85)
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
