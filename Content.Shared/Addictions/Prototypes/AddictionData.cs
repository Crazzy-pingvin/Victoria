using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
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

    [DataField]
    public int SatiationDecayTime = 1800;
    [DataField]
    public int WithdrawlGrowTime = 600;
    [DataField]
    public float WithdrawlDecayTime = 300;

    public AddictionData(AddictionPrototype proto)
    {
        Name = proto.Name;
        SatiationDecayTime = proto.SatiationDecayTime;
        WithdrawlGrowTime = proto.WithdrawlGrowTime;
        WithdrawlDecayTime = proto.WithdrawlDecayTime;
    }

    public void tick()
    {
        if (Satiation > 0.0f){
            Satiation -= 1.0f / SatiationDecayTime;
            WithdrawlRate -= 1.0f / WithdrawlDecayTime;
        }
        else
            WithdrawlRate += 1.0f / WithdrawlGrowTime;
    }
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

