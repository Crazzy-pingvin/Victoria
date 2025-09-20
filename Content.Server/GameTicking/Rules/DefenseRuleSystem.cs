using Content.Server.Administration.Logs;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.RoundEnd;
using Content.Server.Defense;
using Content.Shared.Database;
using Content.Shared.GameTicking.Components;
using Content.Shared.Mobs;
using Content.Shared.Destructible;
using Content.Shared.RandomChangeTime;
using Robust.Shared.Timing;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// Система для защиты
/// </summary>
public sealed class DefenseRuleSystem : GameRuleSystem<DefenseRuleComponent>
{
    [Dependency] private readonly IAdminLogManager _adminLog = default!;
    [Dependency] private readonly RoundEndSystem _roundEnd = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private int _startDefenseNodes = 0;
    private int _defenseNodes = 0;
    private bool _end = false;
    private int _enemyKilled = 0;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DefenseTargetComponent, DestructionEventArgs>(OnTargetDestroyed);

        SubscribeLocalEvent<DefenseEnemyComponent, MobStateChangedEvent>(OnEnemyDeath);

        //SubscribeLocalEvent<PetrRoleComponent, GetBriefingEvent>(OnGetBriefing);

    }

    protected override void ActiveTick(EntityUid uid, DefenseRuleComponent component, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);
        if (component.NextCheck <= _timing.CurTime)
        {
            component.NextCheck = _timing.CurTime + component.TimerWait;
            if (_end)
            {
                _roundEnd.EndRound();
                GameTicker.EndGameRule(uid, gameRule);
            }
        }
    }
    protected override void Started(EntityUid uid, DefenseRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);
        _adminLog.Add(LogType.Action, LogImpact.Extreme, $"СТАРТУЕМ!!!");
        component.NextCheck = _timing.CurTime + component.TimerWait;
        var targets = AllEntityQuery<DefenseSettingsComponent>();
        while (targets.MoveNext(out var id, out var comp))
        {
            _startDefenseNodes = comp.StartNodes;
        }
        _adminLog.Add(LogType.Action, LogImpact.Extreme, $"Кол-во узлов обороны- {_startDefenseNodes}");
        _defenseNodes = _startDefenseNodes;
    }

    protected override void AppendRoundEndText(EntityUid uid,
        DefenseRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        base.AppendRoundEndText(uid, component, gameRule, ref args);
        args.AddLine($"Осталось {_defenseNodes} узлов из {_startDefenseNodes}.");
        args.AddLine(Loc.GetString(DetermineOutcomes()));
        args.AddLine($"Было уничтожено {_enemyKilled} ксеносов.");
        _end = false;
        _enemyKilled = 0;

    }
    private void OnTargetDestroyed(EntityUid uid, DefenseTargetComponent comp, DestructionEventArgs args)
    {
        _defenseNodes -= 1;
        _adminLog.Add(LogType.Action, LogImpact.Extreme, $"Защита потеряла узел обороны! Осталось {_defenseNodes} из {_startDefenseNodes}");
        if (comp.Flag)
            _end = true;
    }
    private void OnEnemyDeath(EntityUid uid, DefenseEnemyComponent comp, MobStateChangedEvent ev)
    {
        if (ev.NewMobState == MobState.Dead || ev.NewMobState == MobState.Critical)
        {
            _enemyKilled += 1;
            if (comp.Boss)
                _end = true;
            if (TryComp<RandomChangeTimeComponent>(uid, out var _))
                RemComp<RandomChangeTimeComponent>(uid);
        }
    }
    private string DetermineOutcomes()
    {
        if (_defenseNodes == 0)
            return Outcomes[0];
        if (_defenseNodes == 1)
            return Outcomes[1];
        float percent = _defenseNodes / _startDefenseNodes;
        if (percent <= 0.25)
            return Outcomes[2];
        if (percent <= 0.5)
            return Outcomes[3];
        if (percent <= 0.75)
            return Outcomes[4];
        else
            return Outcomes[5];
    }


    private static readonly string[] Outcomes =
    {
        //Потеряно всё
        "def-remain-nothing",
        // Остался только флаг
        "def-remain-flag",
        //Осталась четверть
        "def-remain-25",
        //Осталась половина
        "def-remain-50",
        //Потеряна четверть
        "def-remain-75",
        //Ничего не потеряно
        "def-remain-100",
    };
}
