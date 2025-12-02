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
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] string? status)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var query = _context.EmployeeTasks.Include(t => t.AssignedToUser).AsQueryable();

            // Employees only see their own tasks
            if (userRole != "Admin")
            {
                query = query.Where(t => t.AssignedToUserId == userId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            var tasks = await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName = t.AssignedToUser.FullName,
                    Status = t.Status,
                    Priority = t.Priority,
                    ProgressPercentage = t.ProgressPercentage,
                    StartDate = t.StartDate,
                    DueDate = t.DueDate,
                    CompletedDate = t.CompletedDate,
                    Notes = t.Notes,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var task = await _context.EmployeeTasks
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            // Only admin or assigned employee can view
            if (userRole != "Admin" && task.AssignedToUserId != userId)
            {
                return Forbid();
            }

            return Ok(new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser.FullName,
                Status = task.Status,
                Priority = task.Priority,
                ProgressPercentage = task.ProgressPercentage,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                Notes = task.Notes,
                CreatedAt = task.CreatedAt
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createTaskDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var assignedUser = await _context.Users.FindAsync(createTaskDto.AssignedToUserId);
            if (assignedUser == null)
            {
                return BadRequest(new { message = "Assigned user not found" });
            }

            var task = new EmployeeTask
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                AssignedToUserId = createTaskDto.AssignedToUserId,
                Priority = createTaskDto.Priority,
                Status = "Pending",
                StartDate = createTaskDto.StartDate,
                DueDate = createTaskDto.DueDate,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.EmployeeTasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = assignedUser.FullName,
                Status = task.Status,
                Priority = task.Priority,
                ProgressPercentage = task.ProgressPercentage,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                Notes = task.Notes,
                CreatedAt = task.CreatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var task = await _context.EmployeeTasks
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            // Admin can update all fields, employees can only update status and progress
            if (userRole != "Admin" && task.AssignedToUserId != userId)
            {
                return Forbid();
            }

            if (userRole == "Admin")
            {
                if (!string.IsNullOrEmpty(updateTaskDto.Title))
                    task.Title = updateTaskDto.Title;

                if (updateTaskDto.Description != null)
                    task.Description = updateTaskDto.Description;

                if (updateTaskDto.AssignedToUserId.HasValue)
                    task.AssignedToUserId = updateTaskDto.AssignedToUserId.Value;

                if (!string.IsNullOrEmpty(updateTaskDto.Priority))
                    task.Priority = updateTaskDto.Priority;

                if (updateTaskDto.DueDate.HasValue)
                    task.DueDate = updateTaskDto.DueDate;
            }

            // Both admin and employee can update these
            if (!string.IsNullOrEmpty(updateTaskDto.Status))
            {
                task.Status = updateTaskDto.Status;
                if (updateTaskDto.Status == "Completed")
                {
                    task.CompletedDate = DateTime.UtcNow;
                    task.ProgressPercentage = 100;
                }
            }

            if (updateTaskDto.ProgressPercentage.HasValue)
                task.ProgressPercentage = updateTaskDto.ProgressPercentage.Value;

            if (updateTaskDto.Notes != null)
                task.Notes = updateTaskDto.Notes;

            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser.FullName,
                Status = task.Status,
                Priority = task.Priority,
                ProgressPercentage = task.ProgressPercentage,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                Notes = task.Notes,
                CreatedAt = task.CreatedAt
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.EmployeeTasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            _context.EmployeeTasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
