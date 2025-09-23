using UnityEngine;
using Verse;

namespace Foxy.FewTorches {
	[StaticConstructorOnStartup]
	internal static class Static {
		public static readonly Texture2D texSetNormalLight = ContentFinder<Texture2D>.Get("UI/Commands/SetNormalLight");
		public static readonly Texture2D texSetDarklight = ContentFinder<Texture2D>.Get("UI/Commands/SetDarklight");

		static Static() {
			Logger.Log("Applying patches...");
			new HarmonyLib.Harmony("Foxy.FewTorches").PatchAll();
			Logger.Log("Patches applied.");
		}
	}
}