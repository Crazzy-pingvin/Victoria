using Content.Shared.Addictions;
using Content.Shared.Addictions.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Content.Shared.StatusEffect;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityEffects.Effects.StatusEffects;

/// <summary>
///     Adds a generic status effect to the entity,
///     not worrying about things like how to affect the time it lasts for
///     or component fields or anything. Just adds a component to an entity
///     for a given time. Easy.
/// </summary>
/// <remarks>
///     Can be used for things like adding accents or something. I don't know. Go wild.
/// </remarks>
[UsedImplicitly]
public sealed partial class AddictionEffect : EntityEffect
{
    [DataField(required: true)]
    public string Addiction;

    [DataField]
    public bool AddAddiction = true;

    [DataField]
    public float satiationEffect = 0.01f;
    [DataField]
    public float withdrawlEffect = -0.01f;
    [DataField]
    public float cureEffect = -0.1f;

    public override void Effect(EntityEffectBaseArgs args)
    {
        var addicSys = args.EntityManager.EntitySysManager.GetEntitySystem<AddictionSystem>();
        addicSys.HandleEffect(new AddictionId(Addiction),args.TargetEntity,
        AddAddiction, satiationEffect,
        withdrawlEffect, cureEffect);
    }

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) => Loc.GetString("123");
}
