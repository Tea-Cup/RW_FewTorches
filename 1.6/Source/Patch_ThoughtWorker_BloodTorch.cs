using HarmonyLib;
using RimWorld;
using Verse;

namespace Foxy.FewTorches {
	[HarmonyPatch(typeof(ThoughtWorker_BloodTorch), "CurrentStateInternal")]
	public static class Patch_ThoughtWorker_BloodTorch {
		public static bool Prefix(Pawn p, ref ThoughtState __result) {
			__result = TorchFinder.CheckBloodLight(p.Position, p.Map) ? ThoughtState.ActiveAtStage(0) : false;
			return false;
		}
	}
}
