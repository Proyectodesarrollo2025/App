using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Volo.Abp;
using Volo.Abp.Data;

namespace FAFS.DbMigrator;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        // 🔹 Configuración de Serilog para logs en consola y archivo
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
            .MinimumLevel.Override("FAFS", LogEventLevel.Debug)
#else
            .MinimumLevel.Override("FAFS", LogEventLevel.Information)
#endif
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.Console())
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .CreateLogger();

        try
        {
            // ✅ 1. Cargar configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // ✅ 2. Crear aplicación ABP usando tu módulo migrador
            using var application = await AbpApplicationFactory.CreateAsync<FAFSDbMigratorModule>(options =>
            {
                options.UseAutofac();
                options.Services.AddLogging(builder => builder.AddSerilog());
                options.Services.ReplaceConfiguration(configuration);
            });

            // ✅ 3. Inicializar e inyectar el seeder
            await application.InitializeAsync();

            var dataSeeder = application.ServiceProvider.GetRequiredService<IDataSeeder>();
            await dataSeeder.SeedAsync();

            // ✅ 4. Cierre limpio
            await application.ShutdownAsync();
            return 0;
        }
        catch (System.Exception ex)
        {
            Log.Fatal(ex, "Migration failed!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

