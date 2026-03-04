using Volo.Abp.Identity;
using Volo.Abp.Account;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace FAFS;

public static class FAFSDtoExtensions
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ObjectExtensionManager.Instance
                .AddOrUpdateProperty<ProfileDto, string>("FotoUrl")
                .AddOrUpdateProperty<UpdateProfileDto, string>("FotoUrl")
                .AddOrUpdateProperty<IdentityUserDto, string>("FotoUrl")
                .AddOrUpdateProperty<IdentityUserCreateDto, string>("FotoUrl")
                .AddOrUpdateProperty<IdentityUserUpdateDto, string>("FotoUrl");

            ObjectExtensionManager.Instance
                .AddOrUpdateProperty<ProfileDto, string>("Preferencias")
                .AddOrUpdateProperty<UpdateProfileDto, string>("Preferencias")
                .AddOrUpdateProperty<IdentityUserDto, string>("Preferencias")
                .AddOrUpdateProperty<IdentityUserCreateDto, string>("Preferencias")
                .AddOrUpdateProperty<IdentityUserUpdateDto, string>("Preferencias");
        });
    }
}
