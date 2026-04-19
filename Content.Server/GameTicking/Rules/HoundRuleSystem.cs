using Content.Server.Antag;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Roles;
using Content.Shared.Roles.Components;
using Content.Shared.Humanoid;

namespace Content.Server.GameTicking.Rules;

public sealed class HoundRuleSystem : GameRuleSystem<HoundRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HoundRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagSelected);

        SubscribeLocalEvent<HoundRoleComponent, GetBriefingEvent>(OnGetBriefing);
    }

    // Greeting upon Hound activation
    private void AfterAntagSelected(Entity<HoundRuleComponent> mindId, ref AfterAntagEntitySelectedEvent args)
    {
        var ent = args.EntityUid;
        _antag.SendBriefing(ent, MakeBriefing(ent), null, null);
    }

    // Character screen briefing
    private void OnGetBriefing(Entity<HoundRoleComponent> role, ref GetBriefingEvent args)
    {
        var ent = args.Mind.Comp.OwnedEntity;

        if (ent is null)
            return;
        args.Append(MakeBriefing(ent.Value));
    }

    private string MakeBriefing(EntityUid ent)
    {
        var isHuman = HasComp<HumanoidAppearanceComponent>(ent);
        var briefing = isHuman
            ? Loc.GetString("hound-role-greeting-human")
            : Loc.GetString("hound-role-greeting-animal");

        if (isHuman)
            briefing += "\n \n" + Loc.GetString("hound-role-greeting-equipment") + "\n";

        return briefing;
    }
}
