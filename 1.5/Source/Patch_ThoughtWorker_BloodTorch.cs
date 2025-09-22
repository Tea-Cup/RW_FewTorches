using HarmonyLib;
using RimWorld;
using Verse;

namespace Foxy.FewTorches {
	[HarmonyPatch(typeof(ThoughtWorker_BloodTorch), "CurrentStateInternal")]
	public static class Patch_ThoughtWorker_BloodTorch {
		private const float distanceSquared = 64f;

		public static bool Prefix(Pawn p, ref ThoughtState __result) {
			__result = Injection(p) ? ThoughtState.ActiveAtStage(0) : false;
			return false;
		}

		private static bool IsValidTorch(Pawn p, Thing t) {
			if (t.Position.DistanceToSquared(p.Position) > distanceSquared) return false;
			return t.TryGetComp<CompGlower>()?.Glows ?? false;
		}

		private static bool Injection(Pawn p) {
			if (!ModsConfig.BiotechActive) return false;
			if (p.Suspended) return false;

			bool has_torches = p.Map.listerThings.ThingsOfDef(RimWorld.ThingDefOf.SanguphageMeetingTorch).Count > 0;
			bool has_wtorches = p.Map.listerThings.ThingsOfDef(ThingDefOf.SanguophageMeetingTorchWallLamp).Count > 0;
			bool has_lamps = p.Map.listerThings.ThingsOfDef(ThingDefOf.SanguophageMeetingLamp).Count > 0;
			bool has_wlamps = p.Map.listerThings.ThingsOfDef(ThingDefOf.SanguophageMeetingWallLamp).Count > 0;
			if (!has_torches && !has_wtorches && !has_lamps && !has_wlamps) return false;

			Room pawnRoom = p.GetRoom();
			foreach (Region region in pawnRoom.Regions) {
				if (has_torches) {
					foreach (Thing t in region.ListerThings.ThingsOfDef(RimWorld.ThingDefOf.SanguphageMeetingTorch)) {
						if (IsValidTorch(p, t)) return true;
					}
				}
				if (has_wtorches) {
					foreach (Thing t in region.ListerThings.ThingsOfDef(ThingDefOf.SanguophageMeetingTorchWallLamp)) {
						if (IsValidTorch(p, t)) return true;
					}
				}
				if (has_lamps) {
					foreach (Thing t in region.ListerThings.ThingsOfDef(ThingDefOf.SanguophageMeetingLamp)) {
						if (IsValidTorch(p, t)) return true;
					}
				}
				if (has_wlamps) {
					foreach (Thing t in region.ListerThings.ThingsOfDef(ThingDefOf.SanguophageMeetingWallLamp)) {
						if (IsValidTorch(p, t)) return true;
					}
				}
			}

			return false;
		}
	}
}
