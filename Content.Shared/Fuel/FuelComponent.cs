namespace Content.Shared.Fuel;

/// <summary>
/// Компонент куска топлива
/// </summary>
[RegisterComponent]
public sealed partial class FuelComponent : Component
{
    [DataField("fuelPrice")]
    public float FuelPrice = 10;
}
