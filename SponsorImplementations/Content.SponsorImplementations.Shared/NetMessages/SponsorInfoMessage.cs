using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.SponsorImplementations.Shared.NetMessages;

public sealed class SponsorInfoMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public List<string> Prototypes = new();
    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var count = buffer.ReadVariableInt32();
        Prototypes.EnsureCapacity(count);

        for (var i = 0; i < count; i++)
        {
            Prototypes.Add(buffer.ReadString());
        }
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.WriteVariableInt32(Prototypes.Count);

        foreach (var prototype in Prototypes)
        {
            buffer.Write(prototype);
        }
    }
}
