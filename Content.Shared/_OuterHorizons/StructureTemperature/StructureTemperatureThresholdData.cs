namespace Content.Shared._OuterHorizons.StructureTemperature;

[DataDefinition]
public sealed partial class StructureTemperatureThresholdData
{
    [DataField] public int? ActivateTemperature;
    [DataField] public int? DeactivateTemperature;
    [DataField] public IStructureTemperatureThresholdHandler? OnThreshold;
    [DataField] public IStructureTemperatureThresholdHandler? OnThresholdExit;
    [ViewVariables] public bool ActiveThreshold;
}
