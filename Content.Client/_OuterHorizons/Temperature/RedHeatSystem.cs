using Content.Shared._OuterHorizons.Temperature;
using Content.Shared.Temperature;
using Content.Shared.Temperature.Components;
using Robust.Client.GameObjects;

namespace Content.Client._OuterHorizons.Temperature;

public sealed class RedHeatSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RedHeatComponent, OnTemperatureChangeEvent>(OnTemperatureChange);
    }

    private void OnTemperatureChange(Entity<RedHeatComponent> ent, ref OnTemperatureChangeEvent args)
    {
        if(!TryComp<RedHeatComponent>(ent, out var redHeatComponent))
            return;

        redHeatComponent.SpriteData.Sort((a, b) => b.Temperature.CompareTo(a.Temperature));

        RedHeadSpriteData? selected = null;

        foreach (var data in redHeatComponent.SpriteData)
        {
            _spriteSystem.LayerSetVisible(ent.Owner, data.SpriteLayer, false);

            if (args.CurrentTemperature >= data.Temperature)
            {
                if (selected == null || data.Temperature > selected.Temperature)
                    selected = data;
            }
        }

        if (selected != null)
        {
            _spriteSystem.LayerSetVisible(ent.Owner, selected.SpriteLayer, true);
        }
    }
}
