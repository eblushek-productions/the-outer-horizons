using Content.Server.Shuttles.Components;

namespace Content.Server.Shuttles.Systems;

public sealed partial class ThrusterSystem : EntitySystem
{
    public void SetThrust(ThrusterComponent component, float count)
    {
        component.Thrust = count;
    }
}
