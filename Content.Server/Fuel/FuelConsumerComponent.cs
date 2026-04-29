using Content.Shared.Fuel;

namespace Content.Server.Fuel
{
    /// <summary>
    ///     Серверная часть компонента потребителя топлива.
    /// </summary>
    [RegisterComponent]
    public sealed partial class FuelConsumerComponent : SharedFuelConsumerComponent
    {
    /// <summary>
    ///     Кол-во выделяемого тепла. На потом.
    /// </summary>
        [DataField("atmosHeat")]
        public float AtmosHeat = 10;
    }
}
