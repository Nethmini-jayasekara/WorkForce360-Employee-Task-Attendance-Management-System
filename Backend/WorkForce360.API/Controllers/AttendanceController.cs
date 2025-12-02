using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkForce360.API.Data;
using WorkForce360.API.DTOs;
using WorkForce360.API.Models;

namespace WorkForce360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAllAttendance([FromQuery] DateTime? date)
        {
            var query = _context.Attendances.Include(a => a.User).AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(a => a.Date.Date == date.Value.Date);
            }

            var attendances = await query
                .OrderByDescending(a => a.CheckInTime)
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.User.FullName,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    CheckInMethod = a.CheckInMethod,
                    CheckInLocation = a.CheckInLocation,
                    CheckOutLocation = a.CheckOutLocation,
                    WorkingHours = a.WorkingHours,
                    Status = a.Status,
                    Date = a.Date,
                    Notes = a.Notes
                })
                .ToListAsync();

            return Ok(attendances);
        }

        [HttpGet("my-attendance")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetMyAttendance([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var query = _context.Attendances
                .Where(a => a.UserId == userId)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(a => a.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.Date <= endDate.Value.Date);
            }

            var attendances = await query
                .OrderByDescending(a => a.Date)
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.User.FullName,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    CheckInMethod = a.CheckInMethod,
                    CheckInLocation = a.CheckInLocation,
                    CheckOutLocation = a.CheckOutLocation,
                    WorkingHours = a.WorkingHours,
                    Status = a.Status,
                    Date = a.Date,
                    Notes = a.Notes
                })
                .ToListAsync();

            return Ok(attendances);
        }

        [HttpPost("check-in")]
        public async Task<ActionResult<AttendanceDto>> CheckIn(CheckInDto checkInDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Check if already checked in today
            var today = DateTime.UtcNow.Date;
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

            if (existingAttendance != null)
            {
                return BadRequest(new { message = "Already checked in today" });
            }

            var attendance = new Attendance
            {
                UserId = userId,
                CheckInTime = DateTime.UtcNow,
                CheckInMethod = checkInDto.Method,
                CheckInLocation = checkInDto.Location,
                Status = "Present",
                Date = today,
                Notes = checkInDto.Notes
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return Ok(new AttendanceDto
            {
                Id = attendance.Id,
                UserId = attendance.UserId,
                UserName = user!.FullName,
                CheckInTime = attendance.CheckInTime,
                CheckInMethod = attendance.CheckInMethod,
                CheckInLocation = attendance.CheckInLocation,
                Status = attendance.Status,
                Date = attendance.Date,
                Notes = attendance.Notes
            });
        }

        [HttpPost("check-out")]
        public async Task<ActionResult<AttendanceDto>> CheckOut(CheckOutDto checkOutDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var attendance = await _context.Attendances
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == checkOutDto.AttendanceId && a.UserId == userId);

            if (attendance == null)
            {
                return NotFound(new { message = "Attendance record not found" });
            }

            if (attendance.CheckOutTime.HasValue)
            {
                return BadRequest(new { message = "Already checked out" });
            }

            attendance.CheckOutTime = DateTime.UtcNow;
            attendance.CheckOutLocation = checkOutDto.Location;
            attendance.WorkingHours = (attendance.CheckOutTime.Value - attendance.CheckInTime).TotalHours;
            attendance.WorkingHours = Math.Round(attendance.WorkingHours.Value, 2);

            await _context.SaveChangesAsync();

            return Ok(new AttendanceDto
            {
                Id = attendance.Id,
                UserId = attendance.UserId,
                UserName = attendance.User.FullName,
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                CheckInMethod = attendance.CheckInMethod,
                CheckInLocation = attendance.CheckInLocation,
                CheckOutLocation = attendance.CheckOutLocation,
                WorkingHours = attendance.WorkingHours,
                Status = attendance.Status,
                Date = attendance.Date,
                Notes = attendance.Notes
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceDto>> GetAttendance(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var attendance = await _context.Attendances
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            // Only admin or the owner can view
            if (userRole != "Admin" && attendance.UserId != userId)
            {
                return Forbid();
            }

            return Ok(new AttendanceDto
            {
                Id = attendance.Id,
                UserId = attendance.UserId,
                UserName = attendance.User.FullName,
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                CheckInMethod = attendance.CheckInMethod,
                CheckInLocation = attendance.CheckInLocation,
                CheckOutLocation = attendance.CheckOutLocation,
                WorkingHours = attendance.WorkingHours,
                Status = attendance.Status,
                Date = attendance.Date,
                Notes = attendance.Notes
            });
        }
    }
}
