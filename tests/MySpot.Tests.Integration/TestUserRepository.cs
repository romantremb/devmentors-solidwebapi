using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Tests.Integration;

internal sealed class TestUserRepository : IUserRepository
{
    private readonly List<User> _users = new(); 
    
    public async Task<User> GetById(Guid id) 
        => _users.SingleOrDefault(x => x.Id == new UserId(id));

    public async Task<User> GetByEmailAsync(string email)
        => _users.SingleOrDefault(x => x.Email == new Email(email));

    public async Task<User> GetByUsernameAsync(string username)
        => _users.SingleOrDefault(x => x.Username == new Username(username));

    public async Task AddAsync(User user)
        => _users.Add(user);
}