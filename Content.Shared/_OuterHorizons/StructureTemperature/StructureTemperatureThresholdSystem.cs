using Content.Shared.Temperature;

namespace Content.Shared._OuterHorizons.StructureTemperature;

public sealed class StructureTemperatureThresholdSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StructureTemperatureThresholdComponent, OnTemperatureChangeEvent>(OnTemperatureChange);
    }

    private void OnTemperatureChange(Entity<StructureTemperatureThresholdComponent> ent, ref OnTemperatureChangeEvent args)
    {
        foreach (var threshold in ent.Comp.Thresholds)
        {
            if (args.CurrentTemperature > threshold.ActivateTemperature)
            {
                if(threshold.ActiveThreshold)
                    continue;

                threshold.OnThreshold?.Act(ent.Owner, args, EntityManager.EntitySysManager.DependencyCollection);
                threshold.ActiveThreshold = true;
            }

            if (args.CurrentTemperature < threshold.DeactivateTemperature)
            {
                if (!threshold.ActiveThreshold)
                    continue;

                threshold.OnThresholdExit?.Act(ent.Owner, args, EntityManager.EntitySysManager.DependencyCollection);
                threshold.ActiveThreshold = false;
            }
        }
    }
}
