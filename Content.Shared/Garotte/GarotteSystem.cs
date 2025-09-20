using System.Diagnostics.CodeAnalysis;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Serialization;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs.Components;
using Content.Shared.Stunnable;
using Content.Shared.Speech.Muting;
using Content.Shared.StatusEffect;

namespace Content.Shared.Garotte;

/// <summary>
///     Verb for violently murdering cuffed creatures.
/// </summary>
public sealed partial class SharedGarotteSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLog = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffect = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChockableComponent, InteractUsingEvent>(TrySetGarotte);
        SubscribeLocalEvent<ChockableComponent, GarotteDoAfterEvent>(OnDoAfter);
    }

    private void TrySetGarotte(EntityUid uid, ChockableComponent comp, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = TrySet(uid, args.User, out _, args.Used);
    }
    public bool TrySet(EntityUid target, EntityUid user, out DoAfterId? id, EntityUid tool)
    {
        id = null;

        GarotteComponent? comp = null;
        if (!Resolve(tool, ref comp, false))
            return false;

        if (target == user)
        {
            _popup.PopupClient(Loc.GetString("garrote-target-yourself"), target, user);
            return true;
        }
        if (TryComp<MobStateComponent>(target, out var state))
        {
            if (state.CurrentState == Mobs.MobState.Dead)
            {
                _popup.PopupClient(Loc.GetString("garrote-dead-target"), target, user);
                return true;
            }
        }


        StartSet(target, user, tool, comp.SpeedModifier, out id);
        _audio.PlayPredicted(comp.SetSound, target, user);
        return true;
    }
    private bool StartSet(EntityUid target, EntityUid user, EntityUid? tool, float toolModifier, [NotNullWhen(true)] out DoAfterId? id)
    {

        RaiseLocalEvent(target);
        double baseTime = 1.8;
        double time = baseTime / toolModifier;

        if (TryComp<StunnedComponent>(target, out var _))
        {
            time = time / 1.4;
            _popup.PopupPredicted(Loc.GetString("garrote-set-stunned", ("user", Identity.Name(user, EntityManager)), ("target", Identity.Name(target, EntityManager))), target, user, PopupType.SmallCaution);
        }
        else
        {
            _popup.PopupPredicted(Loc.GetString("garrote-set-normal", ("user", Identity.Name(user, EntityManager)), ("target", Identity.Name(target, EntityManager))), target, user, PopupType.SmallCaution);
            //Проверка на поворот спиной
            Angle targ_rot = _transformSystem.GetWorldRotation(target);
            Angle user_rot = _transformSystem.GetWorldRotation(user);
            double diff = Math.Abs(targ_rot.Theta - user_rot.Theta);
            if (diff < -Math.PI) diff += 2 * Math.PI;
            if (diff > Math.PI) diff -= 2 * Math.PI;
            if (Math.Abs(diff) < 1.27)
                time = time / 1.4;
        }



        var doAfterArgs = new DoAfterArgs(EntityManager, user, TimeSpan.FromSeconds(time), new GarotteDoAfterEvent(), target, target, tool)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            NeedHand = tool != user,
            MovementThreshold = 0.2f
        };

        if (tool != user && tool != null)
        {
            _adminLog.Add(LogType.Action, LogImpact.High, $"{ToPrettyString(user)} душит {ToPrettyString(target)} с помощью {ToPrettyString(tool.Value)}!");
        }
        else
        {
            _adminLog.Add(LogType.Action, LogImpact.High, $"{ToPrettyString(user)} душит {ToPrettyString(target)}!");
        }

        return _doAfterSystem.TryStartDoAfter(doAfterArgs, out id);

    }

    private void OnDoAfter(EntityUid uid, ChockableComponent mob, GarotteDoAfterEvent args)
    {
        if (args.Cancelled)
            return;
        if (args.Target is null)
            return;

        TryComp<GarotteComponent>(args.Used, out var comp);
        if (comp == null)
            return;
        //if (!CanPry(uid, args.User, out var message, comp))
        //{
        //if (!string.IsNullOrWhiteSpace(message))
        //  _popup.PopupClient(Loc.GetString(message), uid, args.User);
        //return;
        //}

        var ev = new GarotteSetEvent(args.User);
        RaiseLocalEvent(uid, ref ev);
        _stun.TryStun(uid, comp.Duration, true);
        _stun.TryKnockdown(uid, comp.Duration, true);
        _damage.TryChangeDamage(uid, comp.Damage);
        _statusEffect.TryAddStatusEffect<MutedComponent>(uid, "Muted", comp.Duration, true);
        _audio.PlayPredicted(comp.ChokeSound, uid, args.User);
    }
}



[Serializable, NetSerializable]
public sealed partial class GarotteDoAfterEvent : SimpleDoAfterEvent;

[ByRefEvent]
public readonly record struct GarotteSetEvent(EntityUid User)
{
    public readonly EntityUid User = User;
}
