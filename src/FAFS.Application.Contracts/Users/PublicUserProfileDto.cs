using System;
using Volo.Abp.Application.Dtos;

namespace FAFS.Users;

public class PublicUserProfileDto : EntityDto<Guid>
{
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string FotoUrl { get; set; }
}
