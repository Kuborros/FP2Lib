using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    //Snake Platforms by default have **hardcoded** value of 32 for player's half height. It breaks on anyone with different value.
    internal class PatchSnakePlatform
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SnakePlatform), "IsStandingOnAnySegment", MethodType.Normal)]
        static void PatchSnakePlatformStanding(bool ___shrinking, bool ___growing, ref bool ___moving, ref FPPlayer ___targetPlayer, int ___numSegments, float ___segmentWidth, float ___segmentHeight, SnakePlatformBlock[] ___blocks, ref int ___currentSegment)
        {
            if (FPSaveManager.character > FPCharacterID.NEERA && ___targetPlayer == null)
            {
                FPBaseObject objRef = null;
                while (FPStage.ForEach(FPPlayer.classID, ref objRef) && !___shrinking && !___growing)
                {
                    FPPlayer fPPlayer = (FPPlayer)objRef;
                    ___currentSegment = -1;
                    ___targetPlayer = null;
                    for (int num = ___numSegments - 1; num >= 0; num--)
                    {
                        float x = fPPlayer.position.x;
                        float x2 = ___blocks[num].GetPos().x;
                        if (x <= x2 + ___segmentWidth / 2f && x >= x2 - ___segmentWidth / 2f)
                        {
                            float y = fPPlayer.position.y;
                            float y2 = ___blocks[num].GetPos().y;
                            if (y <= y2 + ___segmentHeight / 2f + fPPlayer.halfHeight && y > y2 + ___segmentHeight / 2f)
                            {
                                ___currentSegment = num;
                                ___targetPlayer = fPPlayer;
                                if (!___moving)
                                {
                                    ___moving = true;
                                }
                                FPStage.ForEachBreak();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
