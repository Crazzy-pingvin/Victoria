using Content.Shared.Tranquil;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;


namespace Content.Server.Tranquil;

public sealed class TranquilSystem : SharedTranquilSystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    private ProtoId<DamageModifierSetPrototype> ProtectionModifierSetId = "Tranquil";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TranquilComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<TranquilComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnInit(EntityUid uid, TranquilComponent component, ComponentInit args)
    {
        if (!_prototypeManager.TryIndex(ProtectionModifierSetId, out var modifier))
            return;
        var buffComp = EnsureComp<DamageProtectionBuffComponent>(uid);
        // add the damage modifier if it isn't in the dict yet
        if (!buffComp.Modifiers.ContainsKey(ProtectionModifierSetId))
            buffComp.Modifiers.Add(ProtectionModifierSetId, modifier);
    }

    private void OnShutdown(EntityUid uid, TranquilComponent component, ComponentShutdown args)
    {
        if (!TryComp<DamageProtectionBuffComponent>(uid, out var buffComp))
            return;
        // remove the damage modifier from the dict
        buffComp.Modifiers.Remove(ProtectionModifierSetId);
        // if the dict is empty now, remove the buff component
        if (buffComp.Modifiers.Count == 0)
            RemComp<DamageProtectionBuffComponent>(uid);
    }
}
