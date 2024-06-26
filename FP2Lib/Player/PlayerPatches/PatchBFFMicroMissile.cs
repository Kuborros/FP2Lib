﻿using HarmonyLib;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchBFFMicroMissile
    {
        public static bool BFFActive = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BFFMicroMissile), "State_Default", MethodType.Normal)]
        static void PatchBFFMicroMissileDefault(BFFMicroMissile __instance, ref float ___speed, float ___explodeTimer)
        {
            if (!BFFActive)
            {
                if ((___explodeTimer > 150f && ___explodeTimer < 175f) || (___explodeTimer > 120f && ___explodeTimer < 140f) || (___explodeTimer > 90f && ___explodeTimer < 110f) || (___explodeTimer > 60f && ___explodeTimer < 80f))
                {
                    ___speed = 0;
                }
                else
                {
                    ___speed = 20;
                }

                if (__instance.target.enemy != null)
                {
                    if (__instance.target.enemy.cannotBeFrozen == true)
                    {
                        __instance.attackPower = 1;
                    }
                    else __instance.attackPower = 3;
                }
            }
            else
            {
                __instance.attackPower = 20;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BFFMicroMissile), "Collide", MethodType.Normal)]
        static bool PatchBFFMicroMissileCollide(BFFMicroMissile __instance)
        {
            if (!BFFActive)
            {
                __instance.collision = true;
                FPAudio.PlaySfx(25);
                FPStage.CreateStageObject(Explosion.classID, __instance.position.x, __instance.position.y);

                return false;
            }
            else return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BFFMicroMissile), "State_Done", MethodType.Normal)]
        static void PatchBFFMicroMissileDone(BFFMicroMissile __instance)
        {
            __instance.activationMode = FPActivationMode.NEVER_ACTIVE;
            __instance.gameObject.SetActive(false);

        }

    }
}
