using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;


namespace Content.Shared.Fuel;

/// <summary>
/// blyaaa
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FuelConsumerComponent : Component
{
    [DataField("currentFuel"), AutoNetworkedField]
    public float CurrentFuel = 100;

    [DataField("maxFuel")]
    public float MaxFuel = 100;

    [DataField("fuelConsumption")]
    public float FuelConsumption = 1;

    [DataField("nextSecond", customTypeSerializer: typeof(TimeOffsetSerializer)), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextCheck = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public ConsumerStates CurrentState = ConsumerStates.Burns;
}

[Serializable, NetSerializable]
public enum FuelVisuals
{
    State,
    Behavior,
    Percent
}

[Serializable, NetSerializable]
public enum ConsumerStates
{
    Empty,
    Burns
}
