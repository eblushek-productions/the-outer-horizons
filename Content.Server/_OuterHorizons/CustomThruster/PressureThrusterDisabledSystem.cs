using Content.Shared._OuterHorizons.Temperature;

namespace Content.Server._OuterHorizons.CustomThruster;

public sealed class PressureThrusterDisabledSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PressureThrusterDisabledComponent, PressureThrusterCheckEventArgs>(OnCheck);
    }

    private void OnCheck(Entity<PressureThrusterDisabledComponent> ent, ref PressureThrusterCheckEventArgs args)
    {
        if (!args.Cancelled)
            args.Cancel();
    }
}
