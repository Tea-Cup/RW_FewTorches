using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Foxy.FewTorches {
	[HarmonyPatch(typeof(CompSanguophageMeetingFire), nameof(CompSanguophageMeetingFire.PostDraw))]
	public static class Patch_CompSanguophageMeetingFire {
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (CodeInstruction inst in instructions) {
				if (inst.LoadsField(AccessTools.Field(typeof(Rot4), nameof(Rot4.North)))) {
					yield return CodeInstruction.LoadArgument(0);
					yield return CodeInstruction.LoadField(typeof(ThingComp), nameof(ThingComp.parent));
					yield return CodeInstruction.Call(typeof(Thing), $"get_{nameof(Thing.Rotation)}");
				} else {
					yield return inst;
				}
			}
		}
	}
}
