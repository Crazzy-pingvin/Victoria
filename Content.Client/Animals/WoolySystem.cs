using Content.Client.Popups;
using Content.Shared.Animals;

namespace Content.Client.Animals;

public sealed partial class WoolySystem : SharedWoolySystem
{
    [Dependency] private readonly PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WoolyComponent, WoolShaveDoAfterEvent>(OnWoolShaveDoAfterEvent);
    }

    private void OnWoolShaveDoAfterEvent(EntityUid mob, WoolyComponent comp, ref WoolShaveDoAfterEvent args)
    {
        if (comp.CurrentState == WoolyState.Naked)
            return;

        if (!comp.WoolyQuantity.TryGetValue(comp.CurrentState, out var quantity))
            return;

        _popup.PopupClient(Loc.GetString("wooly-system-success", ("amount", quantity), ("target", MetaData(mob).EntityName)), args.User);
    }
}
