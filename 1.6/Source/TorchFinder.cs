using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Foxy.FewTorches {
	[StaticConstructorOnStartup]
	public static class TorchFinder {
		private static readonly Dictionary<int, int> countDarktorch = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countDarktorchFungus = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countStandingLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countUraniumLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countSanguophageMeetingLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countDarktorchWallLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countDarktorchFungusWallLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countWallLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countUraniumWallLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countSanguophageMeetingWallLamp = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countSanguphageMeetingTorch = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> countSanguophageMeetingTorchWallLamp = new Dictionary<int, int>();

		public static ThingDef Darktorch { get; }
		public static ThingDef DarktorchFungus { get; }
		public static ThingDef StandingLamp { get; }
		public static ThingDef UraniumLamp { get; }
		public static ThingDef SanguophageMeetingLamp { get; }
		public static ThingDef DarktorchWallLamp { get; }
		public static ThingDef DarktorchFungusWallLamp { get; }
		public static ThingDef WallLamp { get; }
		public static ThingDef UraniumWallLamp { get; }
		public static ThingDef SanguophageMeetingWallLamp { get; }
		public static ThingDef SanguphageMeetingTorch { get; }
		public static ThingDef SanguophageMeetingTorchWallLamp { get; }

		public static MethodInfo MethodCheckDarkLight { get; }
		public static MethodInfo MethodCheckBloodLight { get; }

		static TorchFinder() {
			if (ModsConfig.IdeologyActive) {
				Darktorch = DefDatabase<ThingDef>.GetNamed("Darktorch");
				DarktorchFungus = DefDatabase<ThingDef>.GetNamed("DarktorchFungus");
				DarktorchWallLamp = DefDatabase<ThingDef>.GetNamed("DarktorchWallLamp");
				DarktorchFungusWallLamp = DefDatabase<ThingDef>.GetNamed("DarktorchFungusWallLamp");
			}
			if (ModsConfig.BiotechActive) {
				SanguophageMeetingLamp = DefDatabase<ThingDef>.GetNamed("SanguophageMeetingLamp");
				SanguophageMeetingWallLamp = DefDatabase<ThingDef>.GetNamed("SanguophageMeetingWallLamp");
				SanguphageMeetingTorch = DefDatabase<ThingDef>.GetNamed("SanguphageMeetingTorch");
				SanguophageMeetingTorchWallLamp = DefDatabase<ThingDef>.GetNamed("SanguophageMeetingTorchWallLamp");
			}
			StandingLamp = DefDatabase<ThingDef>.GetNamed("StandingLamp");
			UraniumLamp = DefDatabase<ThingDef>.GetNamed("UraniumLamp");
			WallLamp = DefDatabase<ThingDef>.GetNamed("WallLamp");
			UraniumWallLamp = DefDatabase<ThingDef>.GetNamed("UraniumWallLamp");

			MethodCheckDarkLight = AccessTools.Method(typeof(TorchFinder), nameof(CheckDarkLight));
			MethodCheckBloodLight = AccessTools.Method(typeof(TorchFinder), nameof(CheckBloodLight));
		}

		private static bool IsValidDarkLight(Thing t, IntVec3 pos) {
			if (t.Position.DistanceToSquared(pos) > 25) return false;
			CompGlower glow = t.TryGetComp<CompGlower>();
			if (glow == null) return false;
			if (!glow.Glows) return false;
			if (t.def == Darktorch && t.def == DarktorchFungus) return true;
			if (DarklightUtility.IsDarklight(glow.GlowColor.ToColor)) return true;
			return false;
		}

		private static bool IsValidBloodLight(Thing t, IntVec3 pos) {
			if (t.Position.DistanceToSquared(pos) > 64) return false;
			return t.TryGetComp<CompGlower>()?.Glows ?? false;
		}

		public static bool CheckDarkLight(IntVec3 pos, Map map) {
			Room room = pos.GetRoom(map);
			if (room == null) {
				Logger.Error("Checking for dark light in null room");
				return false;
			}

			ThingDef[] defs = new[] {
				countDarktorch.GetWithFallback(map.uniqueID, 0) > 0 ? Darktorch : null,
				countDarktorchFungus.GetWithFallback(map.uniqueID, 0) > 0 ? DarktorchFungus : null,
				countStandingLamp.GetWithFallback(map.uniqueID, 0) > 0 ? StandingLamp : null,
				countUraniumLamp.GetWithFallback(map.uniqueID, 0) > 0 ? UraniumLamp : null,
				countSanguophageMeetingLamp.GetWithFallback(map.uniqueID, 0) > 0 ? SanguophageMeetingLamp : null,
				countDarktorchWallLamp.GetWithFallback(map.uniqueID, 0) > 0 ? DarktorchWallLamp : null,
				countDarktorchFungusWallLamp.GetWithFallback(map.uniqueID, 0) > 0 ? DarktorchFungusWallLamp : null,
				countWallLamp.GetWithFallback(map.uniqueID, 0) > 0 ? WallLamp : null,
				countUraniumWallLamp.GetWithFallback(map.uniqueID, 0) > 0 ? UraniumWallLamp : null,
				countSanguophageMeetingWallLamp.GetWithFallback(map.uniqueID, 0) > 0 ? SanguophageMeetingWallLamp : null,
			};

			foreach (Region region in room.Regions) {
				for (int i = 0; i < defs.Length; ++i) {
					if (defs[i] == null) continue;
					foreach (Thing t in region.ListerThings.ThingsOfDef(defs[i])) {
						if (IsValidDarkLight(t, pos)) return true;
					}
				}
			}

			return false;
		}

		public static bool CheckBloodLight(IntVec3 pos, Map map) {
			Room room = pos.GetRoom(map);
			if (room == null) {
				Logger.Error("Checking for blood light in null room");
				return false;
			}

			ThingDef[] defs = new[] {
				countSanguophageMeetingLamp.GetWithFallback(map.uniqueID, 0) > 0 ? SanguophageMeetingLamp : null,
				countSanguophageMeetingWallLamp.GetWithFallback(map.uniqueID, 0) > 0 ? SanguophageMeetingWallLamp : null,
				countSanguphageMeetingTorch.GetWithFallback(map.uniqueID, 0) > 0 ? SanguphageMeetingTorch : null,
				countSanguophageMeetingTorchWallLamp.GetWithFallback(map.uniqueID, 0) > 0 ? SanguophageMeetingTorchWallLamp : null,
			};

			foreach (Region region in room.Regions) {
				for (int i = 0; i < defs.Length; ++i) {
					if (defs[i] == null) continue;
					foreach (Thing t in region.ListerThings.ThingsOfDef(defs[i])) {
						if (IsValidBloodLight(t, pos)) return true;
					}
				}
			}

			return false;
		}

		public static void Notify_ThingAdded(Thing t) {
			switch (t.def.defName) {
				case "Darktorch": countDarktorch.Increment(t.Map.uniqueID); break;
				case "DarktorchFungus": countDarktorchFungus.Increment(t.Map.uniqueID); break;
				case "StandingLamp": countStandingLamp.Increment(t.Map.uniqueID); break;
				case "UraniumLamp": countUraniumLamp.Increment(t.Map.uniqueID); break;
				case "SanguophageMeetingLamp": countSanguophageMeetingLamp.Increment(t.Map.uniqueID); break;
				case "DarktorchWallLamp": countDarktorchWallLamp.Increment(t.Map.uniqueID); break;
				case "DarktorchFungusWallLamp": countDarktorchFungusWallLamp.Increment(t.Map.uniqueID); break;
				case "WallLamp": countWallLamp.Increment(t.Map.uniqueID); break;
				case "UraniumWallLamp": countUraniumWallLamp.Increment(t.Map.uniqueID); break;
				case "SanguophageMeetingWallLamp": countSanguophageMeetingWallLamp.Increment(t.Map.uniqueID); break;
				case "SanguphageMeetingTorch": countSanguphageMeetingTorch.Increment(t.Map.uniqueID); break;
				case "SanguophageMeetingTorchWallLamp": countSanguophageMeetingTorchWallLamp.Increment(t.Map.uniqueID); break;
			}
		}

		public static void Notify_ThingRemoved(Thing t) {
			switch (t.def.defName) {
				case "Darktorch": countDarktorch.Decrement(t.Map.uniqueID); break;
				case "DarktorchFungus": countDarktorchFungus.Decrement(t.Map.uniqueID); break;
				case "StandingLamp": countStandingLamp.Decrement(t.Map.uniqueID); break;
				case "UraniumLamp": countUraniumLamp.Decrement(t.Map.uniqueID); break;
				case "SanguophageMeetingLamp": countSanguophageMeetingLamp.Decrement(t.Map.uniqueID); break;
				case "DarktorchWallLamp": countDarktorchWallLamp.Decrement(t.Map.uniqueID); break;
				case "DarktorchFungusWallLamp": countDarktorchFungusWallLamp.Decrement(t.Map.uniqueID); break;
				case "WallLamp": countWallLamp.Decrement(t.Map.uniqueID); break;
				case "UraniumWallLamp": countUraniumWallLamp.Decrement(t.Map.uniqueID); break;
				case "SanguophageMeetingWallLamp": countSanguophageMeetingWallLamp.Decrement(t.Map.uniqueID); break;
				case "SanguphageMeetingTorch": countSanguphageMeetingTorch.Decrement(t.Map.uniqueID); break;
				case "SanguophageMeetingTorchWallLamp": countSanguophageMeetingTorchWallLamp.Decrement(t.Map.uniqueID); break;
			}
		}

		public static void Decrement<K>(this Dictionary<K, int> dict, K key) {
			if (!dict.TryGetValue(key, out int value)) value = 0;
			dict[key] = value - 1;
		}
	}
}
