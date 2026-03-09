namespace Content.Shared._OuterHorizons.StructureTemperature;

[DataDefinition]
public sealed partial class StructureTemperatureThresholdData
{
    [DataField] public int Temperature;
    [DataField] public IStructureTemperatureThresholdHandler? OnThreshold;
}
