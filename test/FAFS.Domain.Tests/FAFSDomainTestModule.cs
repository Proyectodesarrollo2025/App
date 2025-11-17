using Volo.Abp.Modularity;

namespace FAFS;

[DependsOn(
    typeof(FAFSDomainModule),
    typeof(FAFSTestBaseModule)
)]
public class FAFSDomainTestModule : AbpModule
{

}
