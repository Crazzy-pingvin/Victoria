using Content.Server.Fuel;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Monitor.Systems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Popups;
using Content.Shared.Popups;
using Content.Shared.Examine;
using Content.Shared.Fuel;
using Robust.Shared.Timing;
using Content.Shared.Interaction;
using Content.Shared.Stacks;
using Content.Server.Stack;

namespace Content.Server.Fuel;
public sealed partial class FuelSystem : EntitySystem
{
    [Dependency] private readonly SharedPointLightSystem _pointLightSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly StackSystem _stackSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<FuelConsumerComponent, AtmosDeviceUpdateEvent>(OnAtmosUpdate);

        SubscribeLocalEvent<FuelConsumerComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<FuelConsumerComponent, InteractUsingEvent>(OnRefuelAttempt);
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
                ChangeFuelLevel(uid, comp, -comp.FuelConsumption);
            }
        }
    }

/*     private void OnAtmosUpdate(EntityUid uid, FuelConsumerComponent consumer, ref AtmosDeviceUpdateEvent args)
    {
        consumer.LastEnergyDelta = 0f;
        if (!(_power.IsPowered(uid) && TryComp<ApcPowerReceiverComponent>(uid, out var receiver)))
            return;

        GetHeatExchangeGasMixture(uid, consumer, out var heatExchangeGasMixture);
        if (heatExchangeGasMixture == null)
            return;

        float sign = Math.Sign(consumer.Cp); // 1 if heater, -1 if freezer
        float targetTemp = consumer.TargetTemperature;
        float highTemp = targetTemp + sign * consumer.TemperatureTolerance;
        float temp = heatExchangeGasMixture.Temperature;

        if (sign * temp >= sign * highTemp) // upper bound
            consumer.HysteresisState = false; // turn off
        else if (sign * temp < sign * targetTemp) // lower bound
            consumer.HysteresisState = true; // turn on

        if (consumer.HysteresisState)
            targetTemp = highTemp; // when on, target upper hysteresis bound
        else // Hysteresis is the same as "Should this be on?"
        {
            // Turn dynamic load back on when power has been adjusted to not cause lights to
            // blink every time this heater comes on.
            //receiver.Load = 0f;
            return;
        }

        // Multiply power in by coefficient of performance, add that heat to gas
        float dQ = consumer.HeatCapacity * consumer.Cp * args.dt;

        // Clamps the heat transferred to not overshoot
        float Cin = _atmosphereSystem.GetHeatCapacity(heatExchangeGasMixture, true);
        float dT = targetTemp - temp;
        float dQLim = dT * Cin;
        float scale = 1f;
        if (Math.Abs(dQ) > Math.Abs(dQLim))
        {
            scale = dQLim / dQ; // reduce power consumption
            consumer.HysteresisState = false; // turn off
        }

        float dQActual = dQ * scale;
        if (consumer.Atmospheric)
        {
            _atmosphereSystem.AddHeat(heatExchangeGasMixture, dQActual);
            consumer.LastEnergyDelta = dQActual;
        }
        else
        {
            float dQLeak = dQActual * consumer.EnergyLeakPercentage;
            float dQPipe = dQActual - dQLeak;
            _atmosphereSystem.AddHeat(heatExchangeGasMixture, dQPipe);
            consumer.LastEnergyDelta = dQPipe;

            if (dQLeak != 0f && _atmosphereSystem.GetContainingMixture(uid, args.Grid, args.Map, excite: true) is { } containingMixture)
                _atmosphereSystem.AddHeat(containingMixture, dQLeak);
        }

        receiver.Load = consumer.HeatCapacity;// * scale; // we're not ready for dynamic load yet, see note above
    } */
    public void ChangeFuelLevel(EntityUid uid, FuelConsumerComponent consumer, float change_amount)
    {
        consumer.CurrentFuel = Math.Clamp(consumer.CurrentFuel + change_amount,0, consumer.MaxFuel);
        UpdateConsumerState(uid, consumer);
        UpdateVisualizer((uid, consumer));
        UpdateLight((uid, consumer));
    }


    public void UpdateConsumerState(EntityUid uid, FuelConsumerComponent consumer)
    {
        if (consumer.CurrentFuel <= 0)
            consumer.CurrentState = ConsumerStates.Empty;
        else consumer.CurrentState = ConsumerStates.Burns;
    }

    private void UpdateVisualizer(Entity<FuelConsumerComponent> ent, AppearanceComponent? appearance = null)
    {
        var component = ent.Comp;
        if (!Resolve(ent, ref appearance, false))
            return;
        _appearance.SetData(ent, FuelVisuals.State, component.CurrentState, appearance);
    }

    private void UpdateLight(Entity<FuelConsumerComponent> ent)
    {
        FuelConsumerComponent component = ent.Comp;
        float percent = component.CurrentFuel / component.MaxFuel;
        _pointLightSystem.SetEnergy(ent, component.MaxEnergy * Math.Max(0.1f, percent));
    }
    private void OnRefuelAttempt(Entity<FuelConsumerComponent> ent, ref InteractUsingEvent args)
    {
        FuelConsumerComponent consumer = ent.Comp;
        if (args.Handled)
            return;
        if (!TryComp<FuelComponent>(args.Used, out var fuel))
            return;
        if (consumer.MaxFuel == consumer.CurrentFuel){
            _popupSystem.PopupClient(Loc.GetString("fuel-system-max-fuel"),args.User,PopupType.Small);
            return;
        }

        if (!TryComp<StackComponent>(args.Used, out var stack))
        {
            ChangeFuelLevel(ent, ent.Comp, fuel.FuelPrice);
            Del(args.Used);
            return;
        }
        float fuel_shortage = consumer.MaxFuel - consumer.CurrentFuel;
        int pieces_need = (int)Math.Ceiling(fuel_shortage / fuel.FuelPrice);
        ChangeFuelLevel(ent, consumer, fuel_shortage);
        _stackSystem.SetCount(args.Used, stack.Count - pieces_need, stack);
    }

    private void OnExamine(EntityUid uid, FuelConsumerComponent comp, ExaminedEvent args)
    {
        float fuel_percent = comp.CurrentFuel / comp.MaxFuel * 100;
        if (fuel_percent != 0.0)
        args.PushMarkup(fuel_percent.ToString() + "% топлива");
        else args.PushMarkup("Топливо закончилось");
    }

    //public bool IsBurns(EntityUid uid)
  //  {

   // }

}
