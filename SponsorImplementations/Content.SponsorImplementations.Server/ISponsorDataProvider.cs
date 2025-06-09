using Content.SponsorImplementations.Shared;
using Robust.Shared.Network;

namespace Content.SponsorImplementations.Server;

public interface ISponsorDataProvider
{
    public ISponsorData? GetSponsorInfo(NetUserId userId);
}
