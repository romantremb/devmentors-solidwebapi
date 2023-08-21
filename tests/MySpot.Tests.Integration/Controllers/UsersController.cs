using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Application.Commands;
using MySpot.Application.DTO;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Infrastructure.Security;
using MySpot.Infrastructure.Time;
using Shouldly;

namespace MySpot.Tests.Integration.Controllers;

public class UsersController : ControllerTests, IDisposable
{
    [Fact]
    public async Task post_users_should_return_created_201_status_code()
    {
        await _testDatabase.Context.Database.MigrateAsync();
        
        var command = new SignUp(
            Guid.Empty,
            "test-user@myspot.io",
            "test-user",
            "secret",
            "John Doe",
            "user"
        );

        
        var response = await Client.PostAsJsonAsync("users", command);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task post_user_sign_in_should_return_ok_200_status_code_and_jwt()
    {
        var passwordManager = new PasswordManager(new PasswordHasher<User>());
        var clock = new Clock();
        const string password = "secret";
        var user = new User(
            Guid.NewGuid(),
            "test-user@myspot.io",
            "test-user",
            passwordManager.Secure(password),
            "John Doe",
            "user",
            clock.Current()
        );

        await _userRepository.AddAsync(user);
        // await _testDatabase.Context.Database.MigrateAsync();
        // _testDatabase.Context.Users.Add(user);
        // await _testDatabase.Context.SaveChangesAsync();

        var command = new SignIn(user.Email, password);
        var response = await Client.PostAsJsonAsync("users/sign-in", command);
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();

        jwt.ShouldNotBeNull();
        jwt.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task get_users_me_should_return_ok_200_status_code_and_user()
    {
        var passwordManager = new PasswordManager(new PasswordHasher<User>());
        var clock = new Clock();
        const string password = "secret";
        var user = new User(
            Guid.NewGuid(),
            "test-user@myspot.io",
            "test-user",
            passwordManager.Secure(password),
            "John Doe",
            "user",
            clock.Current()
        );
        
        await _testDatabase.Context.Database.MigrateAsync();
        _testDatabase.Context.Users.Add(user);
        await _testDatabase.Context.SaveChangesAsync();

        Authorize(user.Id, user.Role);
        var userDto = await Client.GetFromJsonAsync<UserDto>("Users/me");

        userDto.ShouldNotBeNull();
        userDto.Id.ShouldBe(user.Id.Value);
        userDto.Fullname.ShouldBe(user.FullName.Value);
        userDto.Username.ShouldBe(user.Username.Value);
    }

    private TestUserRepository _userRepository;
    private readonly TestDatabase _testDatabase;

    public UsersController(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDatabase = new TestDatabase();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        _userRepository = new TestUserRepository();
        services.AddSingleton<IUserRepository>(_userRepository);
    }

    public void Dispose()
    {
        _testDatabase.Dispose();
    }
}