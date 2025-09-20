using Content.Shared.GameTicking;
using Content.Shared.Spawners.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Spawners;

public sealed class StateChoiceSpawnerSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private string? _currentState;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
        SubscribeLocalEvent<StateChoiceSpawnerComponent, MapInitEvent>(OnMapInit);
    }

    private void OnRoundRestart(RoundRestartCleanupEvent args)
    {
        _currentState = _random.Pick(new List<string> { "umpor", "mirt", "cdc", "nt", "ussp" });
    }

    private void OnMapInit(EntityUid uid, StateChoiceSpawnerComponent component, MapInitEvent args)
    {
        if (_currentState == null || _timing.InPrediction)
            return;

        var proto = _currentState switch
        {
            "umpor" => component.Umpor,
            "mirt" => component.Mirt,
            "cdc" => component.Cdc,
            "nt" => component.Nt,
            "ussp" => component.Ussp,
            _ => null
        };

        if (proto == null || !_prototype.HasIndex<EntityPrototype>(proto))
            return;

        var coords = Transform(uid).Coordinates;
        Del(uid);
        Spawn(proto, coords);
    }
}
