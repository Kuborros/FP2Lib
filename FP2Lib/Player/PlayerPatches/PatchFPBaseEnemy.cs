using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPBaseEnemy
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPBaseEnemy), "HealthDrain", [typeof(float),typeof(FPCharacterID)])]
        static void PatchFPBaseEnemyHealthDrain(FPCharacterID character, ref int ___healthDrainType)
        {
            if (character > FPCharacterID.NEERA)
            {
                PlayableChara chara = PlayerHandler.GetPlayableCharaByRuntimeIdSafe((int)character);
                switch (chara.element)
                {
                    case CharacterElement.WATER:
                        ___healthDrainType = 2;
                        break;
                    case CharacterElement.METAL:
                        ___healthDrainType = 4;
                        break;
                    case CharacterElement.WOOD:
                        ___healthDrainType = 0;
                        break;
                    case CharacterElement.EARTH:
                        ___healthDrainType = 1;
                        break;
                    case CharacterElement.FIRE:
                    default:
                        ___healthDrainType = 3;
                        break;
                }
            }
        }
    }
}
