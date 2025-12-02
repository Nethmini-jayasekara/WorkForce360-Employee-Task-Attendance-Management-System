using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkForce360.API.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [StringLength(50)]
        public string CheckInMethod { get; set; } = "QR"; // QR or GPS

        [StringLength(200)]
        public string? CheckInLocation { get; set; }

        [StringLength(200)]
        public string? CheckOutLocation { get; set; }

        public double? WorkingHours { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Present"; // Present, Late, Absent

        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
