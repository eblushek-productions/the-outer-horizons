using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos.Components;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Shared.Atmos;
using Content.Shared._OuterHorizons.Electrolyzer;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;

namespace Content.Server._OuterHorizons.Electrolyzer;

[UsedImplicitly]
public sealed partial class ElectrolyzerSystem : EntitySystem
{
    [Dependency] private AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private NodeContainerSystem _nodeContainer = default!;
    [Dependency] private SharedSolutionContainerSystem _solutionContainer = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ElectrolyzerComponent, AtmosDeviceUpdateEvent>(OnUpdated);
    }

    private void OnUpdated(EntityUid uid, ElectrolyzerComponent comp, ref AtmosDeviceUpdateEvent args)
    {
        if (!comp.Enabled)
            return;

        if (TryComp<ApcPowerReceiverComponent>(uid, out var apcPowerReceiver) && !apcPowerReceiver.Powered)
            return;

        if (!_nodeContainer.TryGetNode(uid, comp.Outlet,
                out PipeNode? outlet))
            return;

        if (outlet.Air.Pressure >= Atmospherics.MaxOutputPressure)
            return;

        if (!_solutionContainer.TryGetSolution(uid, comp.SolutionName, out var solution, out _))
            return;

        var liquidAvailable = solution.Value.Comp.Solution.GetTotalPrototypeQuantity(comp.LiquidToConsume);
        if (liquidAvailable <= 0)
            return;

        var liquidToConsume = FixedPoint2.Min(
            liquidAvailable,
            FixedPoint2.New(comp.LiquidConsumptionRate * args.dt)
        );

        if (liquidToConsume <= 0)
            return;

        _solutionContainer.RemoveReagent(solution.Value, comp.LiquidToConsume, liquidToConsume);

        var temperature = solution.Value.Comp.Solution.Temperature;

        foreach (var gas in comp.ReleasedGases.Keys)
        {
            var moles = (float)liquidToConsume * comp.ReleasedGases[gas];
            if (moles > 0)
            {
                var limitGas = AtmosphereSystem.MolesToMaxPressure(
                    new GasMixture { Temperature = temperature },
                    outlet.Air,
                    Atmospherics.MaxOutputPressure);

                var actualGas = Math.Min(moles.Float(), Math.Max(limitGas, 0));
                if (actualGas > 0)
                {
                    var gasMix = new GasMixture { Temperature = temperature };
                    gasMix.SetMoles(gas, actualGas);
                    _atmosphereSystem.Merge(outlet.Air, gasMix);
                }
            }
        }
    }
}
