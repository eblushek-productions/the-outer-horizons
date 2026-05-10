using Content.Client._OuterHorizons.Audio.Components;
using Content.Shared._OuterHorizons.Audio;
using Robust.Client.Player;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Client._OuterHorizons.Audio;

public sealed partial class AudioEffectsSystem : SharedAudioEffectsSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeUnderwater();
        InitializeEcho();

        SubscribeLocalEvent<AudioComponent, ComponentInit>(OnInit, before: [typeof(SharedAudioSystem)]);
        SubscribeLocalEvent<AudioComponent, ComponentAdd>(OnAdd);

        SubscribeLocalEvent<AudioEffectedComponent, ComponentStartup>(OnAudioEffected, after: [typeof(SharedAudioSystem)]);
    }

    public override void Shutdown()
    {
        base.Shutdown();

        ShutdownUnderwater();
        ShutdownEcho();
    }

    private void OnInit(Entity<AudioComponent> ent, ref ComponentInit args)
    {
        OnUnderwaterInit(ent, ref args);
        OnEchoInit(ent, ref args);
    }

    private void OnAdd(Entity<AudioComponent> ent, ref ComponentAdd args)
    {
        EnsureComp<AudioEffectedComponent>(ent);
    }

    private void OnAudioEffected(Entity<AudioEffectedComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<AudioComponent>(ent.Owner, out var audio))
            return;

        var underwatered = OnUnderwaterEffected((ent.Owner, audio));
        if (!underwatered)
            OnEchoEffected((ent.Owner, audio));
    }
}
