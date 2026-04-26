using Robust.Shared.GameStates;
using Robust.Shared.Map.Components;

namespace Content.Shared.Light.Components;

// Тёёёмная нооочь, только пули свистят по степи...
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class LightCycleComponent : Component
{
    [DataField, AutoNetworkedField]
    public Color OriginalColor = Color.Transparent;

    /// <summary>
    /// How long an entire cycle lasts
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan Duration = TimeSpan.FromMinutes(60);

    [DataField, AutoNetworkedField]
    public TimeSpan Offset;

    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    /// <summary>
    /// Should the offset be randomised upon MapInit.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool InitialOffset = true;

    [DataField, AutoNetworkedField]
    public Color ClipLevel = new Color(1f, 1f, 1.25f);

    [DataField, AutoNetworkedField]
    public Color MinLevel = new Color(0.1f, 0.15f, 0.50f);

    [DataField, AutoNetworkedField]
    public TimeSpan SunriseStartTime = TimeSpan.FromMinutes(0);

    [DataField, AutoNetworkedField]
    public TimeSpan SunriseEndTime = TimeSpan.FromMinutes(5);

    [DataField, AutoNetworkedField]
    public TimeSpan SunsetStartTime = TimeSpan.FromMinutes(45);

    [DataField, AutoNetworkedField]
    public TimeSpan SunsetEndTime = TimeSpan.FromMinutes(50);
}
