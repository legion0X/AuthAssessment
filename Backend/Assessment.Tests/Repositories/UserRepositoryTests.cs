using Assessment.Domain.Models;
using Assessment.Infrastructure.Data;
using Assessment.Infrastructure.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new ApplicationDbContext(options);

        _context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "testuser@example.com",
            Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password123"),
        });
        _context.SaveChanges();

        var loggerMock = new Mock<ILogger<UserRepository>>();
        _userRepository = new UserRepository(_context, loggerMock.Object);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Act
        var user = await _userRepository.GetByEmailAsync("testuser@example.com");

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be("testuser@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var user = await _userRepository.GetByEmailAsync("notfound@example.com");

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUserSuccessfully()
    {
        // Arrange
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "newuser@example.com",
            Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password123"),
        };

        // Act
        await _userRepository.AddUserAsync(newUser);
        var user = await _userRepository.GetByEmailAsync("newuser@example.com");

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be("newuser@example.com");
    }

    [Fact]
    public async Task CheckIfUserExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Act
        var exists = await _userRepository.CheckIfUserExistsAsync("testuser@example.com");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task CheckIfUserExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Act
        var exists = await _userRepository.CheckIfUserExistsAsync("notfound@example.com");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUserSuccessfully()
    {
        // Arrange
        var user = await _userRepository.GetByEmailAsync("testuser@example.com");
        user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword("NewPassword123");

        // Act
        await _userRepository.UpdateUserAsync(user);
        var updatedUser = await _userRepository.GetByEmailAsync("testuser@example.com");

        // Assert
        updatedUser.Password.Should().NotBeNull();
        updatedUser.Password.Should().NotBe("Password123");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var existingUser = await _userRepository.GetByEmailAsync("testuser@example.com");

        // Act
        var user = await _userRepository.GetByIdAsync(existingUser.Id);

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be("testuser@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var user = await _userRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "fakeuser@example.com",
            Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password123"),
        };

        // Act
        Func<Task> act = async () => await _userRepository.UpdateUserAsync(nonExistentUser);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to update user.");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}