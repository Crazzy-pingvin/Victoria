using Content.Server.Administration.Logs;
using Content.Shared.Interaction;
using Content.Shared.Doors.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Content.Server.Doors.Systems;
using Content.Server.Power.EntitySystems;
using Content.Shared.Database;
using Content.Shared.Examine;

namespace Content.Server.Bruteforcer;

public sealed class BruteforcerSystem : EntitySystem
{

    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AirlockSystem _airlock = default!;
    [Dependency] private readonly DoorSystem _doorSystem = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<BruteforcerComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(Entity<BruteforcerComponent> entity, ref AfterInteractEvent args)
    {
        bool isAirlock = TryComp<AirlockComponent>(args.Target, out var airlockComp);

        if (args.Handled || args.Target == null || !TryComp<DoorComponent>(args.Target, out var doorComp) || !_examine.InRangeUnOccluded(args.User, args.Target.Value, SharedInteractionSystem.MaxRaycastRange, null))
        {
            return;
        }

        args.Handled = true;
        if (TryComp<DoorBoltComponent>(args.Target, out var boltsComp))
        {
            if (!boltsComp.BoltWireCut && !boltsComp.BoltsDown)
            {
                _doorSystem.SetBoltsDown((args.Target.Value, boltsComp), !boltsComp.BoltsDown, args.Used);
                _audio.PlayPvs(entity.Comp.Sound, entity.Owner);
                _adminLogger.Add(LogType.Action, LogImpact.Medium, $"{ToPrettyString(args.User):player} used {ToPrettyString(args.Used)} on {ToPrettyString(args.Target.Value)} to {(boltsComp.BoltsDown ? "" : "un")}bolt it");
            }
        }
        //break;
    }

}
