using System.Linq;
using Content.Shared._OuterHorizons.ShuttleConsole;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._OuterHorizons.ShuttleConsole;

public sealed class ThrusterInfoScreen : ScrollContainer
{
    private readonly Dictionary<NetEntity, ThrusterInfoEntry> _thrusterInfoEntries = [];
    private readonly BoxContainer _boxContainer = new()
    {
        Orientation = BoxContainer.LayoutOrientation.Vertical,
        Margin = new Thickness(10),
        HorizontalExpand = true,
        SeparationOverride = 15,
    };

    public ThrusterInfoScreen()
    {
        AddChild(_boxContainer);

        HorizontalExpand = true;
    }

    public void SetState(ThrusterInfoInterfaceState state)
    {
        var netUids = state.ThrusterData.ToDictionary(a => a.ThrusterEntity);
        var addedUids = new HashSet<NetEntity>();

        foreach (var (netUid, entry) in _thrusterInfoEntries)
        {
            if (netUids.TryGetValue(netUid, out var data))
            {
                entry.SetState(data);
                addedUids.Add(netUid);
                continue;
            }

            _boxContainer.RemoveChild(entry);
            _thrusterInfoEntries.Remove(netUid);
        }

        foreach (var data in state.ThrusterData)
        {
            if(addedUids.Contains(data.ThrusterEntity))
                continue;

            var entry = new ThrusterInfoEntry();
            entry.SetState(data);

            _boxContainer.AddChild(entry);
            _thrusterInfoEntries.Add(data.ThrusterEntity, entry);
        }
    }
}
