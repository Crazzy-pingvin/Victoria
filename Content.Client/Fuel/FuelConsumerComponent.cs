using Content.Shared.Fuel;

namespace Content.Client.Fuel
{
    /// <summary>
    ///     Component that represents a handheld expendable light which can be activated and eventually dies over time.
    /// </summary>
    [RegisterComponent]
    public sealed partial class FuelConsumerComponent : SharedFuelConsumerComponent
    {
        /// <summary>
        /// The icon state used by expendable lights when the they have been completely expended.
        /// </summary>
        [DataField("emptyState")]
        public string? EmptyState;

        /// <summary>
        /// The icon state used by expendable lights while they are lit.
        /// </summary>
        [DataField("burningState")]
        public string? BurningState;
    }
}

public enum ConsumerVisualLayers : byte
{
    Base = 0,
    Fire = 1,
}
