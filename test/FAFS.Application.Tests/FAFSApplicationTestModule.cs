using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Identity;
using Volo.Abp.Account;

namespace FAFS;

[DependsOn(
    typeof(FAFSApplicationModule),
    typeof(FAFSDomainTestModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule)
)]
public class FAFSApplicationTestModule : AbpModule
{
}
