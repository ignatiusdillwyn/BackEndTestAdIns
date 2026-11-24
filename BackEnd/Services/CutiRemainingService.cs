using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BackEnd.Services
{
    public class CutiRemainingService : ICutiRemainingService
    {
        private readonly IRepository<CutiRemaining> _cutiRemainingRepository;
        private readonly ILogger<CutiRemainingService> _logger;

        public CutiRemainingService(
            IRepository<CutiRemaining> cutiRemainingRepository,
            ILogger<CutiRemainingService> logger)
        {
            _cutiRemainingRepository = cutiRemainingRepository;
            _logger = logger;
        }
        public async Task<CutiRemaining?> GetByEmployeeIdAsync(long employeeId)
        {
            CutiRemaining cutiRemaining =  await _cutiRemainingRepository.GetByIdAsync(employeeId);
            if (cutiRemaining == null) return null;
            return cutiRemaining;
        }
    }
}
