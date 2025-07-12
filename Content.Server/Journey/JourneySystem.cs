using Content.Server.NPC;
using Content.Server.NPC.Systems;
using Content.Shared.Database;
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

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var npc_query = EntityQueryEnumerator<JourneyComponent>();
        while (npc_query.MoveNext(out var uid, out var comp))
        {
            if (_timing.CurTime < comp.NextCheck)
                continue;
            comp.NextCheck = _timing.CurTime + TimeSpan.FromSeconds(1);

            var targets = EntityQueryEnumerator<JourneyTargetComponent>();
            JourneyTargetComponent fav_target = new JourneyTargetComponent();
            fav_target.Priority = -999;

            while (targets.MoveNext(out var targ, out var comp_targ))
            {
                if (comp_targ.Priority > fav_target.Priority && (comp.JourneyGroup == comp_targ.JourneyGroup || comp_targ.IgnoreGroups))
                    fav_target = comp_targ;
            }
            _npc.SetBlackboard(uid, NPCBlackboard.FollowTarget, new EntityCoordinates(fav_target.Owner, Vector2.Zero));
        }
    }
}
