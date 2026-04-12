using Content.Shared.Tranquil;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityEffects.Effects;

public sealed partial class Tranquil : EntityEffect
{
    /// <summary>
    ///     BoozePower is how long each metabolism cycle will make the tranquil effect last for.
    /// </summary>
    [DataField]
    public float TranqPower = 3f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-tranquil", ("chance", Probability));

    public override void Effect(EntityEffectBaseArgs args)
    {
        var tranqPower = TranqPower;

        if (args is EntityEffectReagentArgs reagentArgs) {
            tranqPower *= reagentArgs.Scale.Float();
        }

        var tranqSys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedTranquilSystem>();
        tranqSys.TryApplyTranquilness(args.TargetEntity, tranqPower);
    }
}
