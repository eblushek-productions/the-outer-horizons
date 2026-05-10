using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Shared.NodeContainer;
using Robust.Shared.Timing;

namespace Content.Server._OuterHorizons.CustomThruster;

public sealed class PressureThrusterSystem : EntitySystem
{
    [Dependency] private readonly NodeContainerSystem _nodeContainerSystem = default!;
    [Dependency] private readonly ThrusterSystem _thrusterSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    private EntityQuery<NodeContainerComponent> _nodeQuery;

    public override void Initialize()
    {
        SubscribeLocalEvent<PressureThrusterComponent, ComponentInit>(OnInit);
        _nodeQuery = GetEntityQuery<NodeContainerComponent>();
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<PressureThrusterComponent>();

        while (query.MoveNext(out var uid, out var pressureThrusterComponent))
        {
            if(_gameTiming.CurTime < pressureThrusterComponent.NextExhaust)
                continue;

            pressureThrusterComponent.NextExhaust = _gameTiming.CurTime + pressureThrusterComponent.ExhaustTimeout;

            var ent = new Entity<PressureThrusterComponent>(uid, pressureThrusterComponent);

            UseThrust(ent);
        }
    }

    private void OnInit(Entity<PressureThrusterComponent> ent, ref ComponentInit args)
    {
        if (HasComp<ApcPowerReceiverComponent>(ent))
        {
            Log.Error($"Entity {Name(ent)} has ApcPowerReceiverComponent. May some break thinks!");
        }

        if (!_nodeQuery.TryComp(ent, out var container) ||
            !_nodeContainerSystem.TryGetNode(container, ent.Comp.InletName, out PipeNode? node))
        {
            Log.Error($"Entity {Name(ent)} has no node. Removing component!");
            RemComp(ent,ent.Comp);
            return;
        }

        ent.Comp.Inlet = node;
        ent.Comp.ThrusterComponent = EnsureComp<ThrusterComponent>(ent);
    }

    private void UseThrust(Entity<PressureThrusterComponent> ent)
    {
        if(!UpdateCanThrust(ent) || !ent.Comp.ThrusterComponent.Firing)
            return;

        var comp = ent.Comp;
        var gasMixture = comp.Inlet.Air;

        gasMixture.RemoveRatio(comp.PressureСonsumption / gasMixture.Pressure);
    }

    private bool UpdateCanThrust(Entity<PressureThrusterComponent> ent)
    {
        var thrusterComp = ent.Comp.ThrusterComponent;

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

    private bool IsCanThrust(Entity<PressureThrusterComponent> ent)
    {
        var thrusterComp = ent.Comp.ThrusterComponent;
        return ent.Comp.Pressure >= ent.Comp.PressureСonsumption && _thrusterSystem.CanEnable(ent,thrusterComp);
    }
}
