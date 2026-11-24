using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public RoleStatus Role { get; set; }

        public ICollection<Cuti> Cutis { get; set; } = new List<Cuti>();

        public CutiRemaining? CutiRemaining { get; set; }
    }
}