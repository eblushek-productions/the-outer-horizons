using Content.Server.NodeContainer.Nodes;
using Content.Server.Shuttles.Components;
using Content.Shared.Atmos;

namespace Content.Server._OuterHorizons.CustomThruster;

[RegisterComponent]
public sealed partial class PressureThrusterComponent : Component
{
    [DataField]
    public string InletName = "inlet";
    public PipeNode Inlet;

    [DataField]
    public float PressureСonsumption = 100f;

    [ViewVariables(VVAccess.ReadOnly)]
    public ThrusterComponent ThrusterComponent;

    [ViewVariables(VVAccess.ReadOnly)]
    public float Pressure => Inlet.Air.Pressure;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextExhaust = TimeSpan.Zero;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan ExhaustTimeout = TimeSpan.FromSeconds(0.5);
}
