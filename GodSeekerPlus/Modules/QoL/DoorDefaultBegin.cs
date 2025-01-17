using Mono.Cecil.Cil;

using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

using UnityEngine.EventSystems;

namespace GodSeekerPlus.Modules.QoL;

[Category(nameof(QoL))]
[DefaultEnabled]
internal sealed class DoorDefaultBegin : Module {
	private ILHook? hook = null;

	private protected override void Load() => hook = new(
		typeof(BossDoorChallengeUI)
			.GetMethod("ShowSequence", BindingFlags.Instance | BindingFlags.NonPublic)
			.GetStateMachineTarget(),
		ChangeSelection
	);

	private protected override void Unload() => hook?.Dispose();

	private void ChangeSelection(ILContext il) {
		ILCursor cursor = new ILCursor(il).Goto(0);

		//
		// The following lines are going to be removed:
		//
		// if (bossDoorChallengeUI.buttons.Length != 0) {
		//	EventSystem.current.SetSelectedGameObject(bossDoorChallengeUI.buttons[0].gameObject);
		// }
		// InputHandler.Instance.StartUIInput();
		//

		// Go to the first IL line of the above lines
		cursor.GotoNext(
			i => i.MatchLdloc(1),
			i => i.MatchLdfld(
				typeof(BossDoorChallengeUI)
					.GetField("buttons", BindingFlags.Instance | BindingFlags.NonPublic)
			)
		);

		// Remove all IL lines from the `LdLoc.1` to the one before `Ret`
		do {
			cursor.Remove();
		} while (cursor.TryGotoNext(i => !i.MatchRet()));

		// Load `self` (BossDoorChallengeUI)
		cursor.Emit(OpCodes.Ldloc_1);
		cursor.EmitDelegate(SelectBegin);

		// Fix return
		cursor.Emit(OpCodes.Ldc_I4_0);
	}

	private static void SelectBegin(BossDoorChallengeUI self) {
		EventSystem.current.SetSelectedGameObject(
			self.gameObject.Child("Panel", "BeginButton")
		);

		InputHandler.Instance.StartUIInput();
	}
}
