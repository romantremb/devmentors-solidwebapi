using Microsoft.EntityFrameworkCore;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;

namespace MySpot.Infrastructure.DAL.Repositories;

internal sealed class PostgresUserRepository : IUserRepository
{
    private readonly DbSet<User> _users;

    public PostgresUserRepository(MySpotDbContext dbContext)
    {
        _users = dbContext.Users;
    }

    public async Task<User> GetById(Guid id) 
        => await _users.FindAsync(new UserId(id));

    public async Task<User> GetByEmailAsync(string email) 
        => await _users.SingleOrDefaultAsync(x => x.Email == email);

    public async Task<User> GetByUsernameAsync(string username) 
        => await _users.SingleOrDefaultAsync(x => x.Username == username);

    public Task AddAsync(User user)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }
}