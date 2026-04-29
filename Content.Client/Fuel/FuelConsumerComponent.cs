using Content.Shared.Fuel;

namespace Content.Client.Fuel
{
    /// <summary>
    ///     Клиентская часть компонента херовин кушающих топливо
    /// </summary>
    [RegisterComponent]
    public sealed partial class FuelConsumerComponent : SharedFuelConsumerComponent
    {
        /// <summary>
        /// Пока не используется
        [DataField("emptyState")]
        public string? EmptyState;

        /// <summary>
        /// Пока не используется
        /// </summary>
        [DataField("burningState")]
        public string? BurningState;
    }
}

    /// <summary>
    ///     Слои спрайта компонета
    /// </summary>
public enum ConsumerVisualLayers : byte
{
    Base = 0,
    Fire = 1,
}
