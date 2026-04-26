using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.Timing;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio.Systems;

namespace Content.Shared.Victoria_Stronghold.Botany;

public sealed class CompostBinSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    private readonly List<EntityUid> _deleteQueue = [];

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CompostBinComponent, AfterInteractUsingEvent>(OnInteract);
        SubscribeLocalEvent<CompostBinComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<CompostBinComponent, EntRemovedFromContainerMessage>(OnEntRemoved);
    }
    private void OnMapInit(Entity<CompostBinComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextRegenTime = _timing.CurTime + ent.Comp.Interval;
        Dirty(ent);
    }
    private void OnEntRemoved(Entity<CompostBinComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        if (args.Entity == ent.Comp.SolutionOutRef?.Owner)
            ent.Comp.SolutionOutRef = null;
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        DeleteUsed();

        var query = EntityQueryEnumerator<CompostBinComponent, SolutionContainerManagerComponent>();
        while (query.MoveNext(out var uid, out var compost, out var manager))
        {
            if (_timing.CurTime < compost.NextRegenTime)
                continue;

            compost.NextRegenTime += compost.Interval;
            Dirty(uid, compost);
            if (!_solutionSystem.ResolveSolution((uid, manager), compost.SolutionOutputId, ref compost.SolutionOutRef))
                continue;

            var convertAmount = FixedPoint2.Min(compost.CompostConvertRate, compost.BufferVolume);
            if (convertAmount <= FixedPoint2.Zero)
                continue;

            compost.BufferVolume -= convertAmount;
            Solution toAdd = new(compost.ReagentOutputId, convertAmount);
            _solutionSystem.TryAddSolution(compost.SolutionOutRef.Value, toAdd);
        }
    }

    private void DeleteUsed()
    {
        _deleteQueue.RemoveAll(ent =>
        {
            PredictedQueueDel(ent);
            return true;
        });
    }
    private void OnInteract(Entity<CompostBinComponent> ent, ref AfterInteractUsingEvent args)
    {
        Dirty(ent);
        if (!HasComp<SolutionContainerManagerComponent>(ent) ||
            !HasComp<SolutionContainerManagerComponent>(args.Used))
        {
            return;
        }

        if (!_solutionSystem.TryGetSolution(args.Used, "food", out _, out var solutionItem))
            return;

        var quantity = solutionItem.GetReagent(new ReagentId(ent.Comp.ReagentInputId, null)).Quantity;
        if (quantity <= 0)
            return;

        _deleteQueue.Add(args.Used);
        _audio.PlayPredicted(ent.Comp.InsertSound, ent, args.User);
        ent.Comp.BufferVolume += quantity;
        args.Handled = true;
    }
}

