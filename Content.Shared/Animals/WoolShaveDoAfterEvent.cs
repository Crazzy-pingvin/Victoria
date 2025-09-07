using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Animals;

[Serializable, NetSerializable]
public sealed partial class WoolShaveDoAfterEvent : SimpleDoAfterEvent
{
}
