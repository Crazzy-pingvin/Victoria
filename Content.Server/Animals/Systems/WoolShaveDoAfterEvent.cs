using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Server.Animals.Systems;

[Serializable, NetSerializable]
public sealed partial class WoolShaveDoAfterEvent : SimpleDoAfterEvent
{
}
