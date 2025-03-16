using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Assessment.Application.Common;
using Assessment.Application.DTOs;
using Assessment.Application.Interfaces;
using Assessment.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Assessment.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, JwtSettings jwtSettings, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }

        public async Task<BaseResponse<LoginResponseDTO>> AuthenticateAsync(LoginRequestDTO request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.Password))
                {
                    _logger.LogWarning($"Failed login attempt for username: {request.Email}");
                    return BaseResponse<LoginResponseDTO>.FailureResponse("Invalid credentials.", 401);
                }

                var token = GenerateJwtToken(user);
                return BaseResponse<LoginResponseDTO>.SuccessResponse(new LoginResponseDTO(token, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AuthenticateAsync: {ex.Message}");
                return BaseResponse<LoginResponseDTO>.FailureResponse("An error occurred while processing your request.", 500);
            }
        }

        public async Task<BaseResponse<string>> RegisterUserAsync(RegisterRequestDTO request)
        {
            try
            {
                if (await _userRepository.CheckIfUserExistsAsync(request.Email))
                {
                    _logger.LogWarning($"Attempted registration with existing username: {request.Email}");
                    return BaseResponse<string>.FailureResponse("Username already taken.", 400);
                }

                var newUser = new User
                {
                    Email = request.Email,
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password),
                    Role = "User"
                };

                await _userRepository.AddUserAsync(newUser);
                return BaseResponse<string>.SuccessResponse("User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RegisterUserAsync: {ex.Message}");
                return BaseResponse<string>.FailureResponse("An error occurred while registering the user.", 500);
            }
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Email),
                    new(ClaimTypes.Role, user.Role)
                };

                var token = new JwtSecurityToken(
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating JWT token: {ex.Message}");
                throw new ApplicationException("Failed to generate authentication token.");
            }
        }
    }
}
