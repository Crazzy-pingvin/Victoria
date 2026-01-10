using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;


namespace Content.Shared.Tranquil;

public abstract class SharedTranquilSystem : EntitySystem
{
    [ValidatePrototypeId<StatusEffectPrototype>]
    public const string TranqKey = "Tranquil";

    [Dependency] private readonly StatusEffectsSystem _statusEffectsSystem = default!;

    public void TryApplyTranquilness(EntityUid uid, float tranqPower,
        StatusEffectsComponent? status = null)
    {
        if (!Resolve(uid, ref status, false))
            return;

        if (!_statusEffectsSystem.HasStatusEffect(uid, TranqKey, status))
        {
            _statusEffectsSystem.TryAddStatusEffect<TranquilComponent>(uid, TranqKey, TimeSpan.FromSeconds(tranqPower), false, status);
        }
        else
        {
            _statusEffectsSystem.TryAddTime(uid, TranqKey, TimeSpan.FromSeconds(tranqPower), status);
        }
    }

    public void TryRemoveTranquilness(EntityUid uid)
    {
        _statusEffectsSystem.TryRemoveStatusEffect(uid, TranqKey);
    }
    public void TryRemoveTranquilnessTime(EntityUid uid, double timeRemoved)
    {
        _statusEffectsSystem.TryRemoveTime(uid, TranqKey, TimeSpan.FromSeconds(timeRemoved));
    }

}
