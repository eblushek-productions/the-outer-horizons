using Content.Server.Speech.EntitySystems;

namespace Content.Server.Speech.Components;

[RegisterComponent]
[Access(typeof(IlleismAccentSystem))]
public sealed partial class IlleismAccentComponent : Component
{
    /// <summary>
    /// The strings that should be used to split the character's name.
    /// Only the part up to the first occurrence is kept.
    /// </summary>
    [DataField]
    public List<string> SplitOnStrings = new();
}
