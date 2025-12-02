namespace WorkForce360.API.DTOs
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string CheckInMethod { get; set; } = string.Empty;
        public string? CheckInLocation { get; set; }
        public string? CheckOutLocation { get; set; }
        public double? WorkingHours { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
    }

    public class CheckInDto
    {
        public string Method { get; set; } = "QR"; // QR or GPS
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }

    public class CheckOutDto
    {
        public int AttendanceId { get; set; }
        public string? Location { get; set; }
    }
}
