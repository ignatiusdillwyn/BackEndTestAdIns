using BackEnd.Models;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs
{
    public class CutiRequest
    {

        public long UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [MinLength(10, ErrorMessage = "Reason Minimal 10 Karakter")]
        [MaxLength(500, ErrorMessage = "Reason Maksmial 500 Karakter")]
        public string? Reason { get; set; } = string.Empty;

        public CutiStatus Status { get; set; }
    }
}
