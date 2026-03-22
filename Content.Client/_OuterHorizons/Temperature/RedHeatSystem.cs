using Content.Shared._OuterHorizons.Temperature;
using Robust.Client.GameObjects;

namespace Content.Client._OuterHorizons.Temperature;

public sealed class RedHeatSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RedHeatComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(Entity<RedHeatComponent> ent, ref AppearanceChangeEvent args)
    {
        if (!_appearanceSystem.TryGetData<string>(ent, RedHeadVisualLayers.Main, out var layerName, args.Component))
            return;

        SetLayer(ent, layerName);
    }

    public void SetLayer(EntityUid uid, string? layerName)
    {
        var comp = EnsureComp<ActiveRedHeatComponent>(uid);
        if (comp.ActiveLayer is not null)
            _spriteSystem.LayerSetVisible(uid, comp.ActiveLayer, false);

        if (string.IsNullOrEmpty(layerName))
        {
            RemComp<ActiveRedHeatComponent>(uid);
            return;
        }

        comp.ActiveLayer = layerName;
        _spriteSystem.LayerSetVisible(uid, comp.ActiveLayer, true);
    }
}
