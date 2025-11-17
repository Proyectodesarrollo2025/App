using FAFS.Destinations;
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
using FAFS; // para que reconozca IUserOwned y DestinationRating

namespace FAFS.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class FAFSDbContext : AbpDbContext<FAFSDbContext>, IIdentityDbContext
{
    private const string Schema = "Abp";
    private readonly ICurrentUser? _currentUser;

    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Destination> Destinations { get; set; }
    public DbSet<DestinationRating> DestinationRatings { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext 
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    public FAFSDbContext(DbContextOptions<FAFSDbContext> options) // EF Core constructor
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(FAFSConsts.DbTablePrefix + "YourEntities", FAFSConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        builder.Entity<Destination>(b =>
        {
            b.ToTable("Destination", Schema);
            b.ConfigureByConvention(); // Id, propiedades de auditoría, etc.

            // Propiedades escalares obligatorias
            b.Property(d => d.Name).IsRequired();
            b.Property(d => d.Country).IsRequired();
            b.Property(d => d.City).IsRequired();

            // Propiedades escalares opcionales
            b.Property(d => d.PhotoUrl).HasMaxLength(500); // opcional: limitar tamaño
            b.Property(d => d.LastUpdated);

            // 🔹 Value Object Coordinates
            b.OwnsOne(d => d.Coordinates, c =>
            {
                c.Property(p => p.Latitude)
                 .HasColumnName("Latitude")
                 .IsRequired();

                c.Property(p => p.Longitude)
                 .HasColumnName("Longitude")
                 .IsRequired();
            });

            builder.Entity<DestinationRating>(b =>
            {
                b.ToTable("DestinationRatings", Schema);
                b.ConfigureByConvention();

                b.Property(x => x.Score).IsRequired();
                b.Property(x => x.Comment).HasMaxLength(1000);

                b.HasIndex(x => new { x.UserId, x.DestinationId }).IsUnique(false);

                // 🔹 Filtro automático por usuario autenticado
                b.HasQueryFilter(rating =>
                    _currentUser == null ||
                    !_currentUser.IsAuthenticated ||
                    rating.UserId == _currentUser.GetId());
            });
        });
     }   
}
