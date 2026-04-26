using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Client.Light;

/// <inheritdoc/>
public sealed class LightCycleSystem : SharedLightCycleSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_timing.IsFirstTimePredicted)
            return;

        // Log.Debug($"CurTime: {_timing.CurTime}, RoundStart: {_ticker.RoundStartTimeSpan}");

        var mapQuery = AllEntityQuery<LightCycleComponent, MapLightComponent>();
        while (mapQuery.MoveNext(out var uid, out var cycle, out var map))
        {
            if (!cycle.Running)
                continue;

            var time = GetTimeAbsolute((uid, cycle));

            // var color = GetColor((uid, cycle), cycle.OriginalColor, time);
            var color = CalculateColor(cycle, time);
            // Log.Debug(color.ToString());
            // Log.Debug($"B: {0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B}");
            map.AmbientLightColor = color;
        }
    }
}
