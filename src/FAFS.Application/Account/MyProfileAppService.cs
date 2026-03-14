using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Account;
using Volo.Abp.Account.Settings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Settings;
using FAFS.Notifications;

namespace FAFS.Account
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IProfileAppService), typeof(ProfileAppService), typeof(MyProfileAppService))]
    public class MyProfileAppService : ProfileAppService
    {
        private readonly IRepository<AppNotification, Guid> _notificationRepository;

        public MyProfileAppService(
            IdentityUserManager userManager,
            IOptions<IdentityOptions> identityOptions,
            IRepository<AppNotification, Guid> notificationRepository) 
            : base(userManager, identityOptions)
        {
            _notificationRepository = notificationRepository;
        }

        public override async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            var result = await base.UpdateAsync(input);

            if (CurrentUser.Id.HasValue)
            {
                await _notificationRepository.InsertAsync(new AppNotification(
                    GuidGenerator.Create(),
                    CurrentUser.Id.Value,
                    "Datos de cuenta actualizados",
                    "Se han actualizado los datos personales o de seguridad de tu cuenta.",
                    "AccountUpdate"
                ));
            }

            return result;
        }

        public override async Task ChangePasswordAsync(ChangePasswordInput input)
        {
            await base.ChangePasswordAsync(input);

            if (CurrentUser.Id.HasValue)
            {
                await _notificationRepository.InsertAsync(new AppNotification(
                    GuidGenerator.Create(),
                    CurrentUser.Id.Value,
                    "Datos de cuenta actualizados",
                    "Se han actualizado los datos personales o de seguridad de tu cuenta.",
                    "AccountUpdate"
                ));
            }
        }
    }
}
