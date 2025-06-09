using Robust.Shared.Network;

namespace Content.SponsorImplementations.Server;

public interface ISponsorRecordProvider
{
    public void RegisterSponsorDataProvider(ISponsorDataProvider sponsorDataProvider);
    public T GetSponsorDataProvider<T>() where T : ISponsorDataProvider;
    public void SendSponsorDataToClient(NetUserId id);
}

