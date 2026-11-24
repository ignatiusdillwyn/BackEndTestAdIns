using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BackEnd.Services
{
    public class CutiService : ICutiService
    {
        private readonly IRepository<Cuti> _cutiRepository;
        private readonly ILogger<CutiService> _logger;

        public CutiService(
            IRepository<Cuti> cutiRepository,
            ILogger<CutiService> logger)
        {
            _cutiRepository = cutiRepository;
            _logger = logger;
        }

        public async Task<CutiResponse?> CreateAsync(long employeeId, CutiRequest request)
        {
            await _cutiRepository.BeginTransactionAsync();
            try
            {
                var startDate = ToUtc(request.StartDate);
                var endDate = ToUtc(request.EndDate);

                if (endDate.Date < startDate.Date)
                {
                    throw new ValidationException("Tanggal selesai tidak boleh lebih awal dari tanggal mulai.");
                }

                if (startDate.Date < DateTime.UtcNow.Date)
                {
                    throw new ValidationException("Tanggal mulai tidak boleh di masa lalu.");
                }

                var existingCutis = await _cutiRepository.GetAllAsync();
                bool isOverlap = existingCutis.Any(c =>
                    c.UserId == employeeId &&
                    c.Status != CutiStatus.Rejected &&
                    startDate.Date <= c.EndDate.Date &&
                    endDate.Date >= c.StartDate.Date);

                if (isOverlap)
                {
                    throw new ValidationException("Pengajuan cuti bentrok dengan cuti yang sudah ada.");
                }

                var cuti = new Cuti
                {
                    UserId = employeeId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Reason = request.Reason.Trim(),
                    Status = CutiStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _cutiRepository.AddAsync(cuti);
                await _cutiRepository.SaveChangesAsync();
                await _cutiRepository.CommitTransactionAsync();

                var created = await GetByIdAsync(cuti.Id);
                return created == null ? null : ToCutiResponse(created);
            } catch (Exception ex)
            {
                await _cutiRepository.RollbackTransactionAsync();
                throw ex;
            }
        }

        private static DateTime ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        public async Task<CutiResponse?> UpdateAsync(long cutiId, CutiRequest request)
        {
            await _cutiRepository.BeginTransactionAsync();
            try
            {
                var cuti = await _cutiRepository.GetByIdAsync(cutiId, "User");

                if (cuti == null)
                {
                    await _cutiRepository.RollbackTransactionAsync();
                    return null; 
                }

                if (cuti.Status != CutiStatus.Pending)
                {
                    throw new ValidationException("Hanya cuti status Pending yang dapat diubah.");
                }
                
                if (request.Status != null)
                {
                    cuti.Status = request.Status;
                }

                _cutiRepository.Update(cuti);
                await _cutiRepository.SaveChangesAsync();
                await _cutiRepository.CommitTransactionAsync();
                return ToCutiResponse(cuti);
            }
            catch (Exception ex)
            {
                await _cutiRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<List<Cuti>> GetAllCutiAsync()
        {
            return await _cutiRepository.GetAllAsync();
        }

        public async Task<List<Cuti>?> GetAllCutiByEmployeeIdAsync(long employeeId)
        {
            List<Cuti> listCutis = await _cutiRepository.GetAllAsync("User");
            listCutis = listCutis.Where(lc => lc.UserId == employeeId).ToList();
            if (listCutis.Count == 0) { 
                return null;
            }
            return listCutis;
        }

        public async Task<Cuti?> GetByIdAsync(long cutiId)
        {
            List<Cuti> listCutis = await _cutiRepository.GetAllAsync("User");
            Cuti cuti = listCutis.Where(lc => lc.Id == cutiId).ToList().FirstOrDefault();

            if (cuti == null)
            {
                return null;
            }
            return cuti;
        }

        public async Task<bool> DeleteAsync(long cutiId, long employeeId)
        {
            await _cutiRepository.BeginTransactionAsync();
            try
            {
                var cuti = await _cutiRepository.GetByIdAsync(cutiId);

                if (cuti == null)
                {
                    await _cutiRepository.RollbackTransactionAsync();
                    return false; 
                }

                if (cuti.UserId != employeeId)
                {
                    throw new UnauthorizedAccessException("Anda tidak memiliki hak menghapus pengajuan cuti ini.");
                }

                if (cuti.Status != CutiStatus.Pending)
                {
                    throw new ValidationException("Hanya cuti berstatus Pending yang dapat dihapus.");
                }

                _cutiRepository.Delete(cuti);
                await _cutiRepository.SaveChangesAsync();
                await _cutiRepository.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _cutiRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public CutiResponse ToCutiResponse(Cuti cuti)
        {
            return new CutiResponse
            {
                Id = cuti.Id,
                UserId = cuti.UserId,
                UserName = cuti?.User?.Username,
                FullName = cuti?.User?.FullName,
                StartDate = (DateTime)(cuti?.StartDate),
                EndDate = (DateTime)(cuti?.EndDate),
                Status = cuti.Status,
                Reason = cuti.Reason,
                CreatedAt = (DateTime)(cuti?.CreatedAt),
            };
        }
    }
}
