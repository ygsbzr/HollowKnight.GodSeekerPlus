namespace GodSeekerPlus.Modules.Restrictions;

[Category(nameof(Restrictions))]
internal sealed class NoNailAttack : Module {
	private protected override void Load() =>
		On.HeroController.CanAttack += SuppressAttack;

	private protected override void Unload() =>
		On.HeroController.CanAttack -= SuppressAttack;

	private bool SuppressAttack(On.HeroController.orig_CanAttack orig, HeroController self) => false;
}
