using Content.Server.Antag;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Petr.Components;
using Content.Server.Roles;
using Content.Server.RoundEnd;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Timing;
using Content.Shared.Cuffs.Components;
using Content.Shared.Roles.Components;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// Система для петров. Усё.
/// </summary>
public sealed class PetrRuleSystem : GameRuleSystem<PetrRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly EmergencyShuttleSystem _emergencyShuttle = default!;
    [Dependency] private readonly RoleSystem _role = default!;
    [Dependency] private readonly RoundEndSystem _roundEnd = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PetrTargetComponent, MobStateChangedEvent>(OnTargetMobStateChanged);

        SubscribeLocalEvent<PetrOperativeComponent, MobStateChangedEvent>(OnPetrMobStateChanged);

        SubscribeLocalEvent<PetrRoleComponent, GetBriefingEvent>(OnGetBriefing);

    }

    protected override void Started(EntityUid uid, PetrRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);
        component.CommandCheck = _timing.CurTime + component.TimerWait;
    }

    protected override void ActiveTick(EntityUid uid, PetrRuleComponent component, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);
        if (component.CommandCheck <= _timing.CurTime)
        {
            component.CommandCheck = _timing.CurTime + component.TimerWait;
            if (CheckCommandLose() || CheckPetrLose())
            {
                _roundEnd.EndRound();
                GameTicker.EndGameRule(uid, gameRule);
            }
        }
    }

    protected override void AppendRoundEndText(EntityUid uid,
        PetrRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        base.AppendRoundEndText(uid, component, gameRule, ref args);

        var petrLost = CheckPetrLose();
        var commandLost = CheckCommandLose();
        // Должно быть это взятие по индексу из структа outcomes. CommandLost- +1, petrLost- +2
        var index = (commandLost ? 1 : 0) | (petrLost ? 2 : 0);
        args.AddLine(Loc.GetString(Outcomes[index]));

        var sessionData = _antag.GetAntagIdentifiers(uid);
        args.AddLine(Loc.GetString("petr-count", ("initialCount", sessionData.Count)));
        foreach (var (mind, data, name) in sessionData)
        {
            _role.MindHasRole<PetrRoleComponent>(mind, out var role);

            args.AddLine(Loc.GetString("petr-name-user",
                ("name", name),
                ("username", data.UserName)));

        }
    }

    private void OnGetBriefing(EntityUid uid, PetrRoleComponent comp, ref GetBriefingEvent args)
    {
        var ent = args.Mind.Comp.OwnedEntity;
        var head = HasComp<PetrOperativeComponent>(ent);
        args.Append(Loc.GetString("petr-briefing"));
    }

    private void OnTargetMobStateChanged(EntityUid uid, PetrTargetComponent comp, MobStateChangedEvent ev)
    {
        if (ev.NewMobState == MobState.Dead || ev.NewMobState == MobState.Invalid)
            CheckCommandLose();
    }

    /// <summary>
    /// Checks if all of command is dead and if so will remove all sec and command jobs if there were any left.
    /// </summary>
    private bool CheckCommandLose()
    {
        var commandList = new List<EntityUid>();

        var heads = AllEntityQuery<PetrTargetComponent>();
        while (heads.MoveNext(out var id, out _))
        {
            commandList.Add(id);
        }
        return IsGroupDetainedOrDead(commandList, true, false);
    }

    private void OnPetrMobStateChanged(EntityUid uid, PetrOperativeComponent comp, MobStateChangedEvent ev)
    {
        if (ev.NewMobState == MobState.Dead || ev.NewMobState == MobState.Invalid)
            CheckPetrLose();
    }

    /// <summary>
    /// Чекаем дохлые/закованные ли петры.
    /// </summary>
    private bool CheckPetrLose()
    {
        var stunTime = TimeSpan.FromSeconds(4);
        var petrList = new List<EntityUid>();

        var petr = AllEntityQuery<PetrOperativeComponent, MobStateComponent>();
        while (petr.MoveNext(out var uid, out _, out _))
        {
            petrList.Add(uid);
        }
        return IsGroupDetainedOrDead(petrList, false, true);
    }

    /// <summary>
    /// Will take a group of entities and check if these entities are alive, dead or cuffed.
    /// </summary>
    /// <param name="list">The list of the entities</param>
    /// <param name="checkOffStation">Bool for if you want to check if someone is in space and consider them missing in action. (Won't check when emergency shuttle arrives just in case)</param>
    /// <param name="countCuffed">Bool for if you don't want to count cuffed entities.</param>
    /// <returns></returns>
    private bool IsGroupDetainedOrDead(List<EntityUid> list, bool checkOffStation, bool countCuffed)
    {
        var gone = 0;
        foreach (var entity in list)
        {
            if (TryComp<CuffableComponent>(entity, out var cuffed) && cuffed.CuffedHandCount > 0 && countCuffed)
            {
                gone++;
            }
            else
            {
                if (TryComp<MobStateComponent>(entity, out var state))
                {
                    if (state.CurrentState == MobState.Dead || state.CurrentState == MobState.Invalid)
                    {
                        gone++;
                    }
                    else if (checkOffStation && _stationSystem.GetOwningStation(entity) == null && !_emergencyShuttle.EmergencyShuttleArrived)
                    {
                        gone++;
                    }
                }
                //If they don't have the MobStateComponent they might as well be dead.
                else
                {
                    gone++;
                }
            }
        }

        return gone == list.Count || list.Count == 0;
    }

    private static readonly string[] Outcomes =
    {
        // Никто не умер
        "petr-reverse-stalemate",
        // Петры победили
        "petr-won",
        // Петры проиграли
        "petr-lost",
        // Все умерли
        "petr-stalemate"
    };
}
