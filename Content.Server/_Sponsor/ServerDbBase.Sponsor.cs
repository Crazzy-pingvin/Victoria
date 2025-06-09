using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.SponsorImplementations.Server;
using Content.SponsorImplementations.Shared;
using Microsoft.EntityFrameworkCore;
using Robust.Shared.Network;

namespace Content.Server.Database;

public partial interface IServerDbManager
{
    public Task<SponsorData?> GetSponsorData(NetUserId netUserId, CancellationToken cancel = default);
    public Task<List<SponsorData>> GetSponsorData(CancellationToken cancel = default);
    public Task SetSponsordata(NetUserId netUserId, ISponsorData? sponsorData, CancellationToken cancel = default);
}

public partial class ServerDbManager
{
    public Task<SponsorData?> GetSponsorData(NetUserId netUserId, CancellationToken cancel = default)
    {
        DbReadOpsMetric.Inc();
        return RunDbCommand(() => _db.GetSponsorData(netUserId, cancel));
    }

    public Task<List<SponsorData>> GetSponsorData(CancellationToken cancel = default)
    {
        DbReadOpsMetric.Inc();
        return RunDbCommand(() => _db.GetSponsorData(cancel));
    }

    public Task SetSponsordata(NetUserId netUserId, ISponsorData? sponsorData, CancellationToken cancel = default)
    {
        DbWriteOpsMetric.Inc();
        return RunDbCommand(() => _db.SetSponsorData(netUserId, sponsorData, cancel));
    }
}

public partial class ServerDbBase
{
    public async Task<SponsorData?> GetSponsorData(NetUserId netUserId, CancellationToken cancel = default)
    {
        await using var db = await GetDb(cancel);

        var data = db.DbContext.SponsorsList.FirstOrDefault(a => a.PlayerUserId == netUserId.UserId);
        if (data == null)
        {
            return null;
        }

        return new SponsorData()
        {
            Color = ColorHelper.TryParse(data),
            Guid = netUserId,
            ExtraCharSlots = data.ExtraCharSlots,
            ServerPriorityJoin = data.ServerPriorityJoin,
            Prototypes = db.DbContext.SponsorsPrototypes
                .Where(a => a.PlayerUserId == netUserId)
                .Select(a => a.Prototype)
                .ToList(),
        };
    }

    public async Task<List<SponsorData>> GetSponsorData(CancellationToken cancel = default)
    {
        await using var db = await GetDb(cancel);

        var data = db.DbContext.SponsorsList;

        return data.Select(sponsorDataRaw => new SponsorData()
        {
            Color = ColorHelper.TryParse(sponsorDataRaw),
            Guid = sponsorDataRaw.PlayerUserId,
            ExtraCharSlots = sponsorDataRaw.ExtraCharSlots,
            ServerPriorityJoin = sponsorDataRaw.ServerPriorityJoin,
            Prototypes = db.DbContext.SponsorsPrototypes
                .Where(a => a.PlayerUserId == sponsorDataRaw.PlayerUserId)
                .Select(a => a.Prototype)
                .ToList(),
        })
            .ToList();
    }

    public async Task SetSponsorData(NetUserId netUserId, ISponsorData? sponsorData, CancellationToken cancel)
    {
        await using var db = await GetDb(cancel);

        var data = db.DbContext.SponsorsList
            .FirstOrDefault(s => s.PlayerUserId == netUserId.UserId);

        db.DbContext.SponsorsPrototypes.RemoveRange(
            db.DbContext.SponsorsPrototypes.Where(p => p.PlayerUserId == netUserId.UserId)
            );

        if (data != null)
        {
            db.DbContext.SponsorsList.Remove(data);
        }

        if (sponsorData is null)
        {
            await db.DbContext.SaveChangesAsync(cancel);
            return;
        }

        db.DbContext.SponsorsList.Add(new ServerDbContext.SponsorDataRaw()
        {
            Color = sponsorData.Color?.ToHex(),
            PlayerUserId = netUserId,
            ExtraCharSlots = sponsorData.ExtraCharSlots,
            ServerPriorityJoin = sponsorData.ServerPriorityJoin,
        });

        db.DbContext.SponsorsPrototypes.AddRange(sponsorData.Prototypes.Select(d => new ServerDbContext.SponsorPrototypeData()
        {
            PlayerUserId = netUserId,
            Prototype = d,
        }));

        await db.DbContext.SaveChangesAsync(cancel);
    }
}


public static class ColorHelper
{
    public static Color? TryParse(ServerDbContext.SponsorDataRaw data)
    {
        Color? trueColor = null;

        if (data.Color != null && Color.TryParse(data.Color, out var parsedColor))
        {
            trueColor = parsedColor;
        }

        return trueColor;
    }
}
