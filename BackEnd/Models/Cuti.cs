using BackEnd.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public class Cuti
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(500)] 
        public string Reason { get; set; } = string.Empty ;

        [Required]
        [MaxLength(20)]
        public CutiStatus Status { get; set; } = CutiStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
