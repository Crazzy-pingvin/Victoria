using Content.SponsorImplementations.Shared;
using Robust.Shared.Prototypes;

namespace Content.Server._Sponsor;

[Prototype]
public sealed partial class SponsorGroupPrototype : IPrototype, ISponsorData
{
    [IdDataField] public string ID { get; set; } = default!;

    [DataField] public List<string> Prototypes { get; set;} = [];
    [DataField] public Color? Color { get; set; }
    [DataField] public int ExtraCharSlots { get; set;}
    [DataField] public bool ServerPriorityJoin { get; set;}
}
