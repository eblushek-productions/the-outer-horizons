using Content.Shared.Temperature;

namespace Content.Shared._OuterHorizons.StructureTemperature;

[ImplicitDataDefinitionForInheritors]
public partial interface IStructureTemperatureThresholdHandler
{
    public void Act(EntityUid entityUid, OnTemperatureChangeEvent args, IDependencyCollection dependencies);
}
