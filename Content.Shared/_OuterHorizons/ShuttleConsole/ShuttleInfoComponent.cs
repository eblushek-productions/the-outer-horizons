namespace Content.Shared._OuterHorizons.ShuttleConsole;

[RegisterComponent]
public sealed partial class ShuttleInfoComponent : Component
{
    [DataField] public TimeSpan Timeout = TimeSpan.FromSeconds(0.5);

    [ViewVariables] public TimeSpan NextTimeout;
}
