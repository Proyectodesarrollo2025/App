using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Users;

public interface IUserProfileAppService : IApplicationService
{
    Task<PublicUserProfileDto> GetPublicProfileAsync(Guid id);
    
    Task<PublicUserProfileDto> GetMyProfileAsync();

    // Agregamos esto para completar el requerimiento 1.5 si no existe en la infraestructura base
    Task DeleteMyAccountAsync();

    Task UpdateProfilePictureAsync(UpdateProfilePictureDto input);
}
