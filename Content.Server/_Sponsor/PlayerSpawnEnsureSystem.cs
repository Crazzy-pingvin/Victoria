using Content.Corvax.Interfaces.Shared;
using Content.Shared.GameTicking;
using Content.SponsorImplementations.Shared;

namespace Content.Server._Sponsor;

public sealed class PlayerSpawnEnsureSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<PlayerBeforeSpawnEvent>(OnSpawn);
    }

    private void OnSpawn(PlayerBeforeSpawnEvent ev)
    {
        var sponsorData = new List<string>();
        if (IoCManager.Instance!.TryResolveType<ISharedSponsorsManager>(out var manager) &&
            manager.TryGetServerPrototypes(ev.Player.UserId, out var prototypes))
            sponsorData = prototypes;

        ev.Profile.EnsureValid(ev.Player, IoCManager.Instance, sponsorData.ToArray());
    }
}
