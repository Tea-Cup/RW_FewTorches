using RimWorld;
using UnityEngine;
using Verse;

namespace Foxy.FewTorches {
	[StaticConstructorOnStartup]
	public class CompUraniumFireOverlay : CompFireOverlayBase {
		public static readonly Graphic UraniumFireGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/UraniumFire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

		public new CompProperties_UraniumFireOverlay Props => (CompProperties_UraniumFireOverlay)props;

		public override void PostDraw() {
			base.PostDraw();
			CompGlower compGlower = parent.TryGetComp<CompGlower>();
			if (compGlower == null || compGlower.Glows) {
				Vector3 drawPos = parent.DrawPos;
				drawPos.y += 1f / 26f;
				UraniumFireGraphic.Draw(drawPos, parent.Rotation, parent);
			}
		}
	}
}
