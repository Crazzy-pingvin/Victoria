using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Prototypes;

namespace Content.SponsorImplementations.Shared;

public interface ISponsorData
{
    public List<string> Prototypes {get;}
    public Color? Color {get;}
    public int ExtraCharSlots {get;}
    public bool ServerPriorityJoin { get; }
}

[DataDefinition]
public sealed partial class SponsorData: ISponsorData
{
    [DataField] public Guid Guid { get; set; }
    [DataField] public List<string> Prototypes { get; set; }  = new();
    [DataField] public Color? Color { get; set; }
    [DataField] public int ExtraCharSlots { get; set; }
    [DataField] public bool ServerPriorityJoin { get; set;}

    public void CopyFrom(ISponsorData other)
    {
        Prototypes = other.Prototypes;
        Color = other.Color;
        ExtraCharSlots = other.ExtraCharSlots;
        ServerPriorityJoin = other.ServerPriorityJoin;
    }
}
