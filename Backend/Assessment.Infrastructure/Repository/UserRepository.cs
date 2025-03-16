using Assessment.Application.Interfaces;
using Assessment.Domain.Models;
using Assessment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assessment.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(user => user.Email.Equals(email));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database error in GetByUsernameAsync: {ex.Message}");
                throw new ApplicationException("Failed to retrieve user from the database.");
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database error while saving user: {ex.Message}");
                throw new ApplicationException("Could not save user to database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in AddUserAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred while adding the user.");
            }
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database error in GetByIdAsync: {ex.Message}");
                throw new ApplicationException("Failed to retrieve user by ID.");
            }
        }

        public async Task<bool> CheckIfUserExistsAsync(string email)
        {
            try
            {
                return await _context.Users.AnyAsync(user => user.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database error in CheckIfUserExistsAsync: {ex.Message}");
                throw new ApplicationException("Failed to check if user exists.");
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database error while updating user: {ex.Message}");
                throw new ApplicationException("Failed to update user.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error in UpdateUserAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred while updating the user.");
            }
        }
    }
}
