using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;


namespace Content.Shared.Energized;

public abstract class SharedEnergizedSystem : EntitySystem
{
    [ValidatePrototypeId<StatusEffectPrototype>]
    public const string EnergyKey = "Energized";

    [Dependency] private readonly StatusEffectsSystem _statusEffectsSystem = default!;

    public void TryApplyEnergizedness(EntityUid uid, float energizePower,
        StatusEffectsComponent? status = null)
    {
        if (!Resolve(uid, ref status, false))
            return;

        if (!_statusEffectsSystem.HasStatusEffect(uid, EnergyKey, status))
        {
            _statusEffectsSystem.TryAddStatusEffect<EnergizedComponent>(uid, EnergyKey, TimeSpan.FromSeconds(energizePower), false, status);
        }
        else
        {
            _statusEffectsSystem.TryAddTime(uid, EnergyKey, TimeSpan.FromSeconds(energizePower), status);
        }
    }

    public void TryRemoveEnergizedness(EntityUid uid)
    {
        _statusEffectsSystem.TryRemoveStatusEffect(uid, EnergyKey);
    }
    public void TryRemoveEnergizednessTime(EntityUid uid, double timeRemoved)
    {
        _statusEffectsSystem.TryRemoveTime(uid, EnergyKey, TimeSpan.FromSeconds(timeRemoved));
    }

}
