using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using VenkataAllocationManagementSystem.Enums;
using VenkataAllocationManagementSystem.CustomClass;


namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin, Roles.Manager)]
    public class AssociatesController : Controller
    {
        private readonly ILogger<AssociatesController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public AssociatesController(ILogger<AssociatesController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> AssociateManagement()
        {
            var associatesInfo = new AssociateManagementViewModel
            {
                Associates = await _dbContext.Associates.ToListAsync()
            };
            return View(associatesInfo);
        }
        
        public async Task<IActionResult> CreateAssociate()
        {
            AssociateManagementViewModel associateInfo = new AssociateManagementViewModel()
            {
                //Associate = new Associate(),
                AssociateStatus = await _dbContext.AssociateStatus!.ToListAsync(),
                AssociateTypes = await _dbContext.AssociateTypes!.ToListAsync()
            };
            return View(associateInfo);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAssociate(AssociateManagementViewModel associateInfo)
        {
            if (ModelState.IsValid)
            {
                if (associateInfo.Associate == null)
                {
                    return BadRequest("Associate information is missing.");
                }
                _dbContext.Associates.Add(associateInfo.Associate);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(AssociateManagement));
            }
            return View(associateInfo);
        }

        public async Task<IActionResult> ViewAssociate(int AssociateId)
        {
            // // Fetch all Associates and pass them to the view and also populate the Account Name
            var associate = await _dbContext.Associates.FindAsync(AssociateId);
            if (associate == null)
            {
                return NotFound();
            }
            else
            {
                var associateId = associate.AssociateId;
                var associateType = associate.AssociateTypeId != 0
                ? (await _dbContext.AssociateTypes.FirstOrDefaultAsync(a => a.AssociateTypeId == associate.AssociateTypeId))?.AssociateType
                : null;

                var associateStatusName = associate.AssociateStatusId != 0
                ? (await _dbContext.AssociateStatus.FirstOrDefaultAsync(a => a.AssociateStatusId == associate.AssociateStatusId))?.AssociateStatusName
                : null;

                // associate.AssociateStatusName = associateStatusName;
                // associate.AssociateType = associateType;

                // var accountName = accountId != 0
                // ? (await _dbContext.Associates.FirstOrDefaultAsync(a => a.AssociateId == AssociateId))?.AccountName
                // : null;

                var associateInfo = new AssociateManagementViewModel
                {
                    Associate = associate,
                    AssociateStatusName = associateStatusName,
                    AssociateType = associateType
                    // , AccountName = accountName
                };
                return View(associateInfo);
            }
        }

        public async Task<IActionResult> EditAssociate(int AssociateId)
        {
            var associate = await _dbContext.Associates.FindAsync(AssociateId);
            if (associate == null)
            {
                return NotFound();
            }

            var associateStatusName = associate.AssociateId != 0
                ? (await _dbContext.AssociateStatus.FirstOrDefaultAsync(a => a.AssociateStatusId == associate.AssociateStatusId))?.AssociateStatusName
                : null;

            // ViewBag.AccountName = accountName;

            var associateInfo = new AssociateManagementViewModel()
            {
                Associate = associate,
                AssociateStatus = await _dbContext.AssociateStatus.ToListAsync(),
                AssociateTypes = await _dbContext.AssociateTypes.ToListAsync()

            };

            // System.Diagnostics.EventLog.WriteEntry("Application", "Count of Associate Types: " + associateInfo.AssociateTypes.Count().ToString());

            // System.Diagnostics.EventLog.WriteEntry("Application", "Associate ID Info: " + associateInfo.Associate.AssociateId.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "AssociateStatus ID Info: " + associateInfo.Associate.AssociateStatusId.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", "AssociateType ID Info: " + associateInfo.Associate.AssociateTypeId.ToString());

            return View(associateInfo);
        }

        public async Task<IActionResult> UpdateAssociate(AssociateManagementViewModel associateInfo)
        {
            if (associateInfo.Associate?.AssociateId == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(associateInfo.Associate);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(AssociateManagement));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.Associates.AnyAsync(a => a.AssociateId == associateInfo.Associate.AssociateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(associateInfo.Associate);
        }

        public async Task<IActionResult> DeleteAssociate(int AssociateId)
        {
            var associate = await _dbContext.Associates.FindAsync(AssociateId);
            if (associate == null)
            {
                return NotFound();
            }
            else
            {
                if (associate.AssociateStatusId == (int)Models.AssociateStatusEnum.Active)
                {
                    // Associate is already active, no need to delete
                    associate.AssociateStatusId = (int)Models.AssociateStatusEnum.Inactive; // Soft delete
                    associate.TerminationDate = DateOnly.FromDateTime(DateTime.Now); // Set termination date to now
                }
                else if (associate.AssociateStatusId == (int)Models.AssociateStatusEnum.Inactive)
                {
                    associate.AssociateStatusId = (int)Models.AssociateStatusEnum.Active; // Soft delete
                    associate.TerminationDate = null; // Set termination date to now
                }                
                
                _dbContext.Update(associate);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(AssociateManagement));
            }
        }
    }
}