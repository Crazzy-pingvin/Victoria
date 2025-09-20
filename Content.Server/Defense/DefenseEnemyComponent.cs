namespace Content.Server.Defense;

/// <summary>
/// Босс качалки/режима. Его смерть вызывает конец раунда
/// </summary>
[RegisterComponent]
public sealed partial class DefenseEnemyComponent : Component
{
    /// <summary>
    /// Босс или нет
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("boss")]
    public bool Boss = false;
}
