using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPPlayer
    {

        public static FPPlayer player;

        internal static readonly MethodInfo m_AirMoves = SymbolExtensions.GetMethodInfo(() => HandleActionAirMoves());
        internal static readonly MethodInfo m_GroundMoves = SymbolExtensions.GetMethodInfo(() => HandleActionGroundMoves());
        internal static readonly MethodInfo m_Grind = SymbolExtensions.GetMethodInfo(() => HandleGrindJump());

        internal static void HandleActionGroundMoves()
        {
            PlayerHandler.currentCharacter.GroundMoves?.Invoke();
        }

        internal static void HandleActionAirMoves()
        {
            PlayerHandler.currentCharacter.AirMoves?.Invoke();
        }

        internal static void HandleGrindJump()
        {
            if (player.input.left)
            {
                player.direction = FPDirection.FACING_LEFT;
            }
            else if (player.input.right)
            {
                player.direction = FPDirection.FACING_RIGHT;
            }
            player.Action_Jump();
            HandleActionAirMoves();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPPlayer), "Update", MethodType.Normal)]
        static void PatchPlayerUpdate(FPPlayer __instance)
        {
            //Yeet the player instance for our own nefarious uses.
            player = __instance;
        }


        //AutoGuard
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
                    codes[i + 1].operand = airStart;
                    airEnd = (Label)codes[i + 4].operand;
                }
                if (codes[i].opcode == OpCodes.Switch && codes[i - 1].opcode == OpCodes.Ldloc_1)
                {
                    codes[i + 1].operand = groundStart;
                    groundEnd = (Label)codes[i + 4].operand;
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

        //Crouch
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
                    codes[i + 1].operand = groundStart;
                    groundEnd = (Label)codes[i + 4].operand;
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

        //Looking up
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
                    codes[i + 1].operand = groundStart;
                    groundEnd = (Label)codes[i + 4].operand;
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

        //Grounded
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
                    codes[i + 1].operand = groundStart;
                    groundEnd = (Label)codes[i + 4].operand;
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
                    codes[i + 1].operand = airStart;
                    airEnd = (Label)codes[i + 4].operand;
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
                    codes[i + 1].operand = airStart;
                    airEnd = (Label)codes[i + 4].operand;
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
                    codes[i + 1].operand = airStart;
                    airEnd = (Label)codes[i + 4].operand;
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
                    codes[i + 1].operand = airStart;
                    airEnd = (Label)codes[i + 4].operand;
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
                    airEnd = (Label)codes[i + 1].operand;
                    codes[i + 1].operand = airStart;
                }

            }
            CodeInstruction airCodeStart = new CodeInstruction(OpCodes.Ldarg_0);
            airCodeStart.labels.Add(airStart);

            codes.Add(airCodeStart);
            codes.Add(new CodeInstruction(OpCodes.Call, m_Grind));
            codes.Add(new CodeInstruction(OpCodes.Br, airEnd));

            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "PseudoGrindRail", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerPseudoGrindTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label airStart = il.DefineLabel();

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Switch && (codes[i - 1].opcode == OpCodes.Ldloc_S))
                {
                    //Code is identical for everyone but Neera
                    codes[i + 2].labels.Add(airStart);
                    codes[i + 1].operand = airStart;
                }
            }
            return codes;
        }
    }
}
