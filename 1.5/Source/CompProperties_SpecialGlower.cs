using System.Collections.Generic;
using System.Reflection;
using Foxy.FewTorches;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

public abstract class CompProperties_SpecialGlower : CompProperties_Glower {
	public static HashSet<Color> AllDarklightColors { get; } = new HashSet<Color>() { DarklightUtility.DefaultDarklight };
	public ColorInt DarklightGlowColor { get; }

	public string darklightOnLabel = "ToggleDarklightOn";
	public string darklightOffLabel = "ToggleDarklightOff";

	public CompProperties_SpecialGlower(ColorInt darklight) {
		compClass = typeof(CompSpecialGlower);
		AllDarklightColors.Add((DarklightGlowColor = darklight).ToColor);
	}
}

public class CompProperties_SanguophageGlower : CompProperties_SpecialGlower {
	public CompProperties_SanguophageGlower() : base(new ColorInt(192, 127, 255)) { }
}

public class CompProperties_UraniumGlower : CompProperties_SpecialGlower {
	public CompProperties_UraniumGlower() : base(new ColorInt(150, 255, 220)) { }
}

public class CompSpecialGlower : CompGlower {
	public new CompProperties_SpecialGlower Props => (CompProperties_SpecialGlower)props;

	public override IEnumerable<Gizmo> CompGetGizmosExtra() {
		foreach (Gizmo gizmo in base.CompGetGizmosExtra()) {
			if (IsDarklightGizmo(gizmo, out bool darklight)) {
				yield return GetCustomDarklightGizmo(darklight);
			} else {
				yield return gizmo;
			}
		}
	}

	private List<CompGlower> ExtraSelectedGlowers() {
		List<CompGlower> tmpExtraGlowers = new List<CompGlower>(64);
		foreach (object item in Find.Selector.SelectedObjectsListForReading) {
			if (item == this || !(item is ThingWithComps thingWithComps)) continue;

			foreach (CompGlower comp in thingWithComps.GetComps<CompGlower>()) {
				if (comp.Props.darklightToggle) tmpExtraGlowers.Add(comp);
			}
		}
		return tmpExtraGlowers;
	}

	private Gizmo GetCustomDarklightGizmo(bool darklight) {
		return new Command_Action {
			icon = darklight ? Static.texSetNormalLight : Static.texSetDarklight,
			defaultLabel = (darklight ? Props.darklightOffLabel : Props.darklightOnLabel).Translate(),
			defaultDesc = ((darklight ? Props.darklightOffLabel : Props.darklightOnLabel) + "Desc").Translate(),
			action = () => {
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
				if(darklight) {
					SetGlowColorInternal(null);
				} else {
					GlowColor = Props.DarklightGlowColor;
				}

				foreach (CompGlower extra in ExtraSelectedGlowers()) {
					if (darklight) {
						SetGlowColorInternal(extra, null);
					} else if (extra is CompSpecialGlower special) {
						extra.GlowColor = special.Props.DarklightGlowColor;
					} else {
						extra.GlowColor = new ColorInt(DarklightUtility.DefaultDarklight);
					}
				}
			}
		};
	}

	private static readonly MethodInfo mi = AccessTools.Method(typeof(CompGlower), "SetGlowColorInternal");
	private static void SetGlowColorInternal(CompGlower comp, ColorInt? color) {
		mi.Invoke(comp, new object[] { color });
	}

	private static bool IsDarklightGizmo(Gizmo gizmo, out bool darklight) {
		if (gizmo is Command_Action ca) {
			if (ca.icon == Static.texSetNormalLight) {
				darklight = true;
				return true;
			} else if (ca.icon == Static.texSetDarklight) {
				darklight = false;
				return true;
			}
		}
		darklight = false;
		return false;
	}
}
