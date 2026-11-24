using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/cutis")]
    [Authorize]
    public class CutiController : ControllerBase
    {
        private readonly ICutiService _cutiService;

        public CutiController(ICutiService cutiService)
        {
            _cutiService = cutiService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            List<Cuti> cutiList = await _cutiService.GetAllCutiAsync();
            List<CutiResponse> response = cutiList.Select(_cutiService.ToCutiResponse).ToList();
            return Ok(response);
        }

        [HttpGet("getAllCutiByEmployeeId")]
        public async Task<IActionResult> GetAllCutiByEmployeeId()
        {
            long userId = GetUserId();
            if (userId == null) return Unauthorized();

            List<Cuti> cutiList = await _cutiService.GetAllCutiByEmployeeIdAsync(userId);
            List<CutiResponse> response = cutiList.Select(_cutiService.ToCutiResponse).ToList();
            return Ok(response);
        }

        /*[HttpGet("{cutiId:long}")]
        public async Task<IActionResult> GetById(long cutiId)
        {
            long userId = GetUserId();
            if (userId == null) return Unauthorized();

            Cuti cuti = await _cutiService.GetByIdAsync(cutiId);
            if (cuti == null) return NotFound(new { message = "Cuti tidak ditemukan." });

            if (!User.IsInRole("Manager") && cuti.UserId != userId)
                return Forbid();

            CutiResponse response = _cutiService.ToCutiResponse(cuti);

            return Ok(response);
        }*/

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CutiRequest request)
        {
            long userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _cutiService.CreateAsync(userId, request);
                if (result == null)
                    return StatusCode(500, new { message = "Gagal membuat cuti." });

                return Created($"/api/cutis/{result.Id}", result);
            }
            catch (ValidationException ex)
            {
                throw;
            }
        }

        [HttpPut("{cutiId:long}")]
        public async Task<IActionResult> Update(long cutiId, [FromBody] CutiRequest request)
        {
            long userId = GetUserId();
            if (userId == null) return Unauthorized();

            request.UserId = userId;

            try
            {
                var result = await _cutiService.UpdateAsync(cutiId, request);
                if (result == null)
                    return NotFound(new { message = "Cuti tidak ditemukan." });

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("{cutiId:long}")]
        public async Task<IActionResult> Delete(long cutiId)
        {
            long userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var deleted = await _cutiService.DeleteAsync(cutiId, userId);
                if (!deleted)
                    return NotFound(new { message = "Cuti tidak ditemukan." });

                return NoContent();
            }
            catch (ValidationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private long GetUserId()
        {
            long userId = long.Parse(User.FindFirst("userId")!.Value);
            return userId;
        }


    }
}