using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OuterHorizons.Audio.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class AudioUnderwaterEffectComponent : Component
{
    [ViewVariables] public ProtoId<AudioPresetPrototype> Preset;
}
