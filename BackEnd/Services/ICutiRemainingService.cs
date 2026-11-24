using BackEnd.DTOs;
using BackEnd.Models;

namespace BackEnd.Services
{
    public interface ICutiRemainingService
    {
        Task<CutiRemaining?> GetByEmployeeIdAsync(long employeeId);
    }
}
