using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FP2Lib.Player.PlayerPatches
{
    internal class PatchFPPlayer
    {
        internal static readonly MethodInfo m_AirMoves = SymbolExtensions.GetMethodInfo(() => HandleActionAirMoves());
        internal static readonly MethodInfo m_GroundMoves = SymbolExtensions.GetMethodInfo(() => HandleActionGroundMoves());

        internal static void HandleActionGroundMoves()
        {
            PlayerHandler.currentCharacter.GroundMoves?.Invoke();
        }

        internal static void HandleActionAirMoves()
        {
            PlayerHandler.currentCharacter.AirMoves?.Invoke();
        }

        internal static void HandleGrindJump(FPPlayer player)
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

        internal static bool HandleWaterSurface(FPBaseObject targetWaterSurface, object _discard)
        {
            if (FPSaveManager.character > FPCharacterID.NEERA && PlayerHandler.currentCharacter != null)
            {
                if (PlayerHandler.currentCharacter.disableSwimming)
                {
                    return false;
                }
            }
            return targetWaterSurface != null;
        }

        internal static bool HandleWaterSurfaceInverse(FPBaseObject targetWaterSurface, object _discard)
        {
            return !HandleWaterSurface(targetWaterSurface, _discard);
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
            codes.Add(CodeInstruction.Call(typeof(PatchFPPlayer), nameof(HandleGrindJump)));
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

        //Swimming disable patches
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "State_InAir", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPlayer), "Action_Jump", MethodType.Normal)]
        [HarmonyPatch(typeof(FPPlayer), "Action_Guard", MethodType.Normal)]
        static IEnumerable<CodeInstruction> PlayerStateInAirTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                /*
                Replaces:
                	targetWaterSurface != null
                With:
                    HandleWaterSurface(instance,null) 

                This can be done because "!= null" internally creates a call to IsEqual(Object,null)

                All this Opcode checking is there to not accidentaly patch assebly already overwritten by Sonic Mod's implementation.
                */


                if (codes[i].opcode == OpCodes.Ldfld && codes[i + 1].opcode == OpCodes.Ldnull && codes[i - 1].opcode == OpCodes.Ldarg_0)
                {
                    codes[i + 2] = Transpilers.EmitDelegate(HandleWaterSurface);
                    break;
                }
            }
            return codes;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FPPlayer), "ReturnToGeneralState", [typeof(bool), typeof(bool)])]
        static IEnumerable<CodeInstruction> PlayerGeneralStateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; i++)
            {
                /*
                Replaces:
                	targetWaterSurface == null
                With:
                    HandleWaterSurfaceInverse(instance,null) 
                */

                if (codes[i].opcode == OpCodes.Ldfld && codes[i + 1].opcode == OpCodes.Ldnull && codes[i - 1].opcode == OpCodes.Ldarg_0)
                {
                    codes[i + 2] = Transpilers.EmitDelegate(HandleWaterSurfaceInverse);
                    break;
                }
            }
            return codes;
        }
    }
}
