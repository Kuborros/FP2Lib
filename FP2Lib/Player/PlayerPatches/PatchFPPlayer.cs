using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPPlayer
    {

        //Patches should only fire AirMoves and GroundMoves events. Mod creator handles their own postfixes etc.

        /*
        internal static readonly MethodInfo m_AirMoves = SymbolExtensions.GetMethodInfo(() => Action_Spade_AirMoves());
        internal static readonly MethodInfo m_Jump = SymbolExtensions.GetMethodInfo(() => Action_Jump());
        internal static readonly MethodInfo m_GroundMoves = SymbolExtensions.GetMethodInfo(() => Action_Spade_GroundMoves());

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update", MethodType.Normal)]
        static void PatchPlayerUpdate(FPPlayer __instance, float ___speedMultiplier)
        {
            player = __instance;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "AutoGuard", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerHurtTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label airStart = il.DefineLabel();
            Label airEnd = il.DefineLabel();
            Label groundStart = il.DefineLabel();
            Label groundEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_0)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(airStart).ToArray();
                    codes[i].operand = targets;
                    airEnd = (Label)codes[i + 1].operand;
                }
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(groundStart).ToArray();
                    codes[i].operand = targets;
                    groundEnd = (Label)codes[i + 1].operand;
                    break;
                }
            }
            CodeInstruction airCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            airCodeStart.labels.Add(airStart);

            codes.Add(airCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_AirMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, airEnd));

            CodeInstruction groundCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            groundCodeStart.labels.Add(groundStart);

            codes.Add(groundCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_GroundMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, groundEnd));


            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_Crouching", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerCrouchTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label groundStart = il.DefineLabel();
            Label groundEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(groundStart).ToArray();
                    codes[i].operand = targets;
                    groundEnd = (Label)codes[i + 1].operand;
                    break;
                }
            }

            CodeInstruction groundCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            groundCodeStart.labels.Add(groundStart);

            codes.Add(groundCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_GroundMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, groundEnd));


            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_LookUp", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerLookUpTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label groundStart = il.DefineLabel();
            Label groundEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_0)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(groundStart).ToArray();
                    codes[i].operand = targets;
                    groundEnd = (Label)codes[i + 1].operand;
                    break;
                }
            }

            CodeInstruction groundCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            groundCodeStart.labels.Add(groundStart);

            codes.Add(groundCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_GroundMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, groundEnd));


            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_Ground", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerGroundTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label groundStart = il.DefineLabel();
            Label groundEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_S)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(groundStart).ToArray();
                    codes[i].operand = targets;
                    groundEnd = (Label)codes[i + 1].operand;
                    break;
                }
            }

            CodeInstruction groundCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            groundCodeStart.labels.Add(groundStart);

            codes.Add(groundCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_GroundMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, groundEnd));


            return codes;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_InAir", MethodType.Normal)]
        static void PatchPlayerInAir()
        {
            if (player.currentAnimation == "AirThrow" && player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                player.SetPlayerAnimation("Jumping");
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_InAir", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerAirTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label airStart = il.DefineLabel();
            Label airEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_2)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(airStart).ToArray();
                    codes[i].operand = targets;
                    airEnd = (Label)codes[i + 1].operand;
                }

            }
            CodeInstruction airCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            airCodeStart.labels.Add(airStart);

            codes.Add(airCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_AirMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, airEnd));

            return codes;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_Hanging", MethodType.Normal)]
        static void PatchPlayerHang()
        {
            upDash = true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_Hanging", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerHangTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label airStart = il.DefineLabel();
            Label airEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_S)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(airStart).ToArray();
                    codes[i].operand = targets;
                    airEnd = (Label)codes[i + 1].operand;
                }

            }
            CodeInstruction airCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            airCodeStart.labels.Add(airStart);

            codes.Add(airCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_AirMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, airEnd));

            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_LadderClimb", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerLadderTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label airStart = il.DefineLabel();
            Label airEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_0)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(airStart).ToArray();
                    codes[i].operand = targets;
                    airEnd = (Label)codes[i + 1].operand;
                }

            }
            CodeInstruction airCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            airCodeStart.labels.Add(airStart);

            codes.Add(airCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_AirMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, airEnd));

            return codes;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_Swimming", MethodType.Normal)]
        static void PatchPlayerSwim()
        {
            upDash = true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_Swimming", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerSwimTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label airStart = il.DefineLabel();
            Label airEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(airStart).ToArray();
                    codes[i].operand = targets;
                    airEnd = (Label)codes[i + 1].operand;
                }

            }
            CodeInstruction airCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            airCodeStart.labels.Add(airStart);

            codes.Add(airCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_AirMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, airEnd));

            return codes;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "State_GrindRail", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPlayer), "PseudoGrindRail", MethodType.Normal)]
        static void PatchPlayerGrind()
        {
            upDash = true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_GrindRail", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerGrindTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label airStart = il.DefineLabel();
            Label airEnd = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && (codes[i - 1].opcode == OpCodes.Ldloc_2))
                {
                    Label[] targets = (Label[])codes[i].operand;
                    targets = targets.AddItem(airStart).ToArray();
                    codes[i].operand = targets;
                    airEnd = (Label)codes[i + 1].operand;
                }

            }
            CodeInstruction airCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            airCodeStart.labels.Add(airStart);

            codes.Add(airCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_Jump));
            codes.Add(new CodeInstruction(OpCodes.Call, m_AirMoves));
            codes.Add(new CodeInstruction(OpCodes.Br, airEnd));

            return codes;
        }
        */
    }
}
