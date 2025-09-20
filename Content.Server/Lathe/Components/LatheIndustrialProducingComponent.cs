using Content.Shared.Lathe;
using Content.Server.Atmos.EntitySystems;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Lathe.Components;

/// <summary>
/// This is used for a <see cref="LatheComponent"/> that releases CO₂ into the surroundings while producing items.
/// </summary>
[RegisterComponent]
[Access(typeof(LatheSystem), typeof(AtmosphereSystem))]
public sealed partial class LatheIndustrialProducingComponent : Component
{
    /// <summary>
    /// The amount of CO₂ (in moles) released each second when producing an item.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float MolesPerSecond = 50f;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextUpdate;
}