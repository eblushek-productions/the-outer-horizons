using Content.Shared._OuterHorizons.Temperature;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;

namespace Content.Server._OuterHorizons.Temperature;

public sealed class RedHeatSystem : EntitySystem
{
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RedHeatComponent, OnTemperatureChangeEvent>(OnTemperatureChange);
    }

    private void OnTemperatureChange(Entity<RedHeatComponent> ent, ref OnTemperatureChangeEvent args)
    {
        var redHeatComponent = ent.Comp;

        redHeatComponent.SpriteData.Sort((a, b) => b.Temperature.CompareTo(a.Temperature));

        RedHeadSpriteData? selected = null;

        foreach (var data in redHeatComponent.SpriteData)
        {
            if (args.CurrentTemperature >= data.Temperature)
            {
                if (selected == null || data.Temperature > selected.Temperature)
                    selected = data;
            }
        }

        _appearanceSystem.SetData(ent, RedHeadVisualLayers.Main, selected?.SpriteLayer ?? "");
    }
}
