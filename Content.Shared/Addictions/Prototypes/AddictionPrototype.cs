using Content.Shared.Whitelist;
using Content.Shared.EntityEffects;
using System.Collections.Frozen;
using System.Text.Json.Serialization;
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
    [DataField("satiationDecayTime")]
    public int SatiationDecayTime = 1800;
    [DataField("withdrawlGrowTime")]
    public int WithdrawlGrowTime = 240;
    [DataField("withdrawlDecayTime")]
    public float WithdrawlDecayTime = 120;
    [DataField("cureTime")]
    public float CureTime = 1200;

    [DataField("withdrawlEffects", serverOnly: true,required: true)]
    public List<EntityEffect> WithdrawlEffects = default!;

}

/* [DataDefinition]
public sealed partial class AddictionEffectsEntry
{
    [JsonPropertyName("effects")]
    [DataField("effects", required: true)]
    public EntityEffect[] Effects = default!;
} */
