using System.Linq;
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
        var thresholds = ent.Comp.Thresholds.ToList();
        thresholds.Sort((a, b) => b.Temperature.CompareTo(a.Temperature));

        StructureTemperatureThresholdData? selected = null;

        foreach (var threshold in thresholds)
        {
            if (args.CurrentTemperature >= threshold.Temperature)
            {
                if (selected == null || threshold.Temperature > selected.Temperature)
                    selected = threshold;
            }
        }

        if(selected is null || selected.Equals(ent.Comp.SelectedThreshold))
            return;

        selected.OnThreshold?.Act(ent, args, EntityManager.EntitySysManager.DependencyCollection);
    }
}
