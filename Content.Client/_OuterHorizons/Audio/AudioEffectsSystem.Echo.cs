using Content.Shared._OuterHorizons.Audio.Components;
using Content.Shared.CCVar;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;

namespace Content.Client._OuterHorizons.Audio;

public sealed partial class AudioEffectsSystem
{

    private bool _isEchoEnabled;

    // TODO: Should be overrided by grid's echo's comp
    private readonly ProtoId<AudioPresetPrototype> _standardEchoEffectPreset = "PipeResonant";

    public void InitializeEcho()
    {
        _isEchoEnabled = Cfg.GetCVar(CCVars.EchoEnabled);

        Cfg.OnValueChanged(CCVars.EchoEnabled, OnEnabledToggled);
    }

    public void ShutdownEcho()
    {
        Cfg.UnsubValueChanged(CCVars.EchoEnabled, OnEnabledToggled);
    }

    public void OnEchoInit(Entity<AudioComponent> ent, ref ComponentInit args)
    {
    }

    public void OnEchoEffected(Entity<AudioComponent> ent)
    {
        if (!_isEchoEnabled)
            return;

        if (_player.LocalEntity is not { Valid: true } player || !Exists(player))
            return;

        var xform = Transform(player);
        if (xform.GridUid is null)
            return;

        // Remeber - if preset can't be loaded, it means that preset is doesn't exist.
        // So - just make another preset in content's /Resources/Prototypes.
        if (!TryComp<EchoEffectComponent>(xform.GridUid, out var echoEffect))
            return;

        TryApplyEcho(ent, echoEffect.Preset);
    }

    private void OnEnabledToggled(bool enabled)
    {
        _isEchoEnabled = enabled;

        if (!enabled)
            RevertEcho();
    }

    /// <summary>
    /// Пытается применить эхо к данном звуку
    /// </summary>
    /// <param name="sound">Звук, к которому будет применен эффект</param>
    /// <param name="preset">Пресет, если нужно выставить какой-то особенный</param>
    /// <returns>Получилось или не получилось применить эффект</returns>
    public bool TryApplyEcho(Entity<AudioComponent> sound, ProtoId<AudioPresetPrototype> preset)
    {
        if (TerminatingOrDeleted(sound) || Paused(sound))
            return false;

        // Фоновая музыка не должна подвергаться эффектам эха
        if (sound.Comp.Global)
            return false;

        TryAddEffect(sound, preset);

        // Добавляем компонент-маркер к звуку, который будет хранить эффект эха
        var echoComp = EnsureComp<AudioEchoEffectAffectedComponent>(sound);
        echoComp.Preset = preset;

        return true;
    }

    /// <summary>
    /// Пытается убрать эффект эхо у выбранного звука
    /// </summary>
    public bool TryRemoveEcho(Entity<AudioComponent> sound, AudioEchoEffectAffectedComponent? echoComp = null)
    {
        if (!Resolve(sound, ref echoComp))
            return false;

        if (!TryRemoveEffect(sound, echoComp.Preset))
            return false;

        return true;
    }

    /// <summary>
    /// Убирает эффекты эхо у всех звуков, что имеют его.
    /// Вызывается при выключении эффекта эха игроком.
    /// </summary>
    private void RevertEcho()
    {
        var query = AllEntityQuery<AudioEchoEffectAffectedComponent, AudioComponent>();

        while (query.MoveNext(out var uid, out var echoComp, out var audio))
        {
            TryRemoveEcho((uid, audio), echoComp);
        }
    }
}
