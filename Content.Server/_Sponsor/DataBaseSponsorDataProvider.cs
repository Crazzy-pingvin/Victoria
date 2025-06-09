using System.Threading.Tasks;
using Content.Server.Database;
using Content.SponsorImplementations.Server;
using Content.SponsorImplementations.Shared;
using Robust.Shared.Network;

namespace Content.Server._Sponsor;

public sealed class DataBaseSponsorDataProvider : ISponsorDataProvider
{
    [Dependency] private readonly IServerDbManager _dbManager = default!;

    private readonly ISawmill _logger;

    private readonly Dictionary<NetUserId, ISponsorData> _sponsorCache = new();
    private Task? _currentLoadTask;

    public DataBaseSponsorDataProvider()
    {
        IoCManager.InjectDependencies(this);
        _logger = Logger.GetSawmill("DataBaseSponsorDataProvider");
        _currentLoadTask = LoadSponsorCache();
    }

    public ISponsorData? GetSponsorInfo(NetUserId userId)
    {
        _currentLoadTask?.Wait();

        return _sponsorCache.TryGetValue(userId, out var sponsorData) ? sponsorData : null;
    }

    public Task SetSponsorInfo(NetUserId id, ISponsorData? data)
    {
        _currentLoadTask?.Wait();

        if (data != null)
            _sponsorCache[id] = data;
        else
            _sponsorCache.Remove(id);

        return _dbManager.SetSponsordata(id, data);
    }

    private async Task LoadSponsorCache()
    {
        _logger.Info("Caching sponsor data");
        _sponsorCache.Clear();
        var data = await _dbManager.GetSponsorData();
        foreach (var sponsorData in data)
        {
            _sponsorCache.Add(new NetUserId(sponsorData.Guid), sponsorData);
        }

        _currentLoadTask = null;
        _logger.Info($"Caching sponsor data completed. Precached: {_sponsorCache.Count}");
    }
}
