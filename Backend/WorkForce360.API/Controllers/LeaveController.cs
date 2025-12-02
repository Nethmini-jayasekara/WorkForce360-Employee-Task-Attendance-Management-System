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
    public class LeaveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequests([FromQuery] string? status)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var query = _context.LeaveRequests.Include(l => l.User).AsQueryable();

            // Employees only see their own leave requests
            if (userRole != "Admin")
            {
                query = query.Where(l => l.UserId == userId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.Status == status);
            }

            var leaveRequests = await query
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new LeaveRequestDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    UserName = l.User.FullName,
                    LeaveType = l.LeaveType,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    NumberOfDays = l.NumberOfDays,
                    Reason = l.Reason,
                    Status = l.Status,
                    ApprovedByUserId = l.ApprovedByUserId,
                    ApprovedDate = l.ApprovedDate,
                    RejectionReason = l.RejectionReason,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();

            return Ok(leaveRequests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequest(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            // Only admin or the owner can view
            if (userRole != "Admin" && leaveRequest.UserId != userId)
            {
                return Forbid();
            }

            return Ok(new LeaveRequestDto
            {
                Id = leaveRequest.Id,
                UserId = leaveRequest.UserId,
                UserName = leaveRequest.User.FullName,
                LeaveType = leaveRequest.LeaveType,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                NumberOfDays = leaveRequest.NumberOfDays,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                ApprovedByUserId = leaveRequest.ApprovedByUserId,
                ApprovedDate = leaveRequest.ApprovedDate,
                RejectionReason = leaveRequest.RejectionReason,
                CreatedAt = leaveRequest.CreatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(CreateLeaveRequestDto createLeaveDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (createLeaveDto.EndDate < createLeaveDto.StartDate)
            {
                return BadRequest(new { message = "End date must be after start date" });
            }

            var numberOfDays = (createLeaveDto.EndDate.Date - createLeaveDto.StartDate.Date).Days + 1;

            var leaveRequest = new LeaveRequest
            {
                UserId = userId,
                LeaveType = createLeaveDto.LeaveType,
                StartDate = createLeaveDto.StartDate,
                EndDate = createLeaveDto.EndDate,
                NumberOfDays = numberOfDays,
                Reason = createLeaveDto.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, new LeaveRequestDto
            {
                Id = leaveRequest.Id,
                UserId = leaveRequest.UserId,
                UserName = user!.FullName,
                LeaveType = leaveRequest.LeaveType,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                NumberOfDays = leaveRequest.NumberOfDays,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                CreatedAt = leaveRequest.CreatedAt
            });
        }

        [HttpPost("approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LeaveRequestDto>> ApproveLeave(ApproveLeaveDto approveLeaveDto)
        {
            var adminUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == approveLeaveDto.LeaveRequestId);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            if (leaveRequest.Status != "Pending")
            {
                return BadRequest(new { message = "Leave request has already been processed" });
            }

            leaveRequest.Status = approveLeaveDto.IsApproved ? "Approved" : "Rejected";
            leaveRequest.ApprovedByUserId = adminUserId;
            leaveRequest.ApprovedDate = DateTime.UtcNow;
            leaveRequest.UpdatedAt = DateTime.UtcNow;

            if (!approveLeaveDto.IsApproved && !string.IsNullOrEmpty(approveLeaveDto.RejectionReason))
            {
                leaveRequest.RejectionReason = approveLeaveDto.RejectionReason;
            }

            await _context.SaveChangesAsync();

            return Ok(new LeaveRequestDto
            {
                Id = leaveRequest.Id,
                UserId = leaveRequest.UserId,
                UserName = leaveRequest.User.FullName,
                LeaveType = leaveRequest.LeaveType,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                NumberOfDays = leaveRequest.NumberOfDays,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                ApprovedByUserId = leaveRequest.ApprovedByUserId,
                ApprovedDate = leaveRequest.ApprovedDate,
                RejectionReason = leaveRequest.RejectionReason,
                CreatedAt = leaveRequest.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            // Only admin or the owner can delete
            if (userRole != "Admin" && leaveRequest.UserId != userId)
            {
                return Forbid();
            }

            // Can only delete pending requests
            if (leaveRequest.Status != "Pending")
            {
                return BadRequest(new { message = "Can only delete pending leave requests" });
            }

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
