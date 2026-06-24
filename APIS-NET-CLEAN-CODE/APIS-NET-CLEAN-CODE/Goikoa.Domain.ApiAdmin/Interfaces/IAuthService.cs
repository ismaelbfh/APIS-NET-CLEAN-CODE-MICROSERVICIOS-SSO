using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;

namespace Goikoa.Domain.ApiAdmin.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> RegisterAsync(UserRequest userDto);
        Task<TokenResponseDTO> LoginAsync(UserRequest loginDto);
        Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken);
    }
}
