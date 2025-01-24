using BepInEx.Logging;
using FP2Lib.Badge;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FP2Lib.Map
{
    internal class MapPatches
    {
        private static readonly ManualLogSource BadgeLogSource = FP2Lib.logSource;

        //Patching in the new maps
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MenuWorldMap), "Start", MethodType.Normal)]
        static void PatchMenuWorldMap(ref FPMapScreen[] mapScreens)
        {
  
        }
    }
}
