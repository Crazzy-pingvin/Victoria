namespace Content.Shared.Weapons.Ranged.Events;

/// <summary>
/// Raised on an AmmoProvider to request deets.
/// </summary>
[ByRefEvent]
public sealed partial class GetAmmoCountEvent : EntityEventArgs
{
    public int Count;
    public int Capacity;
}
