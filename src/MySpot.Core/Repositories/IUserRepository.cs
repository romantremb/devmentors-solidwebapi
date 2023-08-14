using MySpot.Core.Entities;

namespace MySpot.Core.Repositories;

public interface IUserRepository
{
    Task<User> GetById(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task AddAsync(User user);
}