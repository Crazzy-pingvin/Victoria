using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Spawners.Components;

[RegisterComponent]
public sealed partial class StateChoiceSpawnerComponent : Component
{
    [DataField("umpor", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? Umpor;

    [DataField("mirt", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? Mirt;

    [DataField("cdc", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? Cdc;

    [DataField("nt", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? Nt;

    [DataField("ussp", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? Ussp;
}
