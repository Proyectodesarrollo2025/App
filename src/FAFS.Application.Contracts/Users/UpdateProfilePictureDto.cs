using System.ComponentModel.DataAnnotations;

namespace FAFS.Users;

public class UpdateProfilePictureDto
{
    [Required]
    [DataType(DataType.Text)]
    public string FotoUrl { get; set; }
}
