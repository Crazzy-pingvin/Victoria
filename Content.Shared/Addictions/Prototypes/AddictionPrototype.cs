using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
namespace Content.Shared.Addictions.Prototypes;

/// <summary>
/// This is a prototype for a cargo bounty, a set of items
/// that must be sold together in a labeled container in order
/// to receive a monetary reward.
/// </summary>
[Prototype("addiction")]
[DataDefinition]
public sealed partial class AddictionPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string Name = "BASE";
    [DataField]
    public int SatiationDecayTime = 1800;
    [DataField]
    public int WithdrawlGrowTime = 600;
    [DataField]
    public float WithdrawlDecayTime = 300;
}
