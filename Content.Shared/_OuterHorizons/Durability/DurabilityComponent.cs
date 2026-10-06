using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._OuterHorizons.Durability;

[RegisterComponent]
public sealed partial class DurabilityComponent : Component
{
    [DataField]
    public int MaxDurability = 65;

    [DataField]
    public int Damage = 0;

    [DataField]
    public bool UseEnergy = false;

    [DataField]
    public int ChargePerUse = 0;

    [DataField]
    public SoundSpecifier BreakSound = new SoundCollectionSpecifier("MetalBreak")
    {
        Params = AudioParams.Default.WithVolume(1f).WithMaxDistance(3f)
    };

    [DataField(customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? ReplaceOnBreak;
}
