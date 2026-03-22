namespace Content.Client.Stylesheets.Palette;

/// <summary>
///     Stores all style palettes in one accessible location
/// </summary>
/// <remarks>
///     Technically not limited to only colors, can store like, standard padding amounts, and font sizes, maybe?
/// </remarks>
public static class Palettes
{
    // muted tones
    public static readonly ColorPalette Navy = ColorPalette.FromHexBase("#3C3E4D", lightnessShift: 0.05f, chromaShift: 0.0045f); // 4f5376
    public static readonly ColorPalette Cyan = ColorPalette.FromHexBase("#323442", lightnessShift: 0.05f, chromaShift: 0.0045f); // 42586a
    public static readonly ColorPalette Slate = ColorPalette.FromHexBase("#434857"); // 545562
    public static readonly ColorPalette Neutral = ColorPalette.FromHexBase("#328499");

    // status tones
    public static readonly ColorPalette Red = ColorPalette.FromHexBase("#EA6D2C", chromaShift: 0.02f);
    public static readonly ColorPalette Amber = ColorPalette.FromHexBase("#1B7D80");
    public static readonly ColorPalette Green = ColorPalette.FromHexBase("#328499");
    public static readonly StatusPalette Status = new([Red.Base, Amber.Base, Green.Base]);

    // highlight tones
    public static readonly ColorPalette Gold = ColorPalette.FromHexBase("#5DB3C9");
    public static readonly ColorPalette Maroon = ColorPalette.FromHexBase("#EA6D2C");

    // Intended to be used with `ModulateSelf` to darken / lighten something
    public static readonly ColorPalette AlphaModulate = ColorPalette.FromHexBase("#ffffff");

}
