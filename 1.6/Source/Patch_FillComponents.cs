using System;
using HarmonyLib;
using Verse;

namespace Foxy.FewTorches {
	[HarmonyPatch(typeof(Map), "FillComponents")]
	internal class Patch_FillComponents {
		public static void Prefix(Map __instance) {
			__instance.thingListChangedCallbacks.onThingAdded = (Action<Thing>)Delegate.Combine(
				__instance.thingListChangedCallbacks.onThingAdded,
				new Action<Thing>(TorchFinder.Notify_ThingAdded)
			);
			__instance.thingListChangedCallbacks.onThingRemoved = (Action<Thing>)Delegate.Combine(
				__instance.thingListChangedCallbacks.onThingRemoved,
				new Action<Thing>(TorchFinder.Notify_ThingRemoved)
			);
		}
	}
}
