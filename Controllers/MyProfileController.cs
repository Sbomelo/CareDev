using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CareDev.Models.ViewModels;

namespace YourProjectNamespace.Controllers
{
    [Authorize]
    public class MyProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MyProfileController> _logger;

        public MyProfileController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<MyProfileController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /MyProfile
        public async Task<IActionResult> Index()
        {
            var (kind, entity) = await GetCurrentProfileAsync();
            if (entity == null) return NotFound();

            var vm = await MapToViewModelAsync(kind!, entity!);
            // Email (from ApplicationUser)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appUser = userId != null ? await _userManager.FindByIdAsync(userId) : null;
            ViewBag.Email = appUser?.Email;

            return View(vm);
        }

        // GET: /MyProfile/Edit
        public async Task<IActionResult> Edit()
        {
            var (kind, entity) = await GetCurrentProfileAsync();
            if (entity == null) return NotFound();

            var vm = await MapToViewModelAsync(kind!, entity!);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appUser = userId != null ? await _userManager.FindByIdAsync(userId) : null;
            ViewBag.Email = appUser?.Email;

            return View(vm);
        }

        // POST: /MyProfile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            var (kind, entity) = await GetCurrentProfileAsync();
            if (entity == null) return NotFound();

            // ensure the posted model matches the current entity (basic guard)
            if (!string.Equals(kind, model.EntityKind, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Mismatched profile type.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var appUser = userId != null ? await _userManager.FindByIdAsync(userId) : null;
                ViewBag.Email = appUser?.Email;
                return View(model);
            }

            try
            {
                if (string.Equals(model.EntityKind, "Patient", StringComparison.OrdinalIgnoreCase))
                {
                    var patient = (CareDev.Models.Patient)entity!;
                    // Map allowed fields
                    patient.Name = model.Name;
                    patient.SurName = model.SurName;
                    patient.DateOfBirth = model.DateOfBirth;
                    patient.IDNumber = model.IDNumber;
                    patient.PhoneNumber = model.PhoneNumber;
                    patient.Address = model.Address;
                    patient.City = model.City;
                    patient.EmergencyContactName = model.EmergencyContactName;
                    patient.EmergencyContactPhone = model.EmergencyContactPhone;

                    _context.Update(patient);
                }
                else if (string.Equals(model.EntityKind, "Employee", StringComparison.OrdinalIgnoreCase))
                {
                    var employee = (CareDev.Models.Employee)entity!;
                    // Map allowed fields
                    employee.Name = model.Name;
                    employee.SurName = model.SurName;
                    employee.DateOfBirth = model.DateOfBirth;
                    employee.IDNumber = model.IDNumber;
                    employee.PhoneNumber = model.PhoneNumber;
                    employee.Address = model.Address;
                    employee.City = model.City;

                    // Only allow JobTitle/Department edits if user is HR or Admin
                    if (User.IsInRole("HR") || User.IsInRole("Admin"))
                    {
                        employee.JobTitle = model.JobTitle;
                        employee.Department = model.Department;
                    }

                    _context.Update(employee);
                }
                else
                {
                    return Forbid();
                }

                await _context.SaveChangesAsync();
                TempData["success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency issue updating profile for user {User}", User.Identity?.Name);
                ModelState.AddModelError("", "Another user updated your profile. Please reload and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {User}", User.Identity?.Name);
                ModelState.AddModelError("", "An error occurred while saving your profile. Please try again.");
            }

            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var au = uid != null ? await _userManager.FindByIdAsync(uid) : null;
            ViewBag.Email = au?.Email;
            return View(model);
        }

        // ---- helper methods ----

        // returns ("Patient", Patient) or ("Employee", Employee) or (null, null)
        private async Task<(string? kind, object? entity)> GetCurrentProfileAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return (null, null);

            // try patient
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
            if (patient != null) return ("Patient", patient);

            // then employee
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
            if (employee != null) return ("Employee", employee);

            return (null, null);
        }

        // maps the entity to the viewmodel
        private async Task<UserProfileViewModel> MapToViewModelAsync(string kind, object entity)
        {
            var vm = new UserProfileViewModel { EntityKind = kind };

            if (string.Equals(kind, "Patient", StringComparison.OrdinalIgnoreCase))
            {
                var p = (CareDev.Models.Patient)entity;
                vm.EntityId = p.PatientId;
                vm.Name = p.Name;
                vm.SurName = p.SurName;
                vm.DateOfBirth = p.DateOfBirth;
                vm.IDNumber = p.IDNumber;
                vm.PhoneNumber = p.PhoneNumber;
                vm.Address = p.Address;
                vm.City = p.City;
                vm.EmergencyContactName = p.EmergencyContactName;
                vm.EmergencyContactPhone = p.EmergencyContactPhone;
            }
            else if (string.Equals(kind, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                var e = (CareDev.Models.Employee)entity;
                vm.EntityId = e.EmployeeId;
                vm.Name = e.Name;
                vm.SurName = e.SurName;
                vm.DateOfBirth = e.DateOfBirth;
                vm.IDNumber = e.IDNumber;
                vm.PhoneNumber = e.PhoneNumber;
                vm.Address = e.Address;
                vm.City = e.City;
                vm.JobTitle = e.JobTitle;
                vm.Department = e.Department;
            }

            return vm;
        }
    }
}

