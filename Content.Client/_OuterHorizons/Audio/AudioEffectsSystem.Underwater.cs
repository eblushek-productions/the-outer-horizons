using Content.Shared._OuterHorizons.Audio.Components;
using Content.Shared._OuterHorizons.Atmos.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Prototypes;

namespace Content.Client._OuterHorizons.Audio;

public sealed partial class AudioEffectsSystem
{
    private readonly ProtoId<AudioPresetPrototype> _underwaterEffectPreset = "Underwater";

    private float _soundMuffleInSpace = -10.0f;

    public void InitializeUnderwater()
    {
    }

    public void ShutdownUnderwater()
    {
    }

    public void OnUnderwaterInit(Entity<AudioComponent> ent, ref ComponentInit args)
    {
    }

    public bool OnUnderwaterEffected(Entity<AudioComponent> ent)
    {
        if (_player.LocalEntity is not { Valid: true } player || !Exists(player))
            return false;

        if (!HasComp<UnderwaterEffectComponent>(player))
            return false;// TryApplyUnderwaterOutside(ent);

        return TryApplyUnderwater(ent);
    }

    public bool TryApplyUnderwaterOutside(Entity<AudioComponent> sound)
    {
        if (TerminatingOrDeleted(sound) || Paused(sound))
            return false;

        if (sound.Comp.Global)
            return false;

        if (TryComp<TransformComponent>(sound, out var xformComp)
            && xformComp.GridUid is null)
        {
            ApplyEffect(sound);
            return true;
        }

        return false;
    }

    public bool TryApplyUnderwater(Entity<AudioComponent> sound)
    {
        if (TerminatingOrDeleted(sound) || Paused(sound))
            return false;

        if (sound.Comp.Global)
            return false;

        ApplyEffect(sound);
        return true;
    }

    private void ApplyEffect(Entity<AudioComponent> sound)
    {
        TryAddEffect(sound, _underwaterEffectPreset);

        var underwatercomp = EnsureComp<AudioUnderwaterEffectComponent>(sound);
        underwatercomp.Preset = _underwaterEffectPreset;

        Audio.SetVolume(sound, _soundMuffleInSpace, sound);
    }

    public bool TryRemoveUnderwater(Entity<AudioComponent> sound, AudioUnderwaterEffectComponent? underwaterComp = null)
    {
        if (!Resolve(sound, ref underwaterComp))
            return false;

        if (!TryRemoveEffect(sound, underwaterComp.Preset))
            return false;

        return true;
    }
}
