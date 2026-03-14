using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OuterHorizons.Audio.Components;

/*
    Mark a grid, that should apply a audio preset (like echo) for audio sources.
*/
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EchoEffectComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<AudioPresetPrototype> Preset = "PipeResonant";
}
