using Content.Server.NodeContainer.Nodes;

namespace Content.Server._OuterHorizons.CustomThruster;

[RegisterComponent]
public sealed partial class PressureThrusterComponent : Component
{
    [DataField]
    public string InletName = "inlet";

    [DataField]
    public float MoleOutletLimit;

    [DataField]
    public float MoleMaxOutletLimit = 100f;

    [DataField]
    public float MoleMinOutletLimit = 10f;

    [DataField]
    public float AccelerationEfficiency = 1f;

    [ViewVariables]
    public PipeNode Inlet;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextExhaust = TimeSpan.Zero;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan ExhaustTimeout = TimeSpan.FromSeconds(0.25);
}
