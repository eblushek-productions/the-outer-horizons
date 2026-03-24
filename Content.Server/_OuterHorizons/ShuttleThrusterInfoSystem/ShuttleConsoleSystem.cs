using Content.Server._OuterHorizons.CustomThruster;
using Content.Server.Shuttles.Components;
using Content.Shared._OuterHorizons.ShuttleConsole;
using Content.Shared.Shuttles.Components;
using Content.Shared.Temperature.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;

namespace Content.Server._OuterHorizons.ShuttleThrusterInfoSystem;

public sealed class ShuttleThrusterInfoSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var shuttleQuery = EntityQueryEnumerator<ShuttleConsoleComponent, TransformComponent>();
        while (shuttleQuery.MoveNext(out var consoleUid, out var shuttleConsoleComponent, out var xform))
        {
            var shuttleUid = xform.GridUid;

            if (!TryComp<ShuttleComponent>(shuttleUid, out var shuttleComponent))
                continue;

            var shuttleInfo = EnsureComp<ShuttleInfoComponent>(shuttleUid.Value);

            if (shuttleInfo.NextTimeout > _gameTiming.CurTime)
                return;

            shuttleInfo.NextTimeout = _gameTiming.CurTime + shuttleInfo.Timeout;

            var thrusterData = new List<ThrusterUnitData>();

            for (var i = 0; i < 4; i++)
            {
                var thrusterList = shuttleComponent.LinearThrusters[i];
                var direction = (Direction)(i * 2);

                foreach (var thrusterUid in thrusterList)
                {
                    if (!TryComp<TemperatureComponent>(thrusterUid, out var temperatureComponent) ||
                       !TryComp<PressureThrusterComponent>(thrusterUid, out var pressureThrusterComponent) ||
                       !TryComp<TemperatureDamageComponent>(thrusterUid, out var containerTemperatureComponent))
                        continue;

                    var oneData = new ThrusterUnitData(
                        GetNetEntity(thrusterUid),
                        GetNetCoordinates(Transform(thrusterUid).Coordinates),
                        direction,
                        pressureThrusterComponent.Inlet.Air.Pressure,
                        temperatureComponent.CurrentTemperature,
                        containerTemperatureComponent.HeatDamageThreshold);

                    thrusterData.Add(oneData);
                }
            }

            var message = new ThrusterChangedMessage(new ThrusterInfoInterfaceState(thrusterData));

            if (_ui.HasUi(consoleUid, ShuttleConsoleUiKey.Key))
            {
                _ui.ServerSendUiMessage(consoleUid, ShuttleConsoleUiKey.Key, message);
            }
        }
    }
}
