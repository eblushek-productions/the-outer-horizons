using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._OuterHorizons.ShuttleConsole;

[Serializable, NetSerializable]
public sealed class ThrusterInfoInterfaceState(List<ThrusterUnitData> thrusterData)
{
    public List<ThrusterUnitData> ThrusterData = thrusterData;
}

[Serializable, NetSerializable]
public record struct ThrusterUnitData(
    NetEntity ThrusterEntity,
    NetCoordinates Coordinates,
    Direction Direction,
    float InletPressure,
    float ThrusterTemperature,
    float? ThrusterHeatDamageThreshold);
