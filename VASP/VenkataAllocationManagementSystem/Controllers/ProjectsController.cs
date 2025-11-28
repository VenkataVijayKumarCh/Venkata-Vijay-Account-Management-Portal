using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenkataAllocationManagementSystem.Data;
using VenkataAllocationManagementSystem.ViewModels;
using VenkataAllocationManagementSystem.Models;
using System.Collections.Frozen;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http.Features;
using VenkataAllocationManagementSystem.CustomClass;
using VenkataAllocationManagementSystem.Enums;

namespace VenkataAllocationManagementSystem.Controllers
{
    [CustomAuthorize(Roles.Admin, Roles.Manager)]
    public class ProjectsController : Controller
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ProjectsController(ILogger<ProjectsController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> ProjectManagement()
        {
            // // Fetch all projects and pass them to the view and also populate the Account Name
            var projects = await _dbContext.Projects.ToListAsync();
            var accountId = projects.FirstOrDefault()?.AccountId;
            var accountName = accountId != null
                ? (await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId))?.AccountName
                : null;

            var projectInfo = new ProjectManagementViewModel
            {
                Projects = projects,
                AccountName = accountName
            };


            // var projectsInfo = (ProjectManagementViewModel)(from pro in _dbContext.Set<Project>() join acc in _dbContext.Set<Account>()
            //     on pro.AccountId equals acc.AccountId
            //     select new ProjectManagementViewModel
            //     {
            //         Project = pro,
            //         AccountName = acc.AccountName
            //     });

            // // var projectsInfo = await _dbContext.

            return View(projectInfo);

        }

        public async Task<IActionResult> CreateProject()
        {
            var projectInfo = new ProjectManagementViewModel();
            var accountInfo = new ManagementDashboardViewModel
            {
                Accounts = await _dbContext.Accounts.ToListAsync()
            };

            projectInfo.Accounts = accountInfo.Accounts;

            return View(projectInfo);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProject(Project project)
        {
            if (ModelState.IsValid)
            {
                // Project projectInfo = new Project();
                // projectInfo.AccountId = project.AccountId
                _dbContext.Projects.Add(project);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectManagement));
            }
            return View(project);
        }

        public async Task<IActionResult> ViewProject(int projectId)
        {
            // // Fetch all projects and pass them to the view and also populate the Account Name
            var project = await _dbContext.Projects.FindAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                var accountId = project.AccountId;

                var accountName = accountId != 0
                ? (await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId))?.AccountName
                : null;

                var allocationsInfo = projectId != 0
                ? (await _dbContext.Allocations.Where(a => a.ProjectId == projectId).ToListAsync())
                : null;

                // var associatesInfo = projectId != 0
                // ? (await _dbContext.Associates.Where(a => a.AssociateId.In(allocationsInfo.FindAll(a => a.AllocationId == a.AllocationId).Select(a => a.AssociateId).ToList())).ToListAsync())
                // : null;

                // write a query to get associates details based on the projectId and its allocations                
                var associatesInfo = from a in _dbContext.Associates
                                     join al in _dbContext.Allocations on a.AssociateId equals al.AssociateId
                                     where al.ProjectId == projectId
                                     select a;



                // from as in _dbContext.Associates
                //                      join al in _dbContext.Allocations on as.AssociateId.equals(al.AssociateId)
                //                      where al.ProjectId == projectId
                //                      select as.ToListAsync();

                var projectInfo = new ProjectManagementViewModel
                {
                    Project = project,
                    AccountName = accountName,
                    Allocations = allocationsInfo,
                    Associates = associatesInfo
                };
                return View(projectInfo);
            }
        }

        public async Task<IActionResult> EditProject(int ProjectId)
        {
            var project = await _dbContext.Projects.FindAsync(ProjectId);
            if (project == null)
            {
                return NotFound();
            }

            var accountName = project.AccountId != 0
                ? (await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == project.AccountId))?.AccountName
                : null;

            ViewBag.AccountName = accountName;
            return View(project);
        }

        public async Task<IActionResult> UpdateProject(int ProjectId, Project project)
        {
            if (ProjectId != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(project);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(ProjectManagement));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.Projects.AnyAsync(p => p.ProjectId == ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(project);
        }

        public async Task<IActionResult> DeleteProject(int ProjectId)
        {
            var project = await _dbContext.Projects.FindAsync(ProjectId);
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                project.IsActive = false; // Soft delete
                _dbContext.Update(project);
                await _dbContext.SaveChangesAsync();

                var allocations = await _dbContext.Allocations.Where(a => a.ProjectId == ProjectId).ToListAsync();
                foreach (var allocation in allocations)
                {
                    allocation.IsActive = false; // Soft delete
                    allocation.EndDate = DateOnly.FromDateTime(DateTime.Now);
                    _dbContext.Update(allocation);
                }
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(ProjectManagement));
            }
            //return View(account);
        }

    }
}