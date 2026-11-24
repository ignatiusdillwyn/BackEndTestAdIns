using BackEnd.DTOs;
using BackEnd.Models;
namespace BackEnd.Services
{
    public interface ICutiService
    {
        Task<Cuti?> GetByIdAsync(long cutiId);
        Task<List<Cuti?>> GetAllCutiAsync();
        Task<List<Cuti?>> GetAllCutiByEmployeeIdAsync(long employeeId);
        Task<CutiResponse?> CreateAsync(long employeeId, CutiRequest request);
        Task<CutiResponse?> UpdateAsync(long cutiId, CutiRequest request);
        Task<bool> DeleteAsync(long cutiId, long employeeId);
        CutiResponse ToCutiResponse(Cuti cuti);
    }
}
