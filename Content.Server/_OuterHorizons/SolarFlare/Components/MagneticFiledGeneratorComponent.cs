using Robust.Shared.Prototypes;

namespace Content.Server._OuterHorizons.SolarFlare.Components;

[RegisterComponent]
public sealed partial class MagneticFieldGeneratorComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? FieldUid = null;

    [DataField("spawn", required: true)]
    public EntProtoId? ProtoSpawnId = null!;
}
