using HarmonyLib;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchMenuCharacterWheel
    {



        [HarmonyPrefix]
        [HarmonyPatch(typeof(MenuCharacterWheel), "Update", MethodType.Normal)]
        static bool ReplaceMenuCharacterUpdate(MenuCharacterWheel __instance, ref SpriteRenderer ___spriteRenderer)
        {

            //Wheel can comfortably fit up to ~10 characters. Any more will go to shadow realm of "Other Characters" menu.
            //Positions are based off Lilac, who is set at 180 degree offset. Add or remove calculated offset for others
            //Calculate offset - we skip anyone without initialised data
            int totalCharacters = PatchMenuCharacterSelect.wheelcharas + 1;
            int offset = 360 / totalCharacters;

            //Apply offset to built-in characters.
            if (__instance != null)
            {
                switch (__instance.name)
                {
                    case "Menu CS Character Milla":
                        __instance.rotationOffset = 180 - (offset * 2); //3
                        break;
                    case "Menu CS Character Carol":
                        __instance.rotationOffset = 180 - offset; //2
                        break;
                    case "Menu CS Character Lilac":
                        __instance.rotationOffset = 180; //1
                        break;
                    case "Menu CS Character Neera":
                        __instance.rotationOffset = 180 - (offset * 3); //4
                        break;
                    default: break; //Customs already got their offset set.
                }
            }

            //Perform accursed math.
            float num = 5f * FPStage.frameScale;
            __instance.transform.position = new Vector3(__instance.startPosition.x + Mathf.Sin(0.017453292f * __instance.rotation) * __instance.distance.x, __instance.startPosition.y + Mathf.Cos(0.017453292f * __instance.rotation) * __instance.distance.y, Mathf.Cos(0.017453292f * __instance.rotation) * 5f + 5f);
            float z = __instance.transform.position.z;
            ___spriteRenderer.color = new Color(1f - z * 0.15f, 1f - z * 0.15f, 1f - z * 0.1f, 1f);
            if (__instance.parentObject != null)
            {
                __instance.rotation = (__instance.rotation * (num - 1f) + (float)__instance.parentObject.character * offset + __instance.rotationOffset) / num;
            }

            return false;
        }
    }
}
