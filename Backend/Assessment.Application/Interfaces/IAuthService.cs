using Assessment.Application.Common;
using Assessment.Application.DTOs;

namespace Assessment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponseDTO>> AuthenticateAsync(LoginRequestDTO request);
        Task<BaseResponse<string>> RegisterUserAsync(RegisterRequestDTO request);
    }
}
