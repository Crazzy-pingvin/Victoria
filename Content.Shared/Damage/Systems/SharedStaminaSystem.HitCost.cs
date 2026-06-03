
using Content.Shared.Damage.Components;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared.Damage.Systems;

public partial class SharedStaminaSystem
{
    private void InitializeCost()
    {
        SubscribeLocalEvent<StaminaHitCostComponent, MeleeHitEvent>(HandleMeleeAttack);
    }

    private void HandleMeleeAttack(EntityUid uid, StaminaHitCostComponent component, MeleeHitEvent args)
    {
        if (component.Cost <= 0)
            return;
        if (!TryComp<StaminaComponent>(args.User, out var stamina))
            return;
        TakeStaminaDamage(args.User, component.Cost, stamina, visual: false, ignoreResist: true);
    }

}
