using Content.Shared._OuterHorizons.Durability;
using Content.Shared.Examine;
using Robust.Shared.Utility;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.Audio.Systems;

namespace Content.Server._OuterHorizons.Durability;

public sealed class DurabilitySystem : SharedDurabilitySystem
{
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DurabilityComponent, ExaminedEvent>(OnExamine);
    }

    private void OnExamine(Entity<DurabilityComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.UseEnergy)
            return;

        if (ent.Comp.MaxDurability == 0)
            return;
        var damagePercent = (int)Math.Round(((float)ent.Comp.Damage / (float)ent.Comp.MaxDurability * 4f));

        var state = Loc.GetString("durability-component-damage-" + damagePercent);

        var message = new FormattedMessage();
        message.AddMarkupPermissive(Loc.GetString("durability-component-damage-message", ("state", state)));

        args.PushMessage(message);
    }

    protected override void Break(EntityUid uid, EntityUid user, DurabilityComponent component)
    {
        base.Break(uid, user, component);

        var cords = Transform(user).Coordinates;

        _audio.PlayPvs(component.BreakSound, cords);
        QueueDel(uid);

        if (component.ReplaceOnBreak == null)
            return;

        if (_hands.GetActiveHand(user) is not { } activeHand)
            return;

        var replaceItem = Spawn(component.ReplaceOnBreak, cords);
        _hands.TryForcePickup(user, replaceItem, activeHand, checkActionBlocker: false);
    }
}
