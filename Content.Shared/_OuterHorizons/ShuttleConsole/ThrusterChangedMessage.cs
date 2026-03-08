using Robust.Shared.Serialization;

namespace Content.Shared._OuterHorizons.ShuttleConsole;

[Serializable, NetSerializable]
public sealed class ThrusterChangedMessage(ThrusterInfoInterfaceState thrusterInfoState) : BoundUserInterfaceMessage
{
    public ThrusterInfoInterfaceState ThrusterInfoState = thrusterInfoState;
}
