using Robust.Shared.Audio;
using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Shared.Garotte;

[RegisterComponent, NetworkedComponent]
public sealed partial class GarotteComponent : Component
{
    /// <summary>
    /// Скорость накидывания/сжатия удавки. Чем больше- тем короче.
    /// </summary>
    [DataField("speedModifier")]
    public float SpeedModifier = 1.0f;

    [DataField("setSound")]
    public SoundSpecifier SetSound = new SoundPathSpecifier("/Audio/Items/bow_pull.ogg");
    [DataField("chokeSound")]
    public SoundSpecifier ChokeSound = new SoundPathSpecifier("/Audio/Effects/Gasp/gasp_male1.ogg");


    [DataField("damage", required: true)]
    public DamageSpecifier Damage = default!;

    /// <summary>
    /// Длительность стана и немоты
    /// </summary>
    [DataField("duration")]
    public TimeSpan Duration = TimeSpan.FromSeconds(3);
}

/// <summary>
/// бефор ивент для гарроты
/// </summary>
[ByRefEvent]
public record struct BeforeGarotteEvent(EntityUid User, bool PryPowered, bool StrongPry)
{
    public readonly EntityUid User = User;
    public string? Message;

    public bool Cancelled;
}
