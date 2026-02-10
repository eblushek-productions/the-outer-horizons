using Content.Server.Speech.Components;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class IlleismAccentSystem : EntitySystem
{
    [Dependency] private readonly ReplacementAccentSystem _replacement = default!;

    private const string LocNameKey = "{$name}";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<IlleismAccentComponent, AccentGetEvent>(OnAccent);
    }

    private bool MostlyUppercase(string message)
    {
        int totalLetters = 0;
        int uppercaseLetters = 0;

        // Iterate through each character in the string
        foreach (char c in message)
        {
            if (char.IsLetter(c)) // Check if the character is a letter
            {
                totalLetters++;
                if (char.IsUpper(c)) // Check if the letter is uppercase
                {
                    uppercaseLetters++;
                }
            }
        }

        if (totalLetters < 2)
        {
            return false;
        }

        return uppercaseLetters > totalLetters / 2;
    }

    private void OnAccent(EntityUid uid, IlleismAccentComponent component, AccentGetEvent args)
    {
        var message = _replacement.ApplyReplacements(args.Message, "illeism");

        var name = Name(uid);
        foreach (var sep in component.SplitOnStrings)
        {
            name = name.Split(sep)[0];
        }

        // TODO: Might be nice in the future to replace some instances with pronouns as well
        // e.g. "I'm new here, and I could use some help" -> "Urist is new here, and he could use some help"
        message = MostlyUppercase(message)
            // ToUpper is needed on both because ApplyReplacements will capitalize the key
            ? message.Replace(LocNameKey.ToUpper(), name.ToUpper())
            : message.Replace(LocNameKey, name);
        args.Message = message;
    }
}
