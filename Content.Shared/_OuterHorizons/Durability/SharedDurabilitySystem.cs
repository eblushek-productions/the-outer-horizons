using Content.Shared.Weapons.Melee.Events;
using Content.Shared.PowerCell;
using Content.Shared.Tools.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Power.Components;

namespace Content.Shared._OuterHorizons.Durability;

public abstract partial class SharedDurabilitySystem : EntitySystem
{
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly SharedBatterySystem _battery = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<DurabilityComponent, MeleeHitEvent>(OnMeleeHit);
        SubscribeLocalEvent<DurabilityComponent, AttemptMeleeEvent>(OnMeleeAttempt);
        SubscribeLocalEvent<DurabilityComponent, ToolUseAttemptEvent>(OnToolAttempt);
    }
    private void OnMeleeHit(Entity<DurabilityComponent> ent, ref MeleeHitEvent args)
    {
        if (args.HitEntities.Count == 0)
            return;

        DurabilityUse(ent.Owner, args.User, ent.Comp);
    }

    private void OnMeleeAttempt(Entity<DurabilityComponent> ent, ref AttemptMeleeEvent args)
    {
        if (!CanUseItem(ent.Owner, args.User, ent.Comp))
            args.Cancelled = true;
    }

    private void OnToolAttempt(Entity<DurabilityComponent> ent, ref ToolUseAttemptEvent args)
    {
        if (!CanUseItem(ent.Owner, args.User, ent.Comp))
            args.Cancel();
    }

    public void DurabilityUse(EntityUid uid, EntityUid user, DurabilityComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (component.UseEnergy)
        {
            if (!_powerCell.TryGetBatteryFromSlot(uid, out var battery))
                return;

            _battery.UseCharge(battery.Value.AsNullable(), component.ChargePerUse);
        } else
        {
            component.Damage++;
            if (component.Damage >= component.MaxDurability)
                Break(uid, user, component);
        }
    }

    protected virtual void Break(EntityUid uid, EntityUid user, DurabilityComponent component)
    {
        //На серверной части
    }

    public bool CanUseItem(EntityUid uid, EntityUid user, DurabilityComponent component)
    {
        if (!component.UseEnergy)
            return true;

        if (!_powerCell.HasCharge(uid, 1))
        {
            return false;
        }

        return true;
    }
}
