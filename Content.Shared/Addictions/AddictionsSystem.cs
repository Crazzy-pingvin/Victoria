using System.Linq;
using System.Diagnostics.CodeAnalysis;
//using System.Runtime.CompilerServices;
using Content.Shared.Addictions.Prototypes;
using Content.Shared.Mobs;
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
    public void AddAddiction(EntityUid uid, AddictionPrototype addicProto, AddictionContainerComponent comp)
    {

        if (HasAddiction(uid,addicProto.Name,comp))
            return;
        comp.CurrentAddictions.Add(addicProto.Name, new AddictionData(addicProto));
    }
    public void SatiateAddiction(AddictionData addiction, float satiationRate)
    {
        addiction.Satiation += satiationRate;
    }

    public void HandleEffect(AddictionId id, EntityUid uid,bool addAddiction,float satiationEffect, float withdrawlEffect, float cureEffect)
    {
        if (!TryComp<AddictionContainerComponent>(uid,out var container))
            return;
        AddictionPrototype addicProto = _prototypeManager.Index<AddictionPrototype>(id.Prototype);
        if (!TryGetAddiction(container,addicProto.Name, out var addiction)){
            if (addAddiction)
                AddAddiction(uid,addicProto, container);
        }
        else
        {
            SatiateAddiction(addiction, satiationEffect);
        }
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

    public bool TryGetAddiction(AddictionContainerComponent container, string key, [NotNullWhen(true)] out AddictionData? addiction)
    {
        addiction = null;
        if (!container.CurrentAddictions.TryGetValue(key,out var temp_addic))
        return false;
        addiction = temp_addic;
        return true;
    }
}
