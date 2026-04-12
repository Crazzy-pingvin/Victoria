using Content.Shared.Addictions.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;


namespace Content.Shared.Addictions;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AddictionContainerComponent : Component
{
    /// <summary>
    /// The amount of fuel in the container.
    /// </summary>
    [ViewVariables, AutoNetworkedField]

    public Dictionary<string, AddictionData> CurrentAddictions = new();

    [DataField("nextSecond", customTypeSerializer: typeof(TimeOffsetSerializer)), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextCheck = TimeSpan.Zero;
}

