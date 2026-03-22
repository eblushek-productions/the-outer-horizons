using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Temperature.Systems;
using Content.Shared.Atmos;
using Content.Shared.NodeContainer;
using Content.Shared.Temperature.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Server._OuterHorizons.CustomThruster;

public sealed class PressureThrusterSystem : EntitySystem
{
    [Dependency] private readonly NodeContainerSystem _nodeContainerSystem = default!;
    [Dependency] private readonly ThrusterSystem _thrusterSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly TemperatureSystem _temperatureSystem = default!;

    private EntityQuery<NodeContainerComponent> _nodeQuery;

    public override void Initialize()
    {
        SubscribeLocalEvent<PressureThrusterComponent, ComponentInit>(OnInit);
        _nodeQuery = GetEntityQuery<NodeContainerComponent>();
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<PressureThrusterComponent, ThrusterComponent>();

        while (query.MoveNext(out var uid, out var pressureThrusterComponent, out var thrusterComponent))
        {
            if(_gameTiming.CurTime < pressureThrusterComponent.NextExhaust)
                continue;

            pressureThrusterComponent.NextExhaust = _gameTiming.CurTime + pressureThrusterComponent.ExhaustTimeout;

            var ent = new Entity<PressureThrusterComponent, ThrusterComponent>(
                uid,
                pressureThrusterComponent,
                thrusterComponent);

            UseThrust(ent);
        }
    }

    private void OnInit(Entity<PressureThrusterComponent> ent, ref ComponentInit args)
    {
        if (HasComp<ApcPowerReceiverComponent>(ent))
        {
            Log.Error($"Entity {Name(ent)} has ApcPowerReceiverComponent. May break some thinks!");
        }

        if (!_nodeQuery.TryComp(ent, out var container) ||
            !_nodeContainerSystem.TryGetNode(container, ent.Comp.InletName, out PipeNode? node))
        {
            Log.Error($"Entity {Name(ent)} has no node. Removing component!");
            RemComp(ent,ent.Comp);
            return;
        }

        ent.Comp.Inlet = node;
        ent.Comp.MoleOutletLimit = ent.Comp.MoleMaxOutletLimit;
    }

    private void UseThrust(Entity<PressureThrusterComponent, ThrusterComponent> ent)
    {
        if(!UpdateCanThrust(ent) || !ent.Comp2.Firing)
            return;

        var comp = ent.Comp1;
        var gasMixture = comp.Inlet.Air;

        var outMixture = gasMixture.Remove(comp.MoleOutletLimit);

        var mass = 1f;
        if(TryComp<PhysicsComponent>(Transform(ent).GridUid, out var gridPhysic))
            mass = gridPhysic.Mass;

        UpdateTemperature(ent, outMixture);

        var acceleration = (outMixture.Pressure / mass) * comp.AccelerationEfficiency;

        Log.Debug("ACC: " + acceleration + " pressure: " + outMixture.Pressure + " mass: " + mass + " mole:" + comp.Inlet.Air.TotalMoles + " limit:" + comp.MoleOutletLimit);
        _thrusterSystem.SetThrust(ent.Comp2, acceleration);
    }

    private void UpdateTemperature(Entity<PressureThrusterComponent> ent, GasMixture outMixture, TemperatureComponent? temperature = null)
    {
        if(!Resolve(ent, ref temperature))
            return;

        var deltaTemp = outMixture.Temperature - temperature.CurrentTemperature;

        if(deltaTemp <= 0)
            return;

        _temperatureSystem.ChangeHeat(ent, deltaTemp, temperature:temperature);
    }

    private bool UpdateCanThrust(Entity<PressureThrusterComponent, ThrusterComponent> ent)
    {
        var thrusterComp = ent.Comp2;

        if (IsCanThrust(ent))
        {
            if(!thrusterComp.IsOn)
                _thrusterSystem.EnableThruster(ent,thrusterComp);
            return true;
        }

        if(thrusterComp.IsOn)
            _thrusterSystem.DisableThruster(ent,thrusterComp);

        return false;
    }

    private bool IsCanThrust(Entity<PressureThrusterComponent, ThrusterComponent> ent)
    {
        var ev = new PressureThrusterCheckEventArgs();
        RaiseLocalEvent(ent, ev);

        return !ev.Cancelled && ent.Comp1.Inlet.Air.Pressure > 0.1 && _thrusterSystem.CanEnable(ent, ent.Comp2);
    }
}

public sealed class PressureThrusterCheckEventArgs : CancellableEntityEventArgs
{
}
