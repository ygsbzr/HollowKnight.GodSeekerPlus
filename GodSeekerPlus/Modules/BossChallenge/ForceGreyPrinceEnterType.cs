namespace GodSeekerPlus.Modules.BossChallenge;

[Category(nameof(BossChallenge))]
[ToggleableLevel(ToggleableLevel.ChangeScene)]
[DefaultEnabled]
internal sealed class ForceGreyPrinceEnterType : Module {
	private protected override void Load() =>
		On.PlayMakerFSM.Start += ModifyGPFSM;

	private protected override void Unload() =>
		On.PlayMakerFSM.Start -= ModifyGPFSM;

	private void ModifyGPFSM(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self) {
		orig(self);

		if (self is {
			name: "Grey Prince",
			FsmName: "Control"
		}) {
			ModifyGPFSM(self);

			Logger.LogDebug("Grey Prince FSM modified");
		}
	}

	private static void ModifyGPFSM(PlayMakerFSM fsm) =>
		fsm.AddCustomAction(
			"Enter 1",
			() => fsm.FsmVariables.FindFsmBool("Faced Zote").Value = Ref.GS.gpzEnterType
		);
}
