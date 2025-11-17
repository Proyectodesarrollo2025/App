using System.Threading.Tasks;

namespace FAFS.Data;

public interface IFAFSDbSchemaMigrator
{
    Task MigrateAsync();
}
