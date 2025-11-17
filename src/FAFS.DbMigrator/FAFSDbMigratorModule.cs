using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using FAFS.EntityFrameworkCore;


namespace FAFS.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FAFSEntityFrameworkCoreModule),
    typeof(FAFSApplicationContractsModule)
    )]
public class FAFSDbMigratorModule : AbpModule
{
}
