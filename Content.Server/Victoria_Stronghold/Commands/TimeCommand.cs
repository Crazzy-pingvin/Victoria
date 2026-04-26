using Content.Shared.Administration;
using Content.Server.Administration;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Server.GameObjects;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;

namespace Content.Server.Victoria_Stronghold.Commands;

[AdminCommand(AdminFlags.VarEdit)]
public sealed class TimeCommand : LocalizedEntityCommands
{
    [Dependency] private readonly MapSystem _map = default!;
    [Dependency] private readonly SharedLightCycleSystem _light = default!;
    public override string Command => "timeid";
    public override string Help => "timeid (MapId) (get|skip|next)";

    private enum SecondSubCommand
    {
        get,
        skip,
        next,
    }

    private enum TimeOfDay
    {
        morning,
        day,
        evening,
        night
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length <= 1)
        {
            shell.WriteLine(Help);
            return;
        }
        if (!int.TryParse(args[0], out var uid))
            return;
        var mapId = _map.GetMap(new MapId(uid));
        if (!EntityManager.TryGetComponent<LightCycleComponent>(mapId, out var cycle))
        {
            shell.WriteError($"Map {mapId} - {EntityManager.MetaQuery.GetComponent(mapId).EntityName} does not have LightCycleComponent");
            return;
        }
        switch (args[1])
        {
            case nameof(SecondSubCommand.next):
                return;
            case nameof(SecondSubCommand.get):
                shell.WriteLine(_light.GetDayTimeSpan((mapId, cycle)).ToString());
                return;
            case nameof(SecondSubCommand.skip):
                if (args.Length != 3)
                {
                    shell.WriteError("Expected 3 arguments");
                    return;
                }
                if (!float.TryParse(args[2], out var time))
                {
                    shell.WriteError("Error parsing arg 3");
                    return;
                }
                _light.SkipTime((mapId, cycle), TimeSpan.FromSeconds(time));
                return;

        }
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        switch (args.Length)
        {
            case 1:
                List<CompletionOption> opts = [];
                foreach (var mapId in _map.GetAllMapIds())
                {
                    var meta = EntityManager.MetaQuery.Get(_map.GetMap(mapId));
                    CompletionOption opt = new(mapId.ToString(), $" - Map {meta.Comp.EntityName}");
                    opts.Add(opt);
                }
                return CompletionResult.FromHintOptions(opts, "grid ID");
            case 2:
                return CompletionResult.FromOptions(Enum.GetNames<SecondSubCommand>());
            case 3:
                return GetSecondCompletion(args[1]);

        }
        return CompletionResult.Empty;
    }

    private static CompletionResult GetSecondCompletion(string secondArg)
    {
        return secondArg switch
        {
            nameof(SecondSubCommand.skip) => CompletionResult.FromHint("Кол-во пропускаемых секунд"),
            nameof(SecondSubCommand.next) => CompletionResult.FromHintOptions(Enum.GetNames<TimeOfDay>(), "Фаза дня"),
            _ => CompletionResult.Empty,
        };
    }
}
