namespace Content.Shared.Weapons.Ranged.Events;

/// <summary>
/// Raised on a player when it is trying to shot but doesn't have gun directly (in hands, being a gun or smh)
/// </summary>
[ByRefEvent]
public sealed partial class GetGunEvent : EntityEventArgs
{
    public EntityUid? Gun;
}
