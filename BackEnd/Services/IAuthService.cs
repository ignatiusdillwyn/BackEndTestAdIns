using BackEnd.DTOs;
using BackEnd.Models;

namespace BackEnd.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
