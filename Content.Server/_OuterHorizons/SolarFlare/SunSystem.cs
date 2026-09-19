using Content.Server._OuterHorizons.SolarFlare.Components;
using Content.Server.GameTicking;
using Content.Shared.Radiation.Components;

namespace Content.Shared.SolarFlare;

public sealed class SunSystem : EntitySystem
{

    [Dependency] private GameTicker _gameTicker = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SolarFlareComponent, ComponentInit>(OnCompInit);
        SubscribeLocalEvent<SolarFlareComponent, ComponentRemove>(OnRemove);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SolarFlareComponent, RadiationSourceComponent>();
        while (query.MoveNext(out var uid, out var solarFlare, out var radiationSource))
        {
            OnUpdateRad(uid, solarFlare, radiationSource, frameTime);
            SolarTimer(uid, solarFlare, frameTime);
        }

    }

    private void OnCompInit(EntityUid uid, SolarFlareComponent comp, ComponentInit arg)
    {
        var radSourceComp = AddComp<RadiationSourceComponent>(uid);
        radSourceComp.IgnoreDistation = true;
        radSourceComp.Slope = 0f;
        comp.RemainingTime = comp.TimeSolarFlare;
    }

    private void OnRemove(EntityUid uid, SolarFlareComponent comp, ComponentRemove args)
    {
        RemComp<RadiationSourceComponent>(uid);
    }

    private void OnUpdateRad(EntityUid uid, SolarFlareComponent solarFlare, RadiationSourceComponent radiation, float frameTime)
    {
        if (MathF.Abs(radiation.Intensity - solarFlare.SolarFlareOnRadiation) < 0.001f)
            radiation.Intensity = solarFlare.SolarFlareOnRadiation;

        float step = 0f;
        if (solarFlare.IsEndSolarFlame)
            step = solarFlare.ReductionGrowthRadiation * frameTime;
        else
            step = solarFlare.IncreaseGrowthRadiation * frameTime;

        if (radiation.Intensity < solarFlare.SolarFlareOnRadiation)
            radiation.Intensity = MathF.Min(radiation.Intensity + step, solarFlare.SolarFlareOnRadiation);

        else
            radiation.Intensity = MathF.Max(radiation.Intensity - step, solarFlare.SolarFlareOnRadiation);
    }

    private void SolarTimer(EntityUid uid, SolarFlareComponent comp, float frameTime)
    {
        if (comp.RemainingTime <= 0)
        {
            if (comp.IsEndSolarFlame)
                return;

            _gameTicker.EndRound();
            comp.SolarFlareOnRadiation = 0;
            comp.IsEndSolarFlame = true;
        }

        comp.RemainingTime -= frameTime;
    }
}
