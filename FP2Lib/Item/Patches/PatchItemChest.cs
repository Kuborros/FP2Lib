using BepInEx;
using FP2Lib.Stage;
using HarmonyLib;

namespace FP2Lib.Item.Patches
{
    internal class PatchItemChest
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemChest),"Start", MethodType.Normal)]
        static void PatchItemChestStart(ItemChest __instance)
        {
            if (__instance.contents == FPItemChestContent.POWERUP)
            {
                if (FPStage.currentStage != null && __instance.powerupType == FPPowerup.RANDOM)
                {
                    // Stages over 32 are custom stages
                    if (FPStage.currentStage.stageID > 32)
                    {
                        CustomStage stage = StageHandler.getCustomStageByRuntimeId(FPStage.currentStage.stageID);
                        if (stage != null && !stage.itemUID.IsNullOrWhiteSpace())
                        {
                            ItemData data = ItemHandler.GetItemDataByUid(stage.itemUID);
                            if (data != null)
                            {
                                __instance.powerupType = (FPPowerup)data.itemID;
                                __instance.itemSprite = data.sprite;
                            }
                        }
                    }
                }
            }
        }
    }
}
