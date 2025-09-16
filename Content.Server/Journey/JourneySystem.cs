using Content.Server.NPC;
using Content.Server.NPC.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using System.Numerics;
namespace Content.Server.Journey;

/// <summary>
/// Кждую секунду заставляет мобов следовать за объектами с компонентом, учитывая приоритет и группы.
/// </summary>
public sealed class JourneySystem : EntitySystem
{
    [Dependency] private readonly NPCSystem _npc = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var npc_query = EntityQueryEnumerator<JourneyComponent>();
        while (npc_query.MoveNext(out var uid, out var comp_seek))
        {

            if (_timing.CurTime < comp_seek.NextCheck)
                continue;
            _adminLogger.Add(LogType.Action, LogImpact.Extreme, $"{ToPrettyString(uid)} ищет путь...");
            comp_seek.NextCheck = _timing.CurTime + TimeSpan.FromSeconds(3);

            var targets = EntityQueryEnumerator<JourneyTargetComponent>();
            _adminLogger.Add(LogType.Action, LogImpact.Extreme, $"'сырые' цели: {targets}");
            var targets_approved = new List<JourneyTargetComponent>();
            JourneyTargetComponent fav_target = new JourneyTargetComponent();

            fav_target.Priority = -999;

            while (targets.MoveNext(out var targ, out var comp_targ))
            {
                if (_transform.InRange(uid, targ, comp_targ.MaxRange))
                    targets_approved.Add(comp_targ);
            };

            _adminLogger.Add(LogType.Action, LogImpact.Extreme, $"фильтрованные цели: {targets_approved}");


            foreach (JourneyTargetComponent cycle_targ in targets_approved)
            {
                if (cycle_targ.Priority > fav_target.Priority && (comp_seek.JourneyGroup == cycle_targ.JourneyGroup || cycle_targ.IgnoreGroups))
                    fav_target = cycle_targ;
            }

            if (fav_target.Priority == -999)
            {
                _adminLogger.Add(LogType.Action, LogImpact.Extreme, $"Цель не найдена. Увынск.");
                return;
            }
            ;

            _npc.SetBlackboard(uid, NPCBlackboard.FollowTarget, new EntityCoordinates(fav_target.Owner, Vector2.Zero));
            _adminLogger.Add(LogType.Action, LogImpact.Extreme, $"цель найдена: {ToPrettyString( fav_target.Owner )}");
        }
    }
}
