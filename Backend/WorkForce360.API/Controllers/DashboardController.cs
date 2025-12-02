using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkForce360.API.Data;

namespace WorkForce360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            var today = DateTime.UtcNow.Date;

            var totalEmployees = await _context.Users.CountAsync(u => u.Role == "Employee" && u.IsActive);
            var presentToday = await _context.Attendances.CountAsync(a => a.Date == today);
            var pendingTasks = await _context.EmployeeTasks.CountAsync(t => t.Status == "Pending");
            var completedTasks = await _context.EmployeeTasks.CountAsync(t => t.Status == "Completed");
            var pendingLeaves = await _context.LeaveRequests.CountAsync(l => l.Status == "Pending");

            return Ok(new
            {
                totalEmployees,
                presentToday,
                absentToday = totalEmployees - presentToday,
                pendingTasks,
                completedTasks,
                inProgressTasks = await _context.EmployeeTasks.CountAsync(t => t.Status == "InProgress"),
                pendingLeaves,
                approvedLeaves = await _context.LeaveRequests.CountAsync(l => l.Status == "Approved"),
                rejectedLeaves = await _context.LeaveRequests.CountAsync(l => l.Status == "Rejected")
            });
        }

        [HttpGet("recent-activity")]
        public async Task<ActionResult<object>> GetRecentActivity()
        {
            var recentAttendances = await _context.Attendances
                .Include(a => a.User)
                .OrderByDescending(a => a.CheckInTime)
                .Take(10)
                .Select(a => new
                {
                    type = "attendance",
                    userName = a.User.FullName,
                    action = a.CheckOutTime.HasValue ? "Checked Out" : "Checked In",
                    timestamp = a.CheckOutTime ?? a.CheckInTime,
                    details = $"{a.Status} - {a.WorkingHours ?? 0:F2} hours"
                })
                .ToListAsync();

            var recentTasks = await _context.EmployeeTasks
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .Take(10)
                .Select(t => new
                {
                    type = "task",
                    userName = t.AssignedToUser.FullName,
                    action = t.Status,
                    timestamp = t.UpdatedAt ?? t.CreatedAt,
                    details = t.Title
                })
                .ToListAsync();

            var recentLeaves = await _context.LeaveRequests
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .Take(10)
                .Select(l => new
                {
                    type = "leave",
                    userName = l.User.FullName,
                    action = l.Status,
                    timestamp = l.CreatedAt,
                    details = $"{l.LeaveType} - {l.NumberOfDays} days"
                })
                .ToListAsync();

            var allActivities = recentAttendances
                .Concat(recentTasks)
                .Concat(recentLeaves)
                .OrderByDescending(a => a.timestamp)
                .Take(20)
                .ToList();

            return Ok(allActivities);
        }

        [HttpGet("attendance-summary")]
        public async Task<ActionResult<object>> GetAttendanceSummary([FromQuery] int days = 7)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days);
            
            var attendanceByDay = await _context.Attendances
                .Where(a => a.Date >= startDate)
                .GroupBy(a => a.Date)
                .Select(g => new
                {
                    date = g.Key,
                    present = g.Count(),
                    late = g.Count(a => a.Status == "Late"),
                    avgWorkingHours = g.Average(a => a.WorkingHours ?? 0)
                })
                .OrderBy(a => a.date)
                .ToListAsync();

            return Ok(attendanceByDay);
        }
    }
}
