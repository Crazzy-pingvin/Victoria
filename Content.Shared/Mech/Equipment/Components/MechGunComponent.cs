using Robust.Shared.Prototypes;

namespace Content.Shared.Mech.Equipment.Components;

/// <summary>
/// A piece of mech equipment that grabs entities and stores them
/// inside of a container so large objects can be moved.
/// </summary>
[RegisterComponent]
public sealed partial class MechGunComponent : Component
{
    [DataField]
    public ComponentRegistry Comps;
}
