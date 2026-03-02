using HomeStore.Domain.Entities;
using HomeStore.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HomeStore.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HomeStoreV2Context _context;

    public UserRepository(HomeStoreV2Context context) => _context = context;

    public async Task<User?> GetByIdAsync(int userId)
        => await _context.Users.FindAsync(userId);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<List<User>> GetAllAsync()
        => await _context.Users.OrderBy(u => u.UserId).ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
