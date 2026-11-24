using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class CutiRemaining
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User? User { get; set; }

        public int RemainingDays { get; set; }
    }
}