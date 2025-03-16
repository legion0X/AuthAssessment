using Assessment.Application.Common;
using Assessment.Application.DTOs;
using Assessment.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="request">The login request DTO.</param>
        /// <returns>JWT Token and success message.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO request)
        {
            return await _authService.AuthenticateAsync(request);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration request DTO.</param>
        /// <returns>Success or failure message.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<BaseResponse<string>>> Register([FromBody] RegisterRequestDTO request)
        {
            return await _authService.RegisterUserAsync(request);
        }
    }
}
