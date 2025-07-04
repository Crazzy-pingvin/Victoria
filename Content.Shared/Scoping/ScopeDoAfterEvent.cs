using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Scoping;

[Serializable, NetSerializable]
public sealed partial class ScopeDoAfterEvent : SimpleDoAfterEvent
{
    [DataField]
    public Direction Direction;

    public ScopeDoAfterEvent(Direction direction)
    {
        Direction = direction;
    }
}
