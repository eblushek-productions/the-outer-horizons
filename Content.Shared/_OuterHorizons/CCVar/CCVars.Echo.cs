using Content.Shared.Administration;
using Content.Shared.CCVar.CVarAccess;
using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Enable echo effect for grids.
    /// </summary>
    public static readonly CVarDef<bool> EchoEnabled =
        CVarDef.Create("echo.echo_enabled", true, CVar.REPLICATED | CVar.SERVER);
}
