using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Foxy.FewTorches {
	[HarmonyPatch(typeof(ThoughtWorker_BloodTorch), "CurrentStateInternal")]
	public static class Patch_ThoughtWorker_BloodTorch {
		private static readonly MethodInfo methodClosestThingReachable = AccessTools.Method(typeof(GenClosest), nameof(GenClosest.ClosestThingReachable));
		private static readonly MethodInfo methodInjection = AccessTools.Method(typeof(Patch_ThoughtWorker_BloodTorch), nameof(Injection));

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) {
			List<CodeInstruction> list = instructions.ToList();
			int index = list.FindIndex(x => x.Calls(methodClosestThingReachable));

			if (index <= 0) {
				Logger.Error("Failed to transpile RimWorld.ThoughtWorker_BloodTorch.CurrentStateInternal: injection start index not found");
				return list;
			}

			if (!list[index + 1].Branches(out Label? label)) {
				Logger.Error("Failed to transpile RimWorld.ThoughtWorker_BloodTorch.CurrentStateInternal: branch target not found");
				return list;
			}

			list.Insert(index + 2, CodeInstruction.LoadArgument(1));
			list.Insert(index + 3, new CodeInstruction(OpCodes.Call, methodInjection));
			list.Insert(index + 4, new CodeInstruction(OpCodes.Brtrue, label));
			return list;
		}

		private static Thing Injection(Pawn p) {
			Room pawnRoom = p.GetRoom();
			Thing thing = GenClosest.ClosestThingReachable(
				p.Position,
				p.Map,
				ThingRequest.ForDef(ThingDefOf.SanguophageMeetingLamp),
				PathEndMode.ClosestTouch,
				TraverseParms.For(p),
				8f,
				(Thing b) => {
					Room room = b.GetRoom();
					return (room == null || room == pawnRoom) && (b.TryGetComp<CompGlower>()?.Glows ?? false);
				}
			);
			return thing;
		}
	}
}
