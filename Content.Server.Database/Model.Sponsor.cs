using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database;
// This exist because i dont want to shit in main code you know
public partial class ServerDbContext
{
    public DbSet<SponsorDataRaw> SponsorsList { get; set; } = null!;
    public DbSet<SponsorPrototypeData> SponsorsPrototypes { get; set; } = null!;
    partial void OnCustomModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SponsorDataRaw>()
            .HasKey(e => e.PlayerUserId);

        modelBuilder.Entity<SponsorDataRaw>()
            .HasOne(w => w.Player)
            .WithOne(p => p.SponsorData)
            .HasForeignKey<SponsorDataRaw>(s => s.PlayerUserId)
            .HasPrincipalKey<Player>(s=> s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SponsorPrototypeData>()
            .HasOne(w => w.Player)
            .WithMany(a => a.Prototypes)
            .HasForeignKey(p => p.PlayerUserId)
            .HasPrincipalKey( s=> s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    [PrimaryKey(nameof(PlayerUserId))]
    public class SponsorDataRaw
    {
        public Guid PlayerUserId { get; set; }
        public Player Player { get; set; } = default!;

        [MaxLength(10)]
        public string? Color { get; set; }

        public int ExtraCharSlots { get; set; } = 0;

        public bool ServerPriorityJoin { get; set; } = false;
    }


    public class SponsorPrototypeData
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Prototype { get; set; } = string.Empty;

        public Guid PlayerUserId { get; set; }
        public Player Player { get; set; } = default!;
    }
}

public partial class Player
{
    public ServerDbContext.SponsorDataRaw? SponsorData { get; set; }
    public List<ServerDbContext.SponsorPrototypeData> Prototypes { get; set; } = null!;
}
