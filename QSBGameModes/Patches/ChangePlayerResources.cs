using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QSBGameModes.Patches
{
	[HarmonyPatch(typeof(PlayerResources))]
	public static class ChangePlayerResources
	{
		
		//TODO :: TRY TO FIND A BETTER WAY TO DO THIS
		
		public const float DefaultMaxFuel = 100f;
		public const float DefaultMaxOxygen = 450f;

		public static bool ChangeValues = false;
		public static float MaxFuel = 100f;
		public static float MaxOxygen = 450f;


		[HarmonyTranspiler]
		[HarmonyPatch("OnRemoveSuit")]
		[HarmonyPatch("OnSuitUp")]
		[HarmonyPatch("UpdateFuel")]
		static IEnumerable<CodeInstruction> TranspilerForMaxFuel(IEnumerable<CodeInstruction> instructions){
			
			
			return new CodeMatcher(instructions)
			.MatchForward(false, //Searchs for everytime that the following "list" of codes is matched
					new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && Convert.ToSingle(i.operand) == DefaultMaxFuel)
				//^ Checks the Opcode to see if it is ldc.r4  and holds the value of DefaultMaxFuel
				).Repeat(matcher => //For every match, execute this part
					matcher.RemoveInstruction()// Remove the original instruction
					.Insert(
						CodeInstruction.Call(typeof(ChangePlayerResources), nameof(ReturnMaxFuel))
				))
			.InstructionEnumeration();
		}

		public static float ReturnMaxFuel() 
		{
			return ChangeValues? MaxFuel : DefaultMaxFuel; 
		}

		[HarmonyTranspiler]
		[HarmonyPatch("OnRemoveSuit")]
		[HarmonyPatch("OnSuitUp")]
		[HarmonyPatch("UpdateOxygen")]
		static IEnumerable<CodeInstruction> TranspilerForMaxOxygen(IEnumerable<CodeInstruction> instructions)
		{
			return new CodeMatcher(instructions)
			.MatchForward(false, //Searchs for everytime that the following "list" of codes is matched
					new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && Convert.ToSingle(i.operand) == DefaultMaxOxygen)
				//^ Checks the Opcode to see if it is ldc.r4  and holds the value of DefaultMaxOxygen
				).Repeat(matcher => //For every match, execute this part
					matcher.RemoveInstruction()// Remove the original instruction
					.Insert(
						CodeInstruction.Call(typeof(ChangePlayerResources),nameof(ReturnMaxOxygen))
				))
			.InstructionEnumeration();
		}

		public static float ReturnMaxOxygen()
		{
			return ChangeValues ? MaxOxygen : DefaultMaxOxygen;
		}

	}
}
