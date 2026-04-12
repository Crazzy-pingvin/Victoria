using Content.Shared.Damage.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public sealed class DamageContactsSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelistSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DamageContactsComponent, StartCollideEvent>(OnEntityEnter);
        SubscribeLocalEvent<DamageContactsComponent, EndCollideEvent>(OnEntityExit);
        // Victoria-DamageContactsOnMove-Start
        SubscribeLocalEvent<DamageContactsComponent, MoveInputEvent>(OnMoveInput);
        // Victoria-DamageContactsOnMove-End
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<DamagedByContactComponent>();

        while (query.MoveNext(out var ent, out var damaged))
        {
            if (_timing.CurTime < damaged.NextSecond)
                continue;
            damaged.NextSecond = _timing.CurTime + TimeSpan.FromSeconds(1);

            if (damaged.Damage != null)
                _damageable.TryChangeDamage(ent, damaged.Damage, interruptsDoAfters: false);
        }
    }

    private void OnEntityExit(EntityUid uid, DamageContactsComponent component, ref EndCollideEvent args)
    {
        var otherUid = args.OtherEntity;

        if (!TryComp<PhysicsComponent>(otherUid, out var body))
            return;

        var damageQuery = GetEntityQuery<DamageContactsComponent>();
        foreach (var ent in _physics.GetContactingEntities(otherUid, body))
        {
            if (ent == uid)
                continue;

            if (damageQuery.HasComponent(ent))
                return;
        }

        RemComp<DamagedByContactComponent>(otherUid);
    }

    private void OnEntityEnter(EntityUid uid, DamageContactsComponent component, ref StartCollideEvent args)
    {
        var otherUid = args.OtherEntity;

        // Victoria-DamageContactsOnMove-Start
        if (component.OnMove)
            return;
        // Victoria-DamageContactsOnMove-End

        if (HasComp<DamagedByContactComponent>(otherUid))
            return;

        if (_whitelistSystem.IsWhitelistPass(component.IgnoreWhitelist, otherUid))
            return;

        var damagedByContact = EnsureComp<DamagedByContactComponent>(otherUid);
        damagedByContact.Damage = component.Damage;
    }

    // Victoria-DamageContactsOnMove-Start
    private void OnMoveInput(EntityUid uid, DamageContactsComponent comp, ref MoveInputEvent ev)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        if (!TryComp<PhysicsComponent>(uid, out var body))
            return;

        foreach (var ent in _physics.GetContactingEntities(uid, body))
        {
            if (ent == uid)
                continue;

            if (ev.HasDirectionalMovement)
            {
                if (HasComp<DamagedByContactComponent>(ent))
                    continue;

                if (_whitelistSystem.IsWhitelistPass(comp.IgnoreWhitelist, ent))
                    continue;

                var damagedByContact = EnsureComp<DamagedByContactComponent>(ent);
                damagedByContact.Damage = comp.Damage;
            }
            else
            {
                if (HasComp<DamagedByContactComponent>(ent))
                    RemComp<DamagedByContactComponent>(ent);
            }
        }
    }
    // Victoria-DamageContactsOnMove-End
}
