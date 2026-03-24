using Content.Shared._OuterHorizons.StructureTemperature;
using Content.Shared.Temperature;

namespace Content.Shared._OuterHorizons.Temperature;

[DataDefinition]
public sealed partial class PressureThrusterOnTemperatureHigh : IStructureTemperatureThresholdHandler
{
    public void Act(EntityUid entityUid, OnTemperatureChangeEvent args, IDependencyCollection dependencies)
    {
        dependencies.Resolve<IEntityManager>().EnsureComponent<PressureThrusterDisabledComponent>(entityUid);
    }
}

[DataDefinition]
public sealed partial class PressureThrusterOnTemperatureLow : IStructureTemperatureThresholdHandler
{
    public void Act(EntityUid entityUid, OnTemperatureChangeEvent args, IDependencyCollection dependencies)
    {
        dependencies.Resolve<IEntityManager>().RemoveComponent<PressureThrusterDisabledComponent>(entityUid);
    }
}
