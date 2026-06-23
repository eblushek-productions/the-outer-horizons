using Content.Shared._OuterHorizons.EyeOffsetInCombatMode;

namespace Content.Server.TheOuterHorizons.EyeOffsetInCombatMode.Systems;

public sealed partial class EyeOffsetInCombatModeSystem : EntitySystem
{
    [Dependency] private SharedEyeSystem _eye = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EyeOffsetInCombatModeComponent, ComponentStartup>(OnCompInit);
        SubscribeLocalEvent<EyeOffsetInCombatModeComponent, ComponentShutdown>(OnCompShutdoown);
    }

    private void OnCompInit(Entity<EyeOffsetInCombatModeComponent> ent, ref ComponentStartup args)
    {
        if (TryComp<EyeComponent>(ent.Owner, out var eyeComp))
        {
            _eye.SetPvsScale(ent.Owner, eyeComp.PvsScale + ent.Comp.PvsIncrease);
            Dirty(ent.Owner, eyeComp);
        }
    }

    private void OnCompShutdoown(Entity<EyeOffsetInCombatModeComponent> ent, ref ComponentShutdown args)
    {
        if (TryComp<EyeComponent>(ent.Owner, out var eyeComp))
        {
            _eye.SetPvsScale(ent.Owner, eyeComp.PvsScale - ent.Comp.PvsIncrease);
            Dirty(ent.Owner, eyeComp);
        }
    }

}
