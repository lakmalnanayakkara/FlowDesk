using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Auth;

namespace FlowDesk.Core.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
}