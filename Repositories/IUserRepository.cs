using AuthenPractice.Models.Entities;

namespace AuthenPractice.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<User> GetByIdAsync(Guid id);
    }
}
