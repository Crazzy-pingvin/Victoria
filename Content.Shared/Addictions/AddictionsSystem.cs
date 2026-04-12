using System.Linq;
using System.Diagnostics.CodeAnalysis;
//using System.Runtime.CompilerServices;
using Content.Shared.Addictions.Prototypes;
using Content.Shared.Mobs;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Random;

namespace Content.Shared.Addictions;


public sealed partial class AddictionSystem: EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLog = default!;
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
            _adminLog.Add(LogType.Action, LogImpact.High, $"Тик, пытаемся");
            foreach (KeyValuePair<string, AddictionData> addiction in container.CurrentAddictions)
            {
                //addiction.Value.tick();

                if (addiction.Value.Satiation > 0.0f)
                {
                    addiction.Value.Satiation = Math.Clamp(addiction.Value.Satiation - 1.0f / addiction.Value.SatiationDecayTime,0.0f,1.0f);
                    addiction.Value.WithdrawlRate = Math.Clamp(addiction.Value.WithdrawlRate - 1.0f / addiction.Value.WithdrawlDecayTime,0.0f,1.0f);
                }
                else
                {
                    addiction.Value.WithdrawlRate = Math.Clamp(addiction.Value.WithdrawlRate + 1.0f / addiction.Value.WithdrawlGrowTime,0.0f,1.0f);
                    addiction.Value.CureRate = Math.Clamp(addiction.Value.CureRate + 1.0f / addiction.Value.CureTime ,0.0f,1.0f);
                    tryDoWithdrawlEffects(addiction.Value, ent);
                }
                _adminLog.Add(LogType.Action, LogImpact.High, $"Тук");
                if (addiction.Value.CureRate >= 1.0f)
                    RemoveAddiction(ent, addiction.Key);
            }
        }
    }

    private void tryDoWithdrawlEffects(AddictionData addiction, EntityUid uid)
    {
        AddictionPrototype proto = _prototypeManager.Index<AddictionPrototype>(addiction.Addiction_ID.Prototype);
        var args = new EntityEffectReagentArgs(uid, EntityManager, null, null, FixedPoint2.New(1), null, null, addiction.WithdrawlRate);
        foreach (EntityEffect effect in proto.WithdrawlEffects)
        {
            if (!effect.ShouldApply(args,_random))
                continue;
            effect.Effect(args);
        }
    }
    public void AddAddiction(EntityUid uid, AddictionPrototype addicProto, AddictionContainerComponent comp)
    {

        if (HasAddiction(uid, addicProto.Name, comp))
            return;
        comp.CurrentAddictions.Add(addicProto.Name, new AddictionData(addicProto));
    }

    public void RemoveAddiction(EntityUid uid, string key)
    {
        if (!TryComp<AddictionContainerComponent>(uid, out var comp))
            return;
        if (!HasAddiction(uid,key))
            return;
        comp.CurrentAddictions.Remove(key);
    }
    public void RemoveAddiction(AddictionContainerComponent comp, string key)
    {
        if (!TryGetAddiction(comp,key,out var addiction))
            return;
        comp.CurrentAddictions.Remove(key);
    }

    public void SatiateAddiction(AddictionData addiction, float satiationRate)
    {
        addiction.Satiation = Math.Clamp(addiction.Satiation + satiationRate, 0.0f, 1.0f);
    }
    public void CureAddiction(AddictionData addiction, float cureRate)
    {
        addiction.CureRate = Math.Clamp(addiction.CureRate + cureRate, 0.0f, 1.0f);
    }
    public void ChangeWithdrawl(AddictionData addiction, float withdrawlRate)
    {
        addiction.WithdrawlRate = Math.Clamp(addiction.WithdrawlRate + withdrawlRate, 0.0f, 1.0f);
    }
    public void HandleEffect(AddictionId id, EntityUid uid, bool addAddiction,
     float satiationEffect, float withdrawlEffect, float cureEffect)
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
            CureAddiction(addiction, cureEffect);
            ChangeWithdrawl(addiction, withdrawlEffect);
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
