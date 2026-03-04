using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace FAFS.EntityFrameworkCore;

public static class FAFSEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        FAFSGlobalFeatureConfigurator.Configure();
        FAFSModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string>(
                    "FotoUrl",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(512);
                    }
                )
                .MapEfCoreProperty<IdentityUser, string>(
                    "Preferencias",
                    (entityBuilder, propertyBuilder) =>
                    {
                        propertyBuilder.HasMaxLength(2048);
                    }
                );
        });
    }
}
