namespace Content.Server.Defense;

/// <summary>
/// Цель ксеноморфов
/// <summary>
[RegisterComponent]
public sealed partial class DefenseTargetComponent : Component
{
    /// <summary>
    /// Флаг или нет
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("flag")]
    public bool Flag = false;

    /// <summary>
    /// Имя узла.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("name")]
    public string Name = "Подстанция";
}
