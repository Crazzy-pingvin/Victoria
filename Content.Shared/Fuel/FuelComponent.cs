namespace Content.Shared.Fuel;

/// <summary>
/// This component is for mobs that have DNA.
/// </summary>
[RegisterComponent]
public sealed partial class FuelComponent : Component
{
    [DataField("fuelPrice")]
    public float FuelPrice = 10;
}
