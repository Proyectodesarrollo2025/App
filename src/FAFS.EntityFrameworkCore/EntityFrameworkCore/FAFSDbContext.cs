using FAFS.Destinations;
using FAFS.Experiences;
using FAFS.Notifications;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Users;

namespace FAFS.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class FAFSDbContext : AbpDbContext<FAFSDbContext>, IIdentityDbContext
{
    private const string Schema = "Abp";

    public DbSet<Destination> Destinations { get; set; }
    public DbSet<DestinationRating> DestinationRatings { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<AppNotification> AppNotifications { get; set; }

    #region Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    #endregion

    public FAFSDbContext(DbContextOptions<FAFSDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        builder.Entity<Destination>(b =>
        {
            b.ToTable("Destination", Schema);
            b.ConfigureByConvention();
            b.Property(d => d.Name).IsRequired();
            b.Property(d => d.Country).IsRequired();
            b.Property(d => d.City).IsRequired();
            b.OwnsOne(d => d.Coordinates, c =>
            {
                c.Property(p => p.Latitude).HasColumnName("Latitude").IsRequired();
                c.Property(p => p.Longitude).HasColumnName("Longitude").IsRequired();
            });
        });

        builder.Entity<DestinationRating>(b =>
        {
            b.ToTable("DestinationRatings", Schema);
            b.ConfigureByConvention();
            b.Property(x => x.Score).IsRequired();
            b.Property(x => x.Comment).HasMaxLength(1000);
            b.HasIndex(x => new { x.UserId, x.DestinationId }).IsUnique(false);
        });

        builder.Entity<Experience>(b =>
        {
            b.ToTable("Experiences", Schema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).IsRequired().HasMaxLength(ExperienceConsts.MaxTitleLength);
            b.Property(x => x.Description).IsRequired().HasMaxLength(ExperienceConsts.MaxDescriptionLength);
            b.Property(x => x.Rating).IsRequired();
            b.HasOne<Destination>().WithMany().HasForeignKey(x => x.DestinationId).IsRequired();
            b.HasIndex(x => x.DestinationId);
        });

        builder.Entity<AppNotification>(b =>
        {
            b.ToTable("AppNotifications", Schema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).IsRequired().HasMaxLength(256);
            b.Property(x => x.Message).IsRequired().HasMaxLength(1024);
            b.Property(x => x.Type).IsRequired().HasMaxLength(64);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.IsRead);
        });
    }
}
