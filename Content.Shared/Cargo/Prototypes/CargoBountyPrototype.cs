using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
namespace Content.Shared.Cargo.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

/// <summary>
/// This is a prototype for a cargo bounty, a set of items
/// that must be sold together in a labeled container in order
/// to receive a monetary reward.
/// </summary>
[Prototype, Serializable, NetSerializable]
public sealed partial class CargoBountyPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The monetary reward for completing the bounty
    /// </summary>
    [DataField(required: true)]
    public int Reward;
    /// <summary>
    /// Какие плюшки заспавнить при выполнении запроса?
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("giftType", customTypeSerializer:typeof(PrototypeIdSerializer<StackPrototype>))]
    public string GiftType = "Credit";
    /// <summary>
    /// Кол-во плюшек
    /// </summary>
    [DataField("giftAmount",required: false)]
    public int GiftAmount = 0;
    /// <summary>
    /// текст в лабеле в консоли
    /// </summary>
    [DataField("giftText",required: false)]
    public string GiftText = string.Empty;
    /// <summary>
    /// A description for flava purposes.
    /// </summary>
    [DataField]
    public LocId Description = string.Empty;

    /// <summary>
    /// The entries that must be satisfied for the cargo bounty to be complete.
    /// </summary>
    [DataField(required: true)]
    public List<CargoBountyItemEntry> Entries = new();

    /// <summary>
    /// A prefix appended to the beginning of a bounty's ID.
    /// </summary>
    [DataField]
    public string IdPrefix = "NT";
}

[DataDefinition, Serializable, NetSerializable]
public readonly partial record struct CargoBountyItemEntry()
{
    /// <summary>
    /// A whitelist for determining what items satisfy the entry.
    /// </summary>
    [DataField(required: true)]
    public EntityWhitelist Whitelist { get; init; } = default!;

    /// <summary>
    /// A blacklist that can be used to exclude items in the whitelist.
    /// </summary>
    [DataField]
    public EntityWhitelist? Blacklist { get; init; } = null;

    // todo: implement some kind of simple generic condition system

    /// <summary>
    /// How much of the item must be present to satisfy the entry
    /// </summary>
    [DataField]
    public int Amount { get; init; } = 1;

    /// <summary>
    /// A player-facing name for the item.
    /// </summary>
    [DataField]
    public LocId Name { get; init; } = string.Empty;
}
