using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace FAFS;

// Base GENÉRICA (no la uses directamente en los tests)
public abstract class FAFSApplicationTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
}

// Base NO GENÉRICA (esta es la que heredan tus tests)
public abstract class FAFSApplicationTestBase : FAFSApplicationTestBase<FAFSApplicationTestModule>
{
}
