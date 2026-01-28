using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System.Linq;

namespace Content.Shared.Addictions.Prototypes;

/// <summary>
/// St
/// </summary>
[Serializable, NetSerializable]
[DataDefinition]
public partial struct AddictionId : IEquatable<AddictionId>
{
    // TODO rename data field.
    [DataField("AddictionId", customTypeSerializer: typeof(PrototypeIdSerializer<AddictionPrototype>), required: true)]
    public string Prototype { get; private set; }

    public AddictionId(string prototype)
    {
        Prototype = prototype;
    }

    public AddictionId()
    {
        Prototype = default!;
    }
    public bool Equals(AddictionId other)
    {
        if (Prototype != other.Prototype)
            return false;

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is AddictionId other && Equals(other);
    }

    public string ToString(FixedPoint2 quantity)
    {
        return $"{Prototype}:{quantity}";
    }

    public override string ToString()
    {
        return $"{Prototype}";
    }

    public static bool operator ==(AddictionId left, AddictionId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AddictionId left, AddictionId right)
    {
        return !(left == right);
    }
}
