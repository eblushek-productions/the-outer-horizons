using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Audio;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Added to an enabled jetpack. Tracks gas usage on server / effect spawning on client.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveJetpackComponent : Component
{
    public float EffectCooldown = 3f; // OH14-Changes, for sfx, 0.3 > 3

    public float MaxDistance = 1f; // OH14-Changes, for sfx, 0.7 > 1

    public EntityCoordinates LastCoordinates;

    public TimeSpan TargetTime = TimeSpan.Zero;

    [DataField("jetSound")] // OH14-Changes, for sfx
    public SoundSpecifier JetSound = new SoundPathSpecifier("/Audio/_OuterHorizons/Effects/rcs_thrust.ogg"); // OH14-Changes, for sfx
}
