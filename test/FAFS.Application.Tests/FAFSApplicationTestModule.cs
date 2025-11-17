using Volo.Abp.Modularity;

namespace FAFS;

[DependsOn(
    typeof(FAFSApplicationModule),
    typeof(FAFSDomainTestModule)
)]
public class FAFSApplicationTestModule : AbpModule
{

}
