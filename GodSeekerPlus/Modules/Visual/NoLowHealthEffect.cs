using MonoMod.Cil;

namespace GodSeekerPlus.Modules.Visual;

[Category(nameof(Visual))]
[ToggleableLevel(ToggleableLevel.ReloadSave)]
internal sealed class NoLowHealthEffect : Module {
	private protected override void Load() {
		On.PlayMakerFSM.Start += ModifyFSM;
		IL.HeroAnimationController.PlayIdle += RemoveAnimation;
	}

	private protected override void Unload() {
		On.PlayMakerFSM.Start -= ModifyFSM;
		IL.HeroAnimationController.PlayIdle -= RemoveAnimation;
	}

	private void ModifyFSM(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self) {
		orig(self);

		if (self is {
			name: "Health",
			FsmName: "Low Health FX"
		}) {
			ModifyLowHealthFXFSM(self);
		} else if (self is {
			name: "Damage Effect",
			FsmName: "Knight Damage"
		}) {
			ModifyDamageEffectFSM(self);
		}
	}

	private static void ModifyLowHealthFXFSM(PlayMakerFSM fsm) {
		fsm.ChangeTransition("Init", "LOW", "Idle");
		fsm.ChangeTransition("HUD In HP Check", "LOW", "Idle");
		fsm.RemoveTransition("Idle", "HERO DAMAGED");
	}

	private static void ModifyDamageEffectFSM(PlayMakerFSM fsm) =>
		fsm.ChangeTransition("Check Focus Prompt", FsmEvent.Finished.Name, "Leak");

	private void RemoveAnimation(ILContext il) {
		int index = new ILCursor(il).Goto(0).GotoNext(
			MoveType.After,
			inst => inst.MatchLdstr("Idle Hurt"),
			inst => inst.MatchCallvirt<tk2dSpriteAnimator>("Play"),
			inst => inst.MatchRet()
		).Index;

		new ILCursor(il).Goto(0).RemoveRange(index);
	}
}
