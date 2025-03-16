using System.Reflection;
using Assessment.Application.Common;
using Assessment.Application.DTOs;
using Assessment.Application.Interfaces;
using Assessment.Application.Services;
using Assessment.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assessment.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            var jwtSettings = new JwtSettings
            {
                Key = "SuperSecretKey123456!",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            _authService = new AuthService(_userRepositoryMock.Object, jwtSettings, _loggerMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new LoginRequestDTO("testuser@gmail.com", "wrongpassword");
            var user = new User { Email = "testuser@gmail.com", Password = BCrypt.Net.BCrypt.EnhancedHashPassword("correctpassword") };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(request.Email))
                               .ReturnsAsync(user);

            // Act
            var result = await _authService.AuthenticateAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid credentials.");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var request = new LoginRequestDTO("nonexistent", "password123");

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(request.Email))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid credentials.");
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequestDTO("newuser", "StrongPass123!");
            _userRepositoryMock.Setup(repo => repo.CheckIfUserExistsAsync(request.Email))
                               .ReturnsAsync(false);
            _userRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Success");
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenUserAlreadyExists()
        {
            // Arrange
            var request = new RegisterRequestDTO("existinguser", "Password123!");
            _userRepositoryMock.Setup(repo => repo.CheckIfUserExistsAsync(request.Email))
                               .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Username already taken.");
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenExceptionOccurs()
        {
            // Arrange
            var request = new RegisterRequestDTO("erroruser", "Password123!");
            _userRepositoryMock.Setup(repo => repo.CheckIfUserExistsAsync(request.Email))
                               .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("An error occurred while registering the user.");
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowException_WhenErrorOccurs()
        {
            // Arrange
            var user = new User { Email = "testuser", Role = "User" };
            var invalidJwtSettings = new JwtSettings
            {
                Key = null, // Invalid key to cause an error
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            var faultyAuthService = new AuthService(_userRepositoryMock.Object, invalidJwtSettings, _loggerMock.Object);

            // Act
            Action act = () => faultyAuthService.GetType()
                                                .GetMethod("GenerateJwtToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                                .Invoke(faultyAuthService, new object[] { user });

            // Assert
            act.Should().Throw<TargetInvocationException>()
               .WithInnerException<ApplicationException>()
               .WithMessage("Failed to generate authentication token.");
        }
    }
}