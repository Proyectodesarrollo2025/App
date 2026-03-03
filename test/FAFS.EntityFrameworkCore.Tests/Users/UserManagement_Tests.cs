using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Xunit;
using FAFS.EntityFrameworkCore;

namespace FAFS.Users;

public class UserManagement_Tests : FAFSEntityFrameworkCoreTestBase
{
    private readonly IAccountAppService _accountAppService;
    private readonly IProfileAppService _profileAppService;
    private readonly IUserProfileAppService _userProfileAppService;
    private readonly IdentityUserManager _userManager;
    private readonly ICurrentUser _currentUser;

    public UserManagement_Tests()
    {
        _userManager = GetRequiredService<IdentityUserManager>();
        _userProfileAppService = GetRequiredService<IUserProfileAppService>();
        _accountAppService = GetRequiredService<IAccountAppService>();
        _profileAppService = GetRequiredService<IProfileAppService>();
        _currentUser = GetRequiredService<ICurrentUser>();
    }

    [Fact]
    public async Task Should_Register_A_New_User()
    {
        // Arrange
        var input = new RegisterDto
        {
            UserName = "testuser",
            EmailAddress = "testuser@fafs.com",
            Password = "Password123!",
            AppName = "FAFS"
        };

        // Act
        var user = await _accountAppService.RegisterAsync(input);

        // Assert
        user.ShouldNotBeNull();
        user.UserName.ShouldBe(input.UserName);
        user.Email.ShouldBe(input.EmailAddress);
        
        var dbUser = await _userManager.FindByNameAsync(input.UserName);
        dbUser.ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_Update_Profile_Data()
    {
        // 1. Crear un usuario
        var user = new IdentityUser(Guid.NewGuid(), "profileuser", "profileuser@fafs.com");
        (await _userManager.CreateAsync(user, "Password123!")).Succeeded.ShouldBeTrue();

        // 2. Simular logged in user
        var fakeCurrentUser = (FakeCurrentUser)_currentUser;
        fakeCurrentUser.Id = user.Id;
        fakeCurrentUser.UserName = user.UserName;

        try
        {
            var updateInput = new UpdateProfileDto
            {
                UserName = "profileuser_updated",
                Email = "profileuser_updated@fafs.com",
                Name = "John",
                Surname = "Doe"
            };

            // Act
            await _profileAppService.UpdateAsync(updateInput);

            // Assert
            var updatedUser = await _userManager.FindByIdAsync(user.Id.ToString());
            updatedUser.UserName.ShouldBe(updateInput.UserName);
            updatedUser.Name.ShouldBe(updateInput.Name);
            updatedUser.Surname.ShouldBe(updateInput.Surname);
        }
        finally
        {
            fakeCurrentUser.Id = null;
        }
    }

    [Fact]
    public async Task Should_Change_Password()
    {
        // 1. Registrar un usuario a través del servicio
        var registrationInput = new RegisterDto
        {
            UserName = "pwduser",
            EmailAddress = "pwduser@fafs.com",
            Password = "OldPassword123!",
            AppName = "FAFS"
        };
        var registeredUser = await _accountAppService.RegisterAsync(registrationInput);

        var fakeCurrentUser = (FakeCurrentUser)_currentUser;
        fakeCurrentUser.Id = registeredUser.Id;
        fakeCurrentUser.UserName = registeredUser.UserName;

        try
        {
            var changePasswordInput = new ChangePasswordInput
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            // Act
            await _profileAppService.ChangePasswordAsync(changePasswordInput);

            // Assert
            var dbUser = await _userManager.FindByIdAsync(registeredUser.Id.ToString());
            var check = await _userManager.CheckPasswordAsync(dbUser, "NewPassword123!");
            check.ShouldBeTrue();
        }
        catch (Exception ex)
        {
            throw new Exception($"ProfileAppService.ChangePasswordAsync failed. Exception: {ex.Message}", ex);
        }
        finally
        {
            fakeCurrentUser.Id = null;
        }
    }

    [Fact]
    public async Task Should_Delete_Own_Account()
    {
        // Arrange
        var user = new IdentityUser(Guid.NewGuid(), "deleteuser", "deleteuser@fafs.com");
        (await _userManager.CreateAsync(user, "Password123!")).Succeeded.ShouldBeTrue();

        var fakeCurrentUser = (FakeCurrentUser)_currentUser;
        fakeCurrentUser.Id = user.Id;

        try
        {
            // Act
            await _userProfileAppService.DeleteMyAccountAsync();

            // Assert
            var deletedUser = await _userManager.FindByIdAsync(user.Id.ToString());
            deletedUser.ShouldBeNull();
        }
        finally
        {
            fakeCurrentUser.Id = null;
        }
    }

    [Fact]
    public async Task Should_Get_Public_Profile()
    {
        // Arrange
        var user = new IdentityUser(Guid.NewGuid(), "publicuser", "publicuser@fafs.com")
        {
            Name = "Public",
            Surname = "User"
        };
        (await _userManager.CreateAsync(user, "Password123!")).Succeeded.ShouldBeTrue();

        // Act
        var profile = await _userProfileAppService.GetPublicProfileAsync(user.Id);

        // Assert
        profile.ShouldNotBeNull();
        profile.UserName.ShouldBe(user.UserName);
        profile.Name.ShouldBe(user.Name);
        profile.Surname.ShouldBe(user.Surname);
    }
}
