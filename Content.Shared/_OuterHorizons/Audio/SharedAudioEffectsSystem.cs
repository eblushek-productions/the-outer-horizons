using Content.Shared.GameTicking;
using Content.Shared._OuterHorizons.Audio.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Shared._OuterHorizons.Audio;

public partial class SharedAudioEffectsSystem : EntitySystem
{
    [Dependency] protected readonly SharedAudioSystem Audio = default!;
    [Dependency] protected readonly IPrototypeManager ProtoManager = default!;
    [Dependency] protected readonly IConfigurationManager Cfg = default!;
    [Dependency] private readonly INetManager _net = default!;

    /// <summary>
    /// Захешированные эффекты под их прототипами пренитов. Позволяет не засрать слоты OpenAL сотней одинаковых эффектов
    /// </summary>
    private static readonly Dictionary<ProtoId<AudioPresetPrototype>, EntityUid> CachedEffects = new ();
    private static CancellationTokenSource _tokenSource = new();

    private static readonly TimeSpan RaceConditionWaiting = TimeSpan.FromTicks(10L);

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GridInitializeEvent>(OnGridInit);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(_ => Clear());
    }

    public override void Shutdown()
    {
        base.Shutdown();

        Clear();
    }

    private void OnGridInit(GridInitializeEvent ev)
    {
        EnsureComp<EchoEffectComponent>(ev.EntityUid);
    }

    private static void Clear()
    {
        CachedEffects.Clear();

        _tokenSource.Cancel();
        _tokenSource = new();
    }

    /// <summary>
    /// Добавляет переданный эффект к звуку
    /// </summary>
    public bool TryAddEffect(Entity<AudioComponent> sound, ProtoId<AudioPresetPrototype> preset)
    {
        if (!CachedEffects.TryGetValue(preset, out var effect) && !TryCreateEffect(preset, out effect))
            return false;

        if (_net.IsServer)
        {
            Timer.Spawn(RaceConditionWaiting, () => Audio.SetAuxiliary(sound, sound, effect), _tokenSource.Token);
        }
        else
        {
            Audio.SetAuxiliary(sound, sound, effect);
        }

        return true;
    }

    /// <summary>
    /// Пытается убрать данный эффект со звука
    /// </summary>
    public bool TryRemoveEffect(Entity<AudioComponent> sound, ProtoId<AudioPresetPrototype> preset)
    {
        if (!CachedEffects.TryGetValue(preset, out var effect))
            return false;

        if (sound.Comp.Auxiliary != effect)
            return false;

        Audio.SetAuxiliary(sound, sound, null);
        return true;
    }

    public void RemoveAllEffects(Entity<AudioComponent> sound)
    {
        Audio.SetAuxiliary(sound, sound, null);
    }

    /// <summary>
    /// Пытается создать эффект и захешировать его
    /// </summary>
    /// <param name="preset">Пресет эффектов</param>
    /// <param name="effectStuff">Получаемый эффект. Не представляет собой ничего, когда метод возвращает false</param>
    /// <returns>Возвращает успешно ли создание и хеширование эффекта</returns>
    public bool TryCreateEffect(ProtoId<AudioPresetPrototype> preset, out EntityUid effectStuff)
    {
        effectStuff = default;

        if (!ProtoManager.TryIndex(preset, out var prototype))
            return false;

        var effect = Audio.CreateEffect();
        var auxiliary = Audio.CreateAuxiliary();

        Audio.SetEffectPreset(effect.Entity, effect.Component, prototype);
        Audio.SetEffect(auxiliary.Entity, auxiliary.Component, effect.Entity);

        if (!Exists(auxiliary.Entity))
            return false;

        if (!CachedEffects.TryAdd(preset, auxiliary.Entity))
            return false;

        effectStuff = auxiliary.Entity;

        return true;
    }

    public static bool HasEffect(Entity<AudioComponent> sound, ProtoId<AudioPresetPrototype> preset)
    {
        if (!CachedEffects.TryGetValue(preset, out var effect))
            return false;

        return sound.Comp.Auxiliary == effect;
    }

    public bool TryGetEffect(Entity<AudioComponent> sound, [NotNullWhen(true)] out ProtoId<AudioPresetPrototype>? preset)
    {
        preset = null;

        foreach (var (storedPreset, auxUid) in CachedEffects)
        {
            if (sound.Comp.Auxiliary != auxUid)
                continue;

            preset = storedPreset;
            return true;
        }

        return false;
    }
}
