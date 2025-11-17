using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FAFS.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class FAFSDbContextFactory : IDesignTimeDbContextFactory<FAFSDbContext>
{
    public FAFSDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        FAFSEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<FAFSDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new FAFSDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FAFS.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
