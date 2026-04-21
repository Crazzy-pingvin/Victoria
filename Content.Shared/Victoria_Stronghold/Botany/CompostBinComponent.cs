using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared.Victoria_Stronghold.Botany;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(CompostBinSystem))]
public sealed partial class CompostBinComponent : Component
{
    [DataField("reagentIn")]
    public string ReagentInputId = "Nutriment";

    [DataField("reagentOut")]
    public string ReagentOutputId = "Compost";

    [DataField("solutionOut")]
    public string SolutionOutputId = "output";

    [DataField, AutoNetworkedField, AutoPausedField]
    public TimeSpan NextRegenTime;

    [DataField]
    public FixedPoint2 CompostConvertRate = 1.0f;

    [DataField]
    public TimeSpan Interval = TimeSpan.FromSeconds(1);

    [ViewVariables]
    public Entity<SolutionComponent>? SolutionOutRef;

    [DataField]
    public FixedPoint2 BufferVolume;

    [DataField("soundInsert")]
    public SoundSpecifier? InsertSound = new SoundPathSpecifier("/Audio/Effects/trashbag1.ogg");

}
