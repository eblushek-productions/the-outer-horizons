using Content.Server._OuterHorizons.SolarFlare.Components;
using Content.Shared.Power;

namespace Content.Server._OuterHorizons.SolarFlare;

public sealed class MagneticFieldGeneratorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MagneticFieldGeneratorComponent, PowerChangedEvent>(OnPowerChanged);
    }

    public void OnPowerChanged(EntityUid uid, MagneticFieldGeneratorComponent comp, ref PowerChangedEvent args)
    {
        if (!args.Powered)
        {
            if (comp.FieldUid is not null)
            {
                QueueDel(comp.FieldUid);
                comp.FieldUid = null;
            }
        }
        else
        {
            if (comp.FieldUid is null)
            {
                comp.FieldUid = Spawn(comp.ProtoSpawnId, Transform(uid).Coordinates);
            }
        }
    }
}
