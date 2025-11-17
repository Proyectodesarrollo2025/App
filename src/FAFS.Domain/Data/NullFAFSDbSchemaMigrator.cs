using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FAFS.Data;

/* This is used if database provider does't define
 * IFAFSDbSchemaMigrator implementation.
 */
public class NullFAFSDbSchemaMigrator : IFAFSDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
