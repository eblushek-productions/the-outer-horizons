using Content.Server._OuterHorizons.SolarFlare.Components;
using Robust.Shared.Physics.Events;

namespace Content.Server._OuterHorizons;

public sealed class MagneticFieldSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MagneticFieldComponent, StartCollideEvent>(OnStartCollide);
        SubscribeLocalEvent<MagneticFieldComponent, EndCollideEvent>(OnEndCollide);
    }

    private void OnStartCollide(Entity<MagneticFieldComponent> entity, ref StartCollideEvent args)
    {
        EnsureComp<RadiationImmuneComponent>(args.OtherEntity);
    }

    private void OnEndCollide(Entity<MagneticFieldComponent> entity, ref EndCollideEvent args)
    {
        RemComp<RadiationImmuneComponent>(args.OtherEntity);
    }
}
