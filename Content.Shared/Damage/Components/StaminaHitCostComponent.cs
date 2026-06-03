using Robust.Shared.Audio;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
public sealed partial class StaminaHitCostComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("cost")]
    public float Cost = 30f;

}
