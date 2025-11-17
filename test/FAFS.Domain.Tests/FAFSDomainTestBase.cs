using Volo.Abp.Modularity;

namespace FAFS;

/* Inherit from this class for your domain layer tests. */
public abstract class FAFSDomainTestBase<TStartupModule> : FAFSTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
