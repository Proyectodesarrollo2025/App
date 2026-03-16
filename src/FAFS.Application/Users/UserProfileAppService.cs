using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FAFS.Users;

public class UserProfileAppService : FAFSAppService, IUserProfileAppService
{
    protected IdentityUserManager UserManager { get; }
    protected IIdentityUserRepository UserRepository { get; }

    public UserProfileAppService(
        IdentityUserManager userManager,
        IIdentityUserRepository userRepository)
    {
        UserManager = userManager;
        UserRepository = userRepository;
    }

    public virtual async Task<PublicUserProfileDto> GetPublicProfileAsync(Guid id)
    {
        var user = await UserManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new UserFriendlyException("Usuario no encontrado");
        }

        return new PublicUserProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            FotoUrl = user.GetProperty<string>("FotoUrl")
        };
    }

    [Authorize]
    public virtual async Task<PublicUserProfileDto> GetMyProfileAsync()
    {
        var userId = CurrentUser.GetId();
        return await GetPublicProfileAsync(userId);
    }

    [Authorize]
    public virtual async Task DeleteMyAccountAsync()
    {
        var userId = CurrentUser.GetId();
        var user = await UserManager.FindByIdAsync(userId.ToString());
        
        if (user == null)
        {
            throw new UserFriendlyException("Usuario no encontrado");
        }

        var result = await UserManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new UserFriendlyException("Error al eliminar la cuenta: " + string.Join(", ", result.Errors));
        }
    }

    [Authorize]
    public virtual async Task UpdateProfilePictureAsync(UpdateProfilePictureDto input)
    {
        try
        {
            var userId = CurrentUser.GetId();
            var user = await UserManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new UserFriendlyException("Usuario no encontrado");
            }

            if (string.IsNullOrEmpty(input.FotoUrl))
            {
                throw new UserFriendlyException("La imagen está vacía");
            }

            user.SetProperty("FotoUrl", input.FotoUrl);
            
            // 🔹 Guardado persistente
            await UserRepository.UpdateAsync(user, autoSave: true);
        }
        catch (Exception ex) when (!(ex is UserFriendlyException))
        {
            throw new UserFriendlyException("Error interno al procesar la imagen: " + ex.Message);
        }
    }
}
