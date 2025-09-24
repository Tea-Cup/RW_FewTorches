using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace Foxy.FewTorches {
	[HarmonyPatch(typeof(DarklightUtility), nameof(DarklightUtility.IsDarklight))]
	public static class Patch_IsDarklight {
		public static bool Prefix(Color color, ref bool __result) {
			if (CompProperties_SpecialGlower.AllDarklightColors.Contains(color)) {
				__result = true;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(DarklightUtility), nameof(DarklightUtility.IsDarklightAt))]
	public static class Patch_IsDarklightAt {
		private static readonly MethodInfo miIsDarklight = AccessTools.Method(
			typeof(DarklightUtility),
			nameof(DarklightUtility.IsDarklight)
		);

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			int index = list.FindIndex(x => x.Calls(miIsDarklight));
			if(index < 0) {
				Logger.Error("Failed to inject into DarklightUtility.IsDarklightAt");
				return instructions;
			}

			Logger.Log($"Removing from index {index}");

			for(int i = 0; i < 6; ++i) {
				Logger.Log($"{index - i} {list[index - i]}");
				list.RemoveAt(index - i);
			}
			index -= 5;

			Logger.Log($"Adding from index {index}");
			Logger.Log($"{index} {list[index]}");
			list.Insert(index++, CodeInstruction.LoadArgument(0));
			Logger.Log($"{index} {list[index]}");
			list.Insert(index++, CodeInstruction.LoadArgument(1));
			Logger.Log($"{index} {list[index]}");
			list.Insert(index++, new CodeInstruction(OpCodes.Call, TorchFinder.MethodCheckDarkLight));
			Logger.Log($"{index} {list[index]}");

			return list;
		}
	}
}
