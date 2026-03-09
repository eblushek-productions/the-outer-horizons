namespace Content.Shared._OuterHorizons.StructureTemperature;

[RegisterComponent]
public sealed partial class StructureTemperatureThresholdComponent : Component
{
    [DataField] public List<StructureTemperatureThresholdData> Thresholds = [];
    [ViewVariables] public StructureTemperatureThresholdData? SelectedThreshold;
}
