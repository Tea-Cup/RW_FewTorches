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
}
