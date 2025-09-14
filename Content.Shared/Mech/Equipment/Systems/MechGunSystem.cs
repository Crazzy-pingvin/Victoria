using Content.Shared.Mech.Components;
using Content.Shared.Mech.EntitySystems;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;

namespace Content.Shared.Mech.Equipment.EntitySystems;

/// <summary>
/// Handles <see cref="MechGunComponent"/> and all related UI logic
/// </summary>
public sealed class MechGunSystem : EntitySystem
{
    [Dependency] private SharedMechSystem _mech = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<MechGunComponent, MechSelectedEquipmentEvent>(OnSelected);
        SubscribeLocalEvent<MechGunComponent, MechUnselectedEquipmentEvent>(OnUnselected);

        SubscribeLocalEvent<MechGunComponent, MechEquipmentUiStateReadyEvent>(OnUiStateReady);
        SubscribeLocalEvent<MechGunComponent, AttemptShootEvent>(OnAttemptShoot);
        SubscribeLocalEvent<MechGunComponent, GetGunEvent>(OnGetGunEvent);
        SubscribeLocalEvent<MechGunComponent, GunShotEvent>(OnGunShot);
    }

    private void OnGunShot(EntityUid uid, MechGunComponent comp, ref GunShotEvent args)
    {
        if (!TryComp<MechEquipmentComponent>(uid, out var equip) || equip.EquipmentOwner == null)
            return;

        _mech.UpdateUserInterface(equip.EquipmentOwner.Value);
    }

    private void OnSelected(EntityUid equip, MechGunComponent comp, ref MechSelectedEquipmentEvent args)
    {
        EntityManager.AddComponents(args.Mech, comp.Comps);
    }

    private void OnUnselected(EntityUid equip, MechGunComponent comp, ref MechUnselectedEquipmentEvent args)
    {
        EntityManager.RemoveComponents(args.Mech, comp.Comps);
    }

    private void OnAttemptShoot(EntityUid uid, MechGunComponent component, ref AttemptShootEvent args)
    {
        if (!TryComp<MechEquipmentComponent>(uid, out var equipment) ||
            !TryComp<MechPilotComponent>(args.User, out var pilot) ||
            pilot.Mech != equipment.EquipmentOwner)
        {
            args.Cancelled = true;
        }
    }

    private void OnGetGunEvent(EntityUid uid, MechGunComponent component, ref GetGunEvent args)
    {
        if (args.Gun != null)
            return;

        if (!TryComp<MechEquipmentComponent>(uid, out var equip) || equip.EquipmentOwner == null)
            return;

        args.Gun = equip.EquipmentOwner;
    }

    private void OnUiStateReady(EntityUid uid, MechGunComponent component, MechEquipmentUiStateReadyEvent args)
    {
        if (!TryComp<MechEquipmentComponent>(uid, out var equip) || equip.EquipmentOwner == null)
            return;

        var ev = new GetAmmoCountEvent();
        RaiseLocalEvent(uid, ref ev);

        var state = new MechGunUiState
        {
            Capacity = ev.Capacity,
            Count = ev.Count,
        };

        args.States.Add(GetNetEntity(uid), state);
    }
}
