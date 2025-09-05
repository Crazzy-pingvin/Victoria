using Content.Shared.Mobs.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Animals;

/// <summary>
///     Gives ability to produce fiber reagents;
///     produces endlessly if the owner has no HungerComponent.
/// </summary>
public abstract class SharedWoolySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WoolyComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<WoolyComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<WoolyComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnMapInit(EntityUid uid, WoolyComponent component, MapInitEvent args)
    {
        component.NextGrowth = _timing.CurTime + component.GrowthDelay;
    }

    private void OnStartup(EntityUid uid, WoolyComponent component, ref ComponentStartup args)
    {
        _appearance.SetData(uid, WoolyVisualState.State, component.CurrentState);
    }

    private void OnShutdown(EntityUid uid, WoolyComponent component, ref ComponentShutdown args)
    {
        _appearance.RemoveData(uid, WoolyVisualState.State);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<WoolyComponent>();
        while (query.MoveNext(out var uid, out var wooly))
        {
            if (_timing.CurTime < wooly.NextGrowth)
                continue;

            if (_mobState.IsDead(uid))
                continue;

            wooly.NextGrowth += wooly.GrowthDelay;

            if (wooly.CurrentState != WoolyState.Long)
            {
                SetState(uid, wooly.CurrentState + 1);
            }
        }
    }

    public void SetState(EntityUid mob, WoolyState state)
    {
        if (!TryComp<WoolyComponent>(mob, out var wooly))
            return;

        wooly.CurrentState = state;
        _appearance.SetData(mob, WoolyVisualState.State, wooly.CurrentState);
    }
}

[Serializable, NetSerializable]
public enum WoolyVisualState
{
    State
}

[Serializable, NetSerializable]
public enum WoolyVisualLayers
{
    Base
}
