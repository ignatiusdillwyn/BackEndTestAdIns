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
    [Route("api/cuti-remainings")]
    [Authorize]
    public class CutiRemainingController : Controller
    {
        private readonly ICutiRemainingService _cutiRemainingService;

        public CutiRemainingController(ICutiRemainingService cutiRemainingService)
        {
            _cutiRemainingService = cutiRemainingService;
        }

        [HttpGet("getCutiRemainingByEmployeeId")]
        public async Task<IActionResult> GetAllCutiRemainingByEmployeeId()
        {
            long userId = GetUserId();
            if (userId == null) return Unauthorized();

            CutiRemaining cutiRemaining = await _cutiRemainingService.GetByEmployeeIdAsync(userId);
            return Ok(cutiRemaining);
        }

        private long GetUserId()
        {
            long userId = long.Parse(User.FindFirst("userId")!.Value);
            return userId;
        }
    }
}
