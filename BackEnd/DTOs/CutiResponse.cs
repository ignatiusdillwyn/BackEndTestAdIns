using BackEnd.Models;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs
{
    public class CutiResponse
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Reason { get; set; } = string.Empty;

        public CutiStatus Status { get; set; } = new CutiStatus();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
