namespace Content.Shared._OuterHorizons.Temperature;

[RegisterComponent]
public sealed partial class RedHeatComponent : Component
{
    [DataField] public List<RedHeadSpriteData> SpriteData = [];
}

[DataDefinition]
public sealed partial class RedHeadSpriteData
{
    [DataField] public int Temperature;
    [DataField] public string SpriteLayer;
}
