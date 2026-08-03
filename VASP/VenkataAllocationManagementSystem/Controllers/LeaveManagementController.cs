using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.Enums;
using VenkataAllocationManagementSystem.Models;
using VenkataAllocationManagementSystem.ViewModels;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin, Roles.Manager)]
    public class LeaveManagementController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public LeaveManagementController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(ManageLeaves));
        }

        public async Task<IActionResult> ManageLeaves()
        {
            var model = new LeaveManagementViewModel
            {
                Associates = await _dbContext.Associates.OrderBy(a => a.FullName).ToListAsync(),
                LeaveRequests = await _dbContext.LeaveRequests
                    .Where(l => l.IsValid && (l.LeaveStatusId == (int)LeaveStatuses.Pending || l.LeaveStatusId == (int)LeaveStatuses.Submitted))
                    .Include(l => l.Associate)
                    .OrderByDescending(l => l.StartDate)
                    .ToListAsync(),
                LeaveRequest = new LeaveRequest
                {
                    StartDate = DateOnly.FromDateTime(DateTime.Today),
                    EndDate = DateOnly.FromDateTime(DateTime.Today),
                    RequestedDays = 1,
                    LeaveType = "Annual",
                    Status = "Pending",
                    LeaveRequestId = (int)LeaveStatuses.Pending
                }
            };

            return View(model);
        }

        public async Task<IActionResult> LeaveRequestsHistory()
        {
            var model = new LeaveManagementViewModel
            {
                Associates = await _dbContext.Associates.OrderBy(a => a.FullName).ToListAsync(),
                LeaveRequests = await _dbContext.LeaveRequests
                    .Where(l => l.IsValid && (l.LeaveStatusId == (int)LeaveStatuses.Approved || l.LeaveStatusId == (int)LeaveStatuses.Rejected))
                    .Include(l => l.Associate)
                    .OrderByDescending(l => l.StartDate)
                    .ToListAsync()
                // ,
                // LeaveRequest = new LeaveRequest
                // {
                //     StartDate = DateOnly.FromDateTime(DateTime.Today),
                //     EndDate = DateOnly.FromDateTime(DateTime.Today),
                //     RequestedDays = 1,
                //     LeaveType = "Annual",
                //     Status = "Pending",
                //     LeaveRequestId = (int)LeaveStatuses.Pending
                // }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLeave(LeaveManagementViewModel model)
        {
            if (model.LeaveRequest == null)
            {
                return BadRequest("Leave request details are missing.");
            }

            if (model.LeaveRequest.EndDate < model.LeaveRequest.StartDate)
            {
                ModelState.AddModelError("LeaveRequest.EndDate", "End date must be on or after the start date.");
            }

            if (model.LeaveRequest.RequestedDays <= 0)
            {
                model.LeaveRequest.RequestedDays = CalculateDefaultRequestedDays(model.LeaveRequest.StartDate, model.LeaveRequest.EndDate);
            }

            if (ModelState.IsValid)
            {
                model.LeaveRequest.LeaveStatusId = (int)LeaveStatuses.Pending; // Set the LeaveStatusId to Pending
                _dbContext.LeaveRequests.Add(model.LeaveRequest);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave request saved successfully.";
                return RedirectToAction(nameof(ManageLeaves));
            }

            model.Associates = await _dbContext.Associates.OrderBy(a => a.FullName).ToListAsync();
            model.LeaveRequests = await _dbContext.LeaveRequests
                .Include(l => l.Associate)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
            return View("ManageLeaves", model);
        }

        public async Task<IActionResult> EditLeave(int id)
        {
            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            // Only allow editing Pending requests
            // if (leaveRequest.Status != "Pending" && leaveRequest.Status != "Submitted")
            // {
            //     TempData["ErrorMessage"] = "You can only edit leave requests with Pending / Submitted status.";
            //     return RedirectToAction(nameof(ManageLeaves));
            // }

            var model = new LeaveManagementViewModel
            {
                Associates = await _dbContext.Associates.OrderBy(a => a.FullName).ToListAsync(),
                LeaveRequest = leaveRequest,
                LeaveRequests = await _dbContext.LeaveRequests
                    .Where(l => l.IsValid)
                    .Include(l => l.Associate)
                    .OrderByDescending(l => l.StartDate)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLeave(LeaveManagementViewModel model)
        {
            if (model.LeaveRequest == null)
            {
                return BadRequest("Leave request details are missing.");
            }

            // Verify request exists and is in Pending status
            var existingRequest = await _dbContext.LeaveRequests.FindAsync(model.LeaveRequest.LeaveRequestId);
            if (existingRequest == null || existingRequest.Status != "Pending")
            {
                return BadRequest("This leave request cannot be edited.");
            }

            if (model.LeaveRequest.EndDate < model.LeaveRequest.StartDate)
            {
                ModelState.AddModelError("LeaveRequest.EndDate", "End date must be on or after the start date.");
            }

            if (model.LeaveRequest.RequestedDays <= 0)
            {
                model.LeaveRequest.RequestedDays = CalculateDefaultRequestedDays(model.LeaveRequest.StartDate, model.LeaveRequest.EndDate);
            }

            if (ModelState.IsValid)
            {
                // Update the tracked entity directly to avoid duplicate tracking
                existingRequest.StartDate = model.LeaveRequest.StartDate;
                existingRequest.EndDate = model.LeaveRequest.EndDate;
                existingRequest.LeaveType = model.LeaveRequest.LeaveType;
                existingRequest.RequestedDays = model.LeaveRequest.RequestedDays;
                existingRequest.Notes = model.LeaveRequest.Notes;
                existingRequest.RequestedOn = DateTime.Now;
                existingRequest.LeaveStatusId = (int)LeaveStatuses.Pending; // Reset the LeaveStatusId to Pending on edit
                // if (existingRequest.Status == "Pending")
                // {
                //     existingRequest.Status = "Submitted"; // Reset status to Submitted on edit if it was Pending
                // }

                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave request updated successfully.";
                return RedirectToAction(nameof(ManageLeaves));
            }

            model.Associates = await _dbContext.Associates.OrderBy(a => a.FullName).ToListAsync();
            model.LeaveRequests = await _dbContext.LeaveRequests
                .Where(l => l.IsValid)
                .Include(l => l.Associate)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLeave(int id)
        {
            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(id);
            if (leaveRequest != null && leaveRequest.Status == "Pending")
            {
                leaveRequest.IsValid = false;
                _dbContext.LeaveRequests.Update(leaveRequest);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave request deleted successfully.";
            }
            else if (leaveRequest != null && leaveRequest.Status != "Pending")
            {
                TempData["ErrorMessage"] = "You can only delete leave requests with Pending status.";
            }

            return RedirectToAction(nameof(ManageLeaves));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitLeave(int id, string? comments = null)
        {
            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(id);
            if (leaveRequest != null && leaveRequest.Status == "Pending")
            {
                leaveRequest.Status = "Submitted";
                leaveRequest.LeaveStatusId = (int)LeaveStatuses.Submitted;
                leaveRequest.Comments = comments;
                _dbContext.LeaveRequests.Update(leaveRequest);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave request submitted for approval successfully.";
            }

            return RedirectToAction(nameof(ManageLeaves));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(int id, string? comments = null)
        {
            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(id);
            if (leaveRequest != null && leaveRequest.Status == "Submitted")
            {
                leaveRequest.Status = "Approved";
                leaveRequest.LeaveStatusId = (int)LeaveStatuses.Approved; // Set the LeaveStatusId to Approved  
                leaveRequest.ApprovedOn = DateTime.UtcNow;
                leaveRequest.ApprovedBy = User.Identity?.Name ?? "System";
                leaveRequest.Comments = comments;
                _dbContext.LeaveRequests.Update(leaveRequest);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave request approved successfully.";
            }

            return RedirectToAction(nameof(ManageLeaves));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLeave(int id, string? comments = null)
        {
            var leaveRequest = await _dbContext.LeaveRequests.FindAsync(id);
            if (leaveRequest != null && leaveRequest.Status == "Submitted")
            {
                leaveRequest.Status = "Rejected";
                leaveRequest.LeaveStatusId = (int)LeaveStatuses.Rejected; // Set the LeaveStatusId to Rejected
                leaveRequest.ApprovedOn = DateTime.UtcNow;
                leaveRequest.ApprovedBy = User.Identity?.Name ?? "System";
                leaveRequest.Comments = comments;
                _dbContext.LeaveRequests.Update(leaveRequest);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave request rejected successfully.";
            }

            return RedirectToAction(nameof(ManageLeaves));
        }

        public async Task<IActionResult> ManageHolidays()
        {
            var model = new LeaveManagementViewModel
            {
                Holidays = await _dbContext.Holidays.OrderBy(h => h.HolidayDate).ToListAsync(),
                Holiday = new Holiday
                {
                    HolidayDate = DateOnly.FromDateTime(DateTime.Today),
                    HolidayName = string.Empty
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHoliday(LeaveManagementViewModel model)
        {
            if (model.Holiday == null)
            {
                return BadRequest("Holiday details are missing.");
            }

            if (ModelState.IsValid)
            {
                _dbContext.Holidays.Add(model.Holiday);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Holiday saved successfully.";
                return RedirectToAction(nameof(ManageHolidays));
            }

            model.Holidays = await _dbContext.Holidays.OrderBy(h => h.HolidayDate).ToListAsync();
            return View("ManageHolidays", model);
        }

        public async Task<IActionResult> EditHoliday(int id)
        {
            var holiday = await _dbContext.Holidays.FindAsync(id);
            if (holiday == null)
            {
                return NotFound();
            }

            var model = new LeaveManagementViewModel
            {
                Holiday = holiday,
                Holidays = await _dbContext.Holidays.OrderBy(h => h.HolidayDate).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHoliday(LeaveManagementViewModel model)
        {
            if (model.Holiday == null)
            {
                return BadRequest("Holiday details are missing.");
            }

            if (ModelState.IsValid)
            {
                _dbContext.Holidays.Update(model.Holiday);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Holiday updated successfully.";
                return RedirectToAction(nameof(ManageHolidays));
            }

            model.Holidays = await _dbContext.Holidays.OrderBy(h => h.HolidayDate).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            var holiday = await _dbContext.Holidays.FindAsync(id);
            if (holiday != null)
            {
                _dbContext.Holidays.Remove(holiday);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Holiday deleted successfully.";
            }

            return RedirectToAction(nameof(ManageHolidays));
        }

        private decimal CalculateDefaultRequestedDays(DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
            {
                return 0;
            }

            decimal count = 0;
            for (var current = startDate; current <= endDate; current = current.AddDays(1))
            {
                var isWeekend = current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday;
                var isHoliday = _dbContext.Holidays.Any(h => h.HolidayDate == current);
                if (!isWeekend && !isHoliday)
                {
                    count += 1;
                }
            }

            return Math.Round(count, 2);
        }
    }
}
