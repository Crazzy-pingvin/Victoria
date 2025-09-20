namespace Content.Server.Defense;

/// <summary>
/// Настрйоки режима. Ставить под флаг, настраивать под каждую карту.
/// </summary>
[RegisterComponent]
public sealed partial class DefenseSettingsComponent : Component
{
    /// <summary>
    /// Стартовое вол-во узлов
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("startNodes")]
    public int StartNodes = 4;
}
