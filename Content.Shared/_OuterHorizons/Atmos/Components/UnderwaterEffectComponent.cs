using Robust.Shared.GameStates;

namespace Content.Shared._OuterHorizons.Atmos.Components;

/// <summary>
///     It marks the entity is in space or not.
///     Specially used for client side audio effects and another systems
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class UnderwaterEffectComponent : Component
{
}
