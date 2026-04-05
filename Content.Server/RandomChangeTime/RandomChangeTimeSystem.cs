using Robust.Shared.Random;
using Content.Shared.RandomChangeTime;

namespace Content.Server.RandomChangeTime;

public sealed class RandomChangeTimeSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<RandomChangeTimeComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            comp.NextCheckTime += frameTime;
            if (comp.NextCheckTime <= comp.Time)
                continue;

            var random = _random.NextFloat();
            if (comp.Entity is null || random >= comp.Prob)
            {
                RemComp<RandomChangeTimeComponent>(uid);
                continue;
            }
            if (!TryComp<TransformComponent>(uid, out var xform))
                continue;
            Spawn(comp.Entity, xform.Coordinates);
            Del(uid);
        }
    }
}
