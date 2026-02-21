using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
//using System;

namespace Content.Shared.Addictions.Prototypes;

[DataDefinition, NetSerializable, Serializable]
public sealed partial class AddictionData
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Satiation = 1;
    //[DataField, ViewVariables(VVAccess.ReadWrite)]
    //public CurrentState = AddictionStates.Normal;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float CureRate = 0;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float WithdrawlRate = 0;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public AddictionId Prototype;
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Name = "BASEEE";

    [IncludeDataField]
    [ViewVariables]
    public AddictionId Addiction_ID { get; private set; }

    [DataField]
    public int SatiationDecayTime = 1800;
    [DataField]
    public int WithdrawlGrowTime = 120;
    [DataField]
    public float WithdrawlDecayTime = 180;
    [DataField]
    public float CureTime = 1800;

    public AddictionData(AddictionPrototype proto)
    {
        Name = proto.Name;
        Addiction_ID = new AddictionId(proto.ID);
        SatiationDecayTime = proto.SatiationDecayTime;
        WithdrawlGrowTime = proto.WithdrawlGrowTime;
        WithdrawlDecayTime = proto.WithdrawlDecayTime;
        CureTime = proto.CureTime;
    }

    //public void tick()
    //{

    //}
    public string ToString()
    {
        return $"{Satiation}, {Name}";
    }
   // public enum AddictionStates
  //  {
       // Normal,
       // Withdrawl
    //}
}

