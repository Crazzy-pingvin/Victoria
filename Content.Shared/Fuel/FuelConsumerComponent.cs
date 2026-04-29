using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;


namespace Content.Shared.Fuel;

/// <summary>
/// Компонент херовин, кушающих топливо
/// </summary>
[NetworkedComponent]
public abstract partial class SharedFuelConsumerComponent : Component
{
    /// <summary>
    ///     Текущий уровень топляка
    /// </summary>
    [DataField("currentFuel")]
    public float CurrentFuel = 100;

    /// <summary>
    ///     Максимальный уровень топлива
    /// </summary>
    [DataField("maxFuel")]
    public float MaxFuel = 100;

    /// <summary>
    ///     потребление (за 5 сек)
    /// </summary>
    [DataField("fuelConsumption")]
    public float FuelConsumption = 1;
    /// <summary>
    ///     Энергия свечения (на максимальном кол-ве топлива)
    /// </summary>
    [DataField("maxEnergy")]
    public float MaxEnergy = 10;
    /// <summary>
    ///     Следующий чек
    /// </summary>
    [DataField("nextSecond", customTypeSerializer: typeof(TimeOffsetSerializer)), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextCheck = TimeSpan.Zero;
    /// <summary>
    ///     Текущее состояние
    /// </summary>
    [DataField]
    public ConsumerStates CurrentState = ConsumerStates.Burns;
}

    /// <summary>
    ///     Ключи апперенса компонента
    /// </summary>
[Serializable, NetSerializable]
public enum FuelVisuals
{
    State,
    Behavior,
    Percent
}

    /// <summary>
    ///     Состояния потербителя
    /// </summary>
[Serializable, NetSerializable]
public enum ConsumerStates
{
    Empty,
    Burns
}
