
using Robust.Shared.Audio;

namespace Content.Server._OuterHorizons.SolarFlare.Components;

[RegisterComponent]
public sealed partial class SolarFlareComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float SolarFlareOnRadiation = 400;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float IncreaseGrowthRadiation = 0.05f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float ReductionGrowthRadiation = 0.1f;

    [DataField]
    public int TimeSolarFlare = 140;

    [ViewVariables(VVAccess.ReadOnly)]
    public float RemainingTime;

    public bool IsEndSolarFlame = false;
}
