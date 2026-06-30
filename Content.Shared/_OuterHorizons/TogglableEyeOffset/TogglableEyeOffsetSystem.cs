using Content.Shared.Actions;
using Content.Shared.Toggleable;

namespace Content.Shared._OuterHorizons.TogglableEyeOffset.Systems;

public sealed partial class TogglableEyeOffsetSystem : EntitySystem
{
    [Dependency] private SharedEyeSystem _eye = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TogglableEyeOffsetComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<TogglableEyeOffsetComponent, ToggleOffsetActionEvent>(OnToggleAction);
        SubscribeLocalEvent<TogglableEyeOffsetComponent, ComponentStartup>(OnCompInit);
        SubscribeLocalEvent<TogglableEyeOffsetComponent, ComponentShutdown>(OnCompShutdoown);
    }

    private void OnMapInit(Entity<TogglableEyeOffsetComponent> ent, ref MapInitEvent args)
    {
        if (string.IsNullOrEmpty(ent.Comp.Action))
            return;

        _actions.AddAction(ent, ref ent.Comp.ActionEntity, ent.Comp.Action);
        _actions.SetToggled(ent.Comp.ActionEntity, ent.Comp.IsActive);
        Dirty(ent);
    }

    private void OnToggleAction(Entity<TogglableEyeOffsetComponent> ent, ref ToggleOffsetActionEvent args)
    {
        ent.Comp.IsActive = !ent.Comp.IsActive;
        args.Handled = true;
        Dirty(ent);
    }

    private void OnCompInit(Entity<TogglableEyeOffsetComponent> ent, ref ComponentStartup args)
    {
        if (TryComp<EyeComponent>(ent.Owner, out var eyeComp))
        {
            _eye.SetPvsScale(ent.Owner, eyeComp.PvsScale + ent.Comp.PvsIncrease);
            Dirty(ent.Owner, eyeComp);
        }
    }

    private void OnCompShutdoown(Entity<TogglableEyeOffsetComponent> ent, ref ComponentShutdown args)
    {
        if (TryComp<EyeComponent>(ent.Owner, out var eyeComp))
        {
            _eye.SetPvsScale(ent.Owner, eyeComp.PvsScale - ent.Comp.PvsIncrease);
            Dirty(ent.Owner, eyeComp);
        }
    }

}
