using System.Linq;
//using System.Runtime.CompilerServices;
using Content.Shared.Addictions.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Addictions;


public sealed partial class AddictionSystem: EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<AddictionContainerComponent>();

        while (query.MoveNext(out var ent, out var container))
        {
            if (_timing.CurTime < container.NextCheck)
                continue;
            container.NextCheck = _timing.CurTime + TimeSpan.FromSeconds(1);

            foreach (KeyValuePair<string, AddictionData> addiction in container.CurrentAddictions)
            {
                addiction.Value.tick();
            }
        }
    }
    public void AddAddiction(AddictionId id, EntityUid uid)
    {
        if (!TryComp<AddictionContainerComponent>(uid,out var comp))
            return;
        AddictionPrototype addicProto = _prototypeManager.Index<AddictionPrototype>(id.Prototype);
        if (HasAddiction(uid,addicProto.Name,comp))
            return;
        comp.CurrentAddictions.Add(addicProto.Name, new AddictionData(addicProto));
    }
    public bool HasAddiction(EntityUid uid, string key,
        AddictionContainerComponent? container = null)
    {
        if (!Resolve(uid, ref container, false))
            return false;
        if (!container.CurrentAddictions.ContainsKey(key))
            return false;

        return true;
    }
}
