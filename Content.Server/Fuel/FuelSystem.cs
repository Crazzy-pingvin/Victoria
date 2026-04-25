using Content.Server.Fuel;
using Content.Shared.Examine;
using Content.Shared.Fuel;
using Robust.Shared.Timing;
using Content.Shared.Interaction;

namespace Content.Server.Fuel;
public sealed partial class FuelSystem : EntitySystem
{
    [Dependency] private readonly SharedPointLightSystem _pointLightSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FuelConsumerComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<FuelComponent, AfterInteractEvent>(OnRefuelAttempt);
    }
    private void OnMapInit(Entity<FuelConsumerComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextCheck = _timing.CurTime + TimeSpan.FromSeconds(5);
    }

    private void OnUnpaused(Entity<FuelConsumerComponent> ent, ref EntityUnpausedEvent args)
    {
        ent.Comp.NextCheck += args.PausedTime;
    }


    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FuelConsumerComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_timing.CurTime < comp.NextCheck)
                continue;
            comp.NextCheck = _timing.CurTime + TimeSpan.FromSeconds(5);
            if (comp.CurrentState == ConsumerStates.Burns)
            {
                ChangeFuelLevel(uid,comp, -comp.FuelConsumption);
            }
        }
    }

    public void ChangeFuelLevel(EntityUid uid, FuelConsumerComponent consumer, float change_amount)
    {
        consumer.CurrentFuel = Math.Clamp(consumer.CurrentFuel + change_amount,0, consumer.MaxFuel);
        UpdateConsumerState(uid, consumer);
        //Dirty(uid, consumer);
        UpdateVisualizer((uid,consumer));
        UpdateLight((uid,consumer));
    }


    public void UpdateConsumerState(EntityUid uid, FuelConsumerComponent consumer)
    {
        if (consumer.CurrentFuel <= 0)
            consumer.CurrentState = ConsumerStates.Empty;
        else consumer.CurrentState = ConsumerStates.Burns;
        //Dirty(uid, consumer);
    }

    private void UpdateVisualizer(Entity<FuelConsumerComponent> ent, AppearanceComponent? appearance = null)
    {
        var component = ent.Comp;
        if (!Resolve(ent, ref appearance, false))
            return;
        //float percent = component.CurrentFuel / component.MaxFuel;
        _appearance.SetData(ent, FuelVisuals.State, component.CurrentState, appearance);
        //_appearance.SetData(ent, FuelVisuals.Percent, percent, appearance);
    }

    private void UpdateLight(Entity<FuelConsumerComponent> ent)
    {
        FuelConsumerComponent component = ent.Comp;
        float percent = component.CurrentFuel / component.MaxFuel;
        _pointLightSystem.SetEnergy(ent,100 * percent);
    }
    private void OnRefuelAttempt(Entity<FuelComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Target == null)
            return;
        if (!TryComp<FuelConsumerComponent>(args.Target, out FuelConsumerComponent? consumer))
            return;
        EntityUid Target2 = args.Target.Value;
        ChangeFuelLevel(Target2, consumer, ent.Comp.FuelPrice);
    }

    private void OnExamine(EntityUid uid, FuelConsumerComponent comp, ExaminedEvent args)
    {
        float fuel_percent = comp.CurrentFuel / comp.MaxFuel * 100;
        args.PushMarkup(fuel_percent.ToString() + "% топлива");
    }

    //public bool IsBurns(EntityUid uid)
  //  {

   // }

}
