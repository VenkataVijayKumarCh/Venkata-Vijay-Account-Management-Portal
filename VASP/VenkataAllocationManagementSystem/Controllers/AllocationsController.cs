using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.Models;
using VenkataAllocationManagementSystem.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;
using System.Security.Claims;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Manager)]
    public class AllocationsController : Controller
    {
        private readonly ILogger<AllocationsController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public AllocationsController(ILogger<AllocationsController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> AllocationManagement()
        {
            var allocations = await _dbContext.Allocations.ToListAsync();
            if (allocations == null || !allocations.Any())
            {
                return View(new AllocationManagementViewModel());
            }
            else
            {
                var allocationsInfo = await (from a in _dbContext.Allocations

                                             join proj in _dbContext.Projects on a.ProjectId equals proj.ProjectId
                                             join assoc in _dbContext.Associates on a.AssociateId equals assoc.AssociateId
                                             join acc in _dbContext.Accounts on proj.AccountId equals acc.AccountId
                                             join bt in _dbContext.BillabilityTypes on a.BillabilityTypeId equals bt.BillabilityTypeId
                                             select new AllocationManagementViewModel
                                             {
                                                 AllocationId = a.AllocationId,
                                                 AccountName = acc.AccountName,
                                                 ProjectName = proj.ProjectName,
                                                 ProjectEndDate = proj.EndDate,
                                                 AssociateName = assoc.FullName,
                                                 StartDate = a.StartDate,
                                                 EndDate = a.EndDate,
                                                 IsActive = a.IsActive,
                                                 AllocationPercentage = a.AllocationPercentage,
                                                 BillabilityTypeId = a.BillabilityTypeId
                                             })
                    .ToListAsync();

                var allocationsMgmtInfo = new AllocationManagementViewModel()
                {
                    AllocationsMgmtInfo = allocationsInfo
                };

                return View(allocationsMgmtInfo);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateAllocation()
        {
            var associates = await _dbContext.Associates
                .Where(a => a.AssociateStatusId == (int)AssociateStatusEnum.Active) // Assuming 1 is the ID for 'Active' status
                .OrderBy(a => a.FullName)
                .ToListAsync();
            // var projects = await _dbContext.Projects
            //     .Include(p => p.AccountId > 0)
            //     .OrderBy(p => p.ProjectName)
            //     .ToListAsync();
            var billabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();

            // ViewData["AssociateId"] = new SelectList(associates, "AssociateId", "FullName");
            // ViewData["ProjectId"] = new SelectList(projects.Select(p => new
            // {
            //     p.ProjectId,
            //     DisplayName = $"{p.ProjectName} ({p.Account?.AccountName})"
            // }), "ProjectId", "DisplayName");
            // ViewData["BillabilityTypeId"] = new SelectList(billabilityTypes, "BillabilityTypeId", "TypeName");


           
            var allocationRate = new AllocationRate()
            {
                AllocationRateStartDate = DateOnly.FromDateTime(DateTime.Now),
                AllocationRateEndDate = DateOnly.FromDateTime(DateTime.Now),
            };

            //allocationInfo.AllocationRates.Add(allocationRate);
                

            return View(new AllocationManagementViewModel
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true,
                BillabilityTypes = billabilityTypes,
                Projects = await _dbContext.Projects.ToListAsync(), //.Where(pr => pr.IsActive == true).ToListAsync(),
                Accounts = await _dbContext.Accounts.ToListAsync(),
                Associates = await _dbContext.Associates.ToListAsync(),
                AllocationRates = new List<AllocationRate> { allocationRate }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAllocation(AllocationManagementViewModel allocationInfo)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "Came inside Save Allocation: Model State: " + ModelState.IsValid.ToString());
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Associate ID: {allocationInfo.AssociateId}, Project ID: {allocationInfo.ProjectId}, Start Date: {allocationInfo.StartDate}, End Date: {allocationInfo.EndDate}, Allocation Percentage: {allocationInfo.AllocationPercentage}, Billability Type ID: {allocationInfo.BillabilityTypeId}");

            if (ModelState.IsValid)
            {
                try
                {
                    var newAllocation = new Allocation
                    {
                        AssociateId = allocationInfo.AssociateId,
                        ProjectId = allocationInfo.ProjectId,
                        StartDate = allocationInfo.StartDate,
                        EndDate = allocationInfo.EndDate,
                        AllocationPercentage = allocationInfo.AllocationPercentage,
                        IsActive = true,
                        BillabilityTypeId = allocationInfo.BillabilityTypeId
                    };

                    _dbContext.Add(newAllocation);
                    await _dbContext.SaveChangesAsync();

                    foreach (var rate in allocationInfo.AllocationRates!)
                        { 
                            //System.Diagnostics.EventLog.WriteEntry("Application", "Came to Allocation Rates: ");                          
                            AllocationRate allocationRates = new AllocationRate()
                            {
                                AllocationId = newAllocation.AllocationId,
                                AllocationRateStartDate = rate.AllocationRateStartDate,
                                AllocationRateEndDate = rate.AllocationRateEndDate,
                                AllocationBillRate = rate.AllocationBillRate,
                                AllocationPercentage = allocationInfo.AllocationPercentage,
                                BillabilityTypeId = allocationInfo.BillabilityTypeId,
                                CreatedBy = 1, //userId,
                                CreatedOn = DateTime.Now
                            };
                            await _dbContext.AllocationRates.AddAsync(allocationRates);
                            await _dbContext.SaveChangesAsync();
                        }

                    return RedirectToAction(nameof(AllocationManagement));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating new allocation");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the allocation. Please try again.");
                }
            }

            // System.Diagnostics.EventLog.WriteEntry("Application", "Model State is not valid: " + ModelState.IsValid.ToString() + " Errors: " + JsonSerializer.Serialize(ModelState.Values.SelectMany(v => v.Errors)));

            // If we got this far, something failed; re-populate dropdowns and return view
            allocationInfo.BillabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();
            allocationInfo.Projects = await _dbContext.Projects.ToListAsync(); //.Where(pr => pr.IsActive == true).ToListAsync();
            allocationInfo.Accounts = await _dbContext.Accounts.ToListAsync();
            allocationInfo.Associates = await _dbContext.Associates.ToListAsync();

            return View(allocationInfo);
        }

        public async Task<IActionResult> ViewAllocation(int AllocationId)
        {
            var allocation = await _dbContext.Allocations.FindAsync(AllocationId);
            if (allocation == null)
            {
                return NotFound();
            }

            var project = await _dbContext.Projects.FindAsync(allocation.ProjectId);
            var associate = await _dbContext.Associates.FindAsync(allocation.AssociateId);
            var account = await _dbContext.Accounts.FindAsync(project?.AccountId);
            var allocationRates = await _dbContext.AllocationRates
                                        .Where(ar => ar.AllocationId == AllocationId)
                                        .ToListAsync();

            var allocationInfo = new AllocationManagementViewModel()
            {
                AllocationsInfo = new List<Allocation> { allocation },
                ProjectName = project?.ProjectName,
                AssociateName = associate?.FullName,
                AccountName = account?.AccountName,
                ProjectEndDate = project!.EndDate,
                AllocationRates = allocationRates
            };

            return View(allocationInfo);
        }

        public async Task<IActionResult> EditAllocation(int AllocationId)
        {
            var allocation = await _dbContext.Allocations.FindAsync(AllocationId);
            if (allocation == null)
            {
                return NotFound();
            }
            else
            {
                var project = await _dbContext.Projects.FindAsync(allocation.ProjectId);
                var associate = await _dbContext.Associates.FindAsync(allocation.AssociateId);
                var account = await _dbContext.Accounts.FindAsync(project?.AccountId);
                var billabilityTypes = await _dbContext.BillabilityTypes.ToListAsync();                

                // Prepare the view model with allocation details
                var allocationInfo = new AllocationManagementViewModel()
                {
                    AllocationsInfo = new List<Allocation> { allocation },
                    EndDate = allocation.EndDate,
                    StartDate = allocation.StartDate,
                    ProjectName = project?.ProjectName,
                    AssociateName = associate?.FullName,
                    AccountName = account?.AccountName,
                    ProjectEndDate = project!.EndDate,
                    ProjectStartDate = project.StartDate,
                    BillabilityTypeId = allocation.BillabilityTypeId,
                    BillabilityTypes = billabilityTypes
                };

                if(await _dbContext.AllocationRates.Where(ar => ar.AllocationId == AllocationId).CountAsync() == 0)
                {
                    // System.Diagnostics.EventLog.WriteEntry("Application", "Allocation Rates DB Set is null");
                    // allocationInfo.AllocationRate = null;
                    //allocationInfo.AllocationRates.Add(GenerateEmptyAllocationRateLineITem());
                    //allocationInfo.AllocationRates = null;

                    var allocationRate = new AllocationRate()
                    {
                        AllocationRateStartDate = allocation.StartDate,
                        AllocationRateEndDate = allocation.EndDate
                    };

                    allocationInfo.AllocationRates.Add(allocationRate);
                }
                else
                {
                    // System.Diagnostics.EventLog.WriteEntry("Application", "Allocation Rates DB Set is not null");
                    // var allocationRate = await _dbContext.AllocationRates
                    //                     .Where(ar => ar.AllocationId == AllocationId)
                    //                     .OrderByDescending(ar => ar.AllocationRateId).FirstOrDefaultAsync();
                    // allocationInfo.AllocationRate = allocationRate;
                    var allocationRates = await _dbContext.AllocationRates
                                        .Where(ar => ar.AllocationId == AllocationId)
                                        .OrderByDescending(ar => ar.AllocationRateId).ToListAsync();
                    //allocationInfo.AllocationRate = allocationRate;
                    allocationInfo.AllocationRates = allocationRates;
                }

                return View(allocationInfo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAllocation(AllocationManagementViewModel allocationInfo)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", "I came here - Starting");
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Controller - Update Allocation - Project End Date : {allocationInfo.ProjectEndDate}");
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Controller - Update Allocation - Allocation End Date : {allocationInfo.AllocationsInfo[0].EndDate}");
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Associate Name : {allocationInfo.AssociateName}");
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Project Name : {allocationInfo.ProjectName}");
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Account Name : {allocationInfo.AccountName}");

            if (allocationInfo.AllocationsInfo[0].AllocationId == 0)
            {
                return NotFound();
            }
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Controller - Update Allocation - Model State is : {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                try
                {
                    var allocation = allocationInfo.AllocationsInfo[0];
                    allocation.StartDate = allocationInfo.StartDate;
                    allocation.EndDate = allocationInfo.EndDate;
                    allocation.BillabilityTypeId = allocationInfo.BillabilityTypeId;
                    allocation.AllocationPercentage = allocationInfo.AllocationsInfo[0].AllocationPercentage;
                    _dbContext.Update(allocation);
                    await _dbContext.SaveChangesAsync();


                    // Check if there's any change
                    // bool rolesChanged = !existingRoleIds.OrderBy(x => x).SequenceEqual(userInfo.SelectedRoles!.OrderBy(x => x));

                    // if (rolesChanged)
                    // {
                    var existingAllocationRates = _dbContext.AllocationRates.Where(ur => ur.AllocationId == allocationInfo.AllocationsInfo[0].AllocationId);
                        _dbContext.AllocationRates.RemoveRange(existingAllocationRates);
                    // _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
                    // _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
                    // _dbContext.CurrentUser = User.Identity!.Name!;                    
                    await _dbContext.SaveChangesAsync();
                        
                        var userId = User.FindFirstValue(ClaimTypes.Name);
// System.Diagnostics.EventLog.WriteEntry("Application", "Came before Allocation Rates Adding: " );
// System.Diagnostics.EventLog.WriteEntry("Application", "Allocation Rate Start Date: " + allocationInfo.AllocationRate!.AllocationRateStartDate.ToString());
// System.Diagnostics.EventLog.WriteEntry("Application", "Allocation Rate End Date: " + allocationInfo.AllocationRate!.AllocationRateEndDate.ToString());
// System.Diagnostics.EventLog.WriteEntry("Application", "Allocation Bill Rate: " + allocationInfo.AllocationRate!.AllocationBillRate.ToString());


                    // var allocationRateinfo = new AllocationRate
                    // {
                    //     AllocationId = allocationInfo.AllocationsInfo[0].AllocationId,
                    //     AllocationRateStartDate = allocationInfo.AllocationRate!.AllocationRateStartDate,
                    //     AllocationRateEndDate = allocationInfo.AllocationRate.AllocationRateEndDate,
                    //     AllocationBillRate = allocationInfo.AllocationRate.AllocationBillRate,
                    //     AllocationPercentage = allocationInfo.AllocationsInfo[0].AllocationPercentage,
                    //     BillabilityTypeId = allocationInfo.BillabilityTypeId,
                    //     CreatedBy = 1,                    
                    //     CreatedOn = DateTime.Now
                    // };
                    // _dbContext.Add(allocationRateinfo);
                    // await _dbContext.SaveChangesAsync();

                        foreach (var rate in allocationInfo.AllocationRates!)
                        {                           
                            AllocationRate allocationRates = new AllocationRate()
                            {
                                AllocationId = allocationInfo.AllocationsInfo[0].AllocationId,
                                AllocationRateStartDate = rate.AllocationRateStartDate,
                                AllocationRateEndDate = rate.AllocationRateEndDate,
                                AllocationBillRate = rate.AllocationBillRate,
                                AllocationPercentage = allocationInfo.AllocationsInfo[0].AllocationPercentage,
                                BillabilityTypeId = allocationInfo.BillabilityTypeId,
                                CreatedBy = 1, //userId,
                                CreatedOn = DateTime.Now
                            };
                            await _dbContext.AllocationRates.AddAsync(allocationRates);
                            await _dbContext.SaveChangesAsync();
                        }
                        // _dbContext.CurrentController = ControllerContext.ActionDescriptor.ControllerName;
                        // _dbContext.CurrentAction = ControllerContext.ActionDescriptor.ActionName;
                        // _dbContext.CurrentUser = User.Identity!.Name!;
                        
                    // }



                    return RedirectToAction(nameof(AllocationManagement));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.Allocations.AnyAsync(a => a.AllocationId == allocationInfo.AllocationsInfo[0].AllocationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                // System.Diagnostics.EventLog.WriteEntry("Application", "Model State is not valid");
                // If model state is invalid, return the same view with the current model
                return View(allocationInfo);
            }
            // System.Diagnostics.EventLog.WriteEntry("Application", "I came here - ending");


        }

        // GET: Allocations
        // public async Task<IActionResult> Index()
        // {
        //     var applicationDbContext = _context.Allocations
        //         .Include(a => a.Associate)
        //         .Include(a => a.Project)
        //         .ThenInclude(p => p.AccountId);
        //     return View(await applicationDbContext.ToListAsync());
        // }

        // GET: Allocations/Details/5
        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null) return NotFound();

        //     var allocation = await _context.Allocations
        //         .Include(a => a.Associate)
        //         .Include(a => a.Project)
        //         .ThenInclude(p => p.AccountId)
        //         .FirstOrDefaultAsync(m => m.AllocationId == id);

        //     if (allocation == null) return NotFound();

        //     return View(allocation);
        // }

        // // GET: Allocations/Create
        // public IActionResult Create()
        // {
        //     PopulateDropDowns();
        //     return View();
        // }

        // POST: Allocations/Create
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("AssociateId,ProjectId,StartDate,EndDate")] Allocation allocation)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         if (allocation.EndDate < allocation.StartDate)
        //         {
        //             ModelState.AddModelError(string.Empty, "End Date must be greater than or equal to Start Date.");
        //             PopulateDropDowns(allocation.AssociateId, allocation.ProjectId);
        //             return View(allocation);
        //         }

        //         _context.Add(allocation);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     PopulateDropDowns(allocation.AssociateId, allocation.ProjectId);
        //     return View(allocation);
        // }

        // // GET: Allocations/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null) return NotFound();

        //     var allocation = await _context.Allocations.FindAsync(id);
        //     if (allocation == null) return NotFound();

        //     PopulateDropDowns(allocation.AssociateId, allocation.ProjectId);
        //     return View(allocation);
        // }

        // // POST: Allocations/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("AllocationId,AssociateId,ProjectId,StartDate,EndDate")] Allocation allocation)
        // {
        //     if (id != allocation.AllocationId) return NotFound();

        //     if (ModelState.IsValid)
        //     {
        //         if (allocation.EndDate < allocation.StartDate)
        //         {
        //             ModelState.AddModelError(string.Empty, "End Date must be greater than or equal to Start Date.");
        //             PopulateDropDowns(allocation.AssociateId, allocation.ProjectId);
        //             return View(allocation);
        //         }

        //         try
        //         {
        //             _context.Update(allocation);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!AllocationExists(allocation.AllocationId))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     PopulateDropDowns(allocation.AssociateId, allocation.ProjectId);
        //     return View(allocation);
        // }

        // GET: Allocations/Delete/5
        public async Task<IActionResult> CompleteAllocation(int allocationId)
        {
            var allocation = await _dbContext.Allocations.FindAsync(allocationId);
            if (allocation == null)
            {
                return NotFound();
            }
            else
            {
                allocation.IsActive = false; // Soft delete
                if (allocation.EndDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    allocation.EndDate = allocation.EndDate;
                }
                else
                {
                    allocation.EndDate = DateOnly.FromDateTime(DateTime.Now);
                }
                _dbContext.Update(allocation);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(AllocationManagement));
            }
        }

        // POST: Allocations/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var allocation = await _context.Allocations.FindAsync(id);
        //     if (allocation != null)
        //     {
        //         _context.Allocations.Remove(allocation);
        //         await _context.SaveChangesAsync();
        //     }
        //     return RedirectToAction(nameof(Index));
        // }

        // private bool AllocationExists(int id)
        // {
        //     return _context.Allocations.Any(e => e.AllocationId == id);
        // }

        // private void PopulateDropDowns(object? selectedAssociate = null, object? selectedProject = null)
        // {
        //     // ViewData["AssociateId"] = new SelectList(_context.Associates.OrderBy(a => a.FullName), "AssociateId", "FullName", selectedAssociate);
        //     // var projects = _context.Projects.Include(p => p.Account).ToList();
        //     // ViewData["ProjectId"] = new SelectList(projects.Select(p => new 
        //     //     { p.ProjectId, DisplayName = $"{p.Name} ({p.Account?.AccountName})" }), "ProjectId", "DisplayName", selectedProject);
        // }

        public JsonResult GetProjectsByAccount(int accountId)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Account ID received: {accountId}");
            var projects = _dbContext.Projects.Where(p => p.AccountId == accountId).ToList();
            // Transform projects into a format suitable for the dropdown (e.g., SelectListItem)
            var projectList = projects.Select(p => new SelectListItem { Value = p.ProjectId.ToString(), Text = p.ProjectName });
            return Json(projectList);
        }

        public string GetProjectDetailsBySelectedProjectId(int projectId)
        {
            // System.Diagnostics.EventLog.WriteEntry("Application", $"Project ID received: {projectId}");
            var project = _dbContext.Projects.FirstOrDefault(p => p.ProjectId == projectId);

            // Transform projects into a format suitable for the dropdown (e.g., SelectListItem)
            // var projectList = projects.Select(p => new SelectListItem { Value = p.ProjectId.ToString(), Text = p.ProjectName });          

            // return Json(project);
            return (project?.StartDate + ";" + project?.EndDate);
        }
    
        private AllocationRate GenerateEmptyAllocationRateLineITem()
        {
            AllocationRate allocationRate = new AllocationRate();
            return allocationRate;
        }
    }
}