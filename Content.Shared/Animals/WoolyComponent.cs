using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Animals;

/// <summary>
///     Gives the ability to produce wool fibers;
///     produces endlessly if the owner does not have a HungerComponent.
/// </summary>
[RegisterComponent, AutoGenerateComponentState, AutoGenerateComponentPause, NetworkedComponent]
public sealed partial class WoolyComponent : Component
{
    /// <summary>
    ///     The item to spawn.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntProtoId WoolEntity = "GoatWool";

    /// <summary>
    ///     When to next try growing wool.
    /// </summary>
    [DataField, AutoPausedField, Access(typeof(SharedWoolySystem))]
    public TimeSpan NextGrowth = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public TimeSpan GrowthDelay = TimeSpan.FromMinutes(5);

    // Note: Isn't synchronized between client and server
    // The synchronization should be done manually if you need it, cuz RT is bad at synchronization of enumerable types
    [DataField]
    public Dictionary<WoolyState, int> WoolyQuantity = new()
    {
        { WoolyState.Naked, 0 },
        { WoolyState.Short, 4 },
        { WoolyState.Normal, 6 },
        { WoolyState.Long, 10 },
    };

    [DataField, AutoNetworkedField]
    public WoolyState CurrentState = WoolyState.Normal;
}

[Serializable, NetSerializable]
public enum WoolyState
{
    Naked = 0,
    Short = 1,
    Normal = 2,
    Long = 3
}
