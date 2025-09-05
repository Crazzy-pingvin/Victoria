using Content.Server.Animals.Systems;
using Content.Server.DoAfter;
using Content.Server.Hands.Systems;
using Content.Server.Kitchen.Components;
using Content.Server.Stack;
using Content.Shared.Animals;
using Content.Shared.DoAfter;
using Content.Shared.Verbs;

namespace Content.Server.Animals;

public sealed partial class WoolySystem : SharedWoolySystem
{
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly StackSystem _stack = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WoolyComponent, GetVerbsEvent<AlternativeVerb>>(AddShaveVerb);
        SubscribeLocalEvent<WoolyComponent, WoolShaveDoAfterEvent>(OnWoolShaveDoAfterEvent);
    }

    private void AddShaveVerb(Entity<WoolyComponent> entity, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (args.Using == null ||
             !args.CanInteract ||
             entity.Comp.CurrentState == WoolyState.Naked)
            return;

        var uid = entity.Owner;
        var user = args.User;
        var used = args.Using.Value;

        AlternativeVerb verb = new()
        {
            Act = () =>
            {
                AttemptShave(uid, user, used);
            },
            Text = Loc.GetString("wooly-system-verb-shave"),
            Priority = 2
        };
        args.Verbs.Add(verb);
    }

    private void OnWoolShaveDoAfterEvent(EntityUid mob, WoolyComponent comp, ref WoolShaveDoAfterEvent args)
    {
        if (comp.CurrentState == WoolyState.Naked)
            return;

        if (!comp.WoolyQuantity.TryGetValue(comp.CurrentState, out var quantity))
            return;

        _stack.SpawnMultiple(comp.WoolEntity, quantity, mob);

        SetState(mob, comp.CurrentState - 1);
    }

    private void AttemptShave(EntityUid mob, EntityUid user, EntityUid used)
    {
        if (!HasComp<SharpComponent>(used))
            return;

        var doargs = new DoAfterArgs(EntityManager, user, 5, new WoolShaveDoAfterEvent(), mob, mob, used)
        {
            BreakOnDamage = true,
            BreakOnHandChange = true,
            BlockDuplicate = true,
            MovementThreshold = 1.0f,
        };

        _doAfter.TryStartDoAfter(doargs);
    }
}

