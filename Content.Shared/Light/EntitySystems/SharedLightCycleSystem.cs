using Content.Shared.GameTicking;
using Content.Shared.Light.Components;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Light.EntitySystems;

using DayTimeSpan = (TimeSpan current, TimeSpan dayDuration);
public abstract class SharedLightCycleSystem : EntitySystem
{
    [Dependency] private readonly SharedGameTicker _ticker = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] protected readonly MetaDataSystem Metadata = default!;


    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LightCycleComponent, MapInitEvent>(OnCycleMapInit);
        SubscribeLocalEvent<LightCycleComponent, ComponentShutdown>(OnCycleShutdown);
    }

    protected virtual void OnCycleMapInit(Entity<LightCycleComponent> ent, ref MapInitEvent args)
    {
        if (TryComp(ent.Owner, out MapLightComponent? mapLight))
        {
            ent.Comp.OriginalColor = mapLight.AmbientLightColor;
            Dirty(ent);
        }
    }

    private void OnCycleShutdown(Entity<LightCycleComponent> ent, ref ComponentShutdown args)
    {
        if (TryComp(ent.Owner, out MapLightComponent? mapLight))
        {
            mapLight.AmbientLightColor = ent.Comp.OriginalColor;
            Dirty(ent.Owner, mapLight);
        }
    }
    public float GetTimeAbsolute(Entity<LightCycleComponent> cycle)
    {
        var pausedTime = Metadata.GetPauseTime(cycle.Owner);

        return (float)_timing.CurTime
            .Add(cycle.Comp.Offset)
            .Subtract(_ticker.RoundStartTimeSpan)
            .Subtract(pausedTime)
            .TotalSeconds;
    }

    public DayTimeSpan GetDayTimeSpan(Entity<LightCycleComponent> cycle)
    {
        var time = GetTimeAbsolute(cycle);
        var duration = cycle.Comp.Duration;
        return (TimeSpan.FromSeconds(time % (float)duration.TotalSeconds), duration);
    }
    public void SetOffset(Entity<LightCycleComponent> entity, TimeSpan offset)
    {
        entity.Comp.Offset = offset;
        var ev = new LightCycleOffsetEvent(offset);

        RaiseLocalEvent(entity, ref ev);
        Dirty(entity);
    }
    public void SkipTime(Entity<LightCycleComponent> ent, TimeSpan time)
    {
        SetOffset(ent, ent.Comp.Offset.Add(time));
    }

    private static float SmootherStep(
            float x,
            float waveLength,
            float minLightLevel,
            float maxLightLevel,
            float exponent,
            float sunriseStart,
            float sunriseEnd,
            float sunsetStart,
            float sunsetEnd)
    {
        var t = (x % waveLength) / waveLength;
        var riseCoeff = LinearInterpolation(sunriseStart, sunriseEnd, t);
        var setCoeff = 1.0f - LinearInterpolation(sunsetStart, sunsetEnd, t);
        var factor = MathF.Min(riseCoeff, setCoeff);
        var curve = MathF.Pow(factor, exponent);
        return minLightLevel + (maxLightLevel - minLightLevel) * curve;
    }

    private static float LinearInterpolation(float start, float end, float value)
    {
        return Math.Clamp((value - start) / (end - start), 0f, 1f);
    }

    public static Color CalculateColor(LightCycleComponent comp, float time)
    {
        float GetCoeff(TimeSpan t) => GetTimeCoeff(t, comp.Duration);

        var waveLength = MathF.Max(1f, (float)comp.Duration.TotalSeconds);
        var riseSCoeff = GetCoeff(comp.SunriseStartTime);
        var riseECoeff = GetCoeff(comp.SunriseEndTime);
        var setSCoeff = GetCoeff(comp.SunsetStartTime);
        var setECoeff = GetCoeff(comp.SunsetEndTime);

        float GetChannel(float minChannel, float maxChannel, float exponent) => SmootherStep(time, waveLength, minChannel, maxChannel, exponent, riseSCoeff, riseECoeff, setSCoeff, setECoeff);

        var r = GetChannel(comp.MinLevel.R, comp.OriginalColor.R, 4f);
        var g = GetChannel(comp.MinLevel.G, comp.OriginalColor.G, 10f);
        var b = GetChannel(comp.MinLevel.B, comp.OriginalColor.B, 2f);

        return new Color(r, g, b);
    }

    private static float GetTimeCoeff(TimeSpan time, TimeSpan duration)
    {
        return (float)(time.TotalSeconds / duration.TotalSeconds);
    }
}

// TODO: Сделать команду, которая пропускает время к определённой стадии дня
public enum TimeOfDay
{
    Morning,
    Noon,
    Night,
    Midnight,
}

/// <summary>
/// Raised when the offset on <see cref="LightCycleComponent"/> changes.
/// </summary>
[ByRefEvent]
public record struct LightCycleOffsetEvent(TimeSpan Offset)
{
    public readonly TimeSpan Offset = Offset;
}
