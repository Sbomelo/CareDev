using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareDev.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ILogger<EmployeesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPortal()
        {
            return View();
        }

        [Authorize(Roles = "WardAdmin")]
        public async Task<IActionResult> WardAdminPortal() 
        {
            return View();
        }

        //[Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DoctorPortal()
        {
            return View();
        }

        //[Authorize(Roles = "Nurse")]
        public async Task<IActionResult> NursePortal()
        {
            return View("Employee/NursePortal");
        }

        // GET: Employees
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Name,SurName,Age,Gender,PhoneNumber,Email,Active")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["success"] = "Employee record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Error creating employee record. Please check the details and try again.";
            return View(employee);
        }

        //// GET: Employees/Edit/
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var employee = await _context.Employees.FindAsync(id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(employee);
        //}

        ////// POST: Employees/Edit/5
        [Authorize(Roles = "Admin")]
        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == id.Value);

            if (employee == null) return NotFound();

            var vm = new EmployeeEditViewModel
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                SurName = employee.SurName,
                Age = employee.Age,
                Gender = employee.Gender,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                RoleId = employee.RoleId
            };

            await PopulateRoles(vm);
            return View(vm);
        }

        // POST: Employees/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel vm)
        {
            _logger.LogInformation("Entered POST Edit action for EmployeeId {Id}", id);
            if (id != vm.EmployeeId) return BadRequest();
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState invalid during Employee Edit for ID {Id}", id);
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        _logger.LogWarning("Field {Field} error: {Error}", kvp.Key, err.ErrorMessage);
                    }
                }

                await PopulateRoles(vm);
                // Add a message so you can see that validation failed
                ViewData["ValidationMessage"] = "Model validation failed — check required fields.";
                return View(vm);
            }

            //_logger.LogInformation("Entered POST Edit. EmployeeId={Id}", id);

            //// Log the raw form data so we can see what actually posted
            //if (Request.HasFormContentType)
            //{
            //    foreach (var kv in Request.Form)
            //        _logger.LogDebug("FORM KEY: {Key} = {Value}", kv.Key, kv.Value);
            //}

            //// If ModelState invalid, collect detailed errors and show them in the view
            //if (!ModelState.IsValid)
            //{
            //    _logger.LogWarning("ModelState invalid during Employee Edit for ID {Id}", id);

            //    // Build a list of friendly validation messages
            //    var modelErrors = new List<string>();
            //    foreach (var kvp in ModelState)
            //    {
            //        foreach (var err in kvp.Value.Errors)
            //        {
            //            var fieldName = kvp.Key;
            //            var errorMsg = string.IsNullOrWhiteSpace(err.ErrorMessage)
            //                ? (err.Exception?.Message ?? "Unknown error")
            //                : err.ErrorMessage;

            //            _logger.LogWarning("Field {Field}: {Error}", fieldName, errorMsg);
            //            modelErrors.Add($"{fieldName}: {errorMsg}");
            //        }
            //    }

            //    // Expose the errors to the view (for debugging)
            //    ViewBag.ModelErrors = modelErrors;
            //    // show a summary notice as well
            //    ViewData["ValidationMessage"] = "Model validation failed — see the list below.";

            //    return View(vm);
            //}


            // Validate RoleId exists
            var roleEntity = await _context.Roles.FindAsync(vm.RoleId);
            if (roleEntity == null)
            {
                ModelState.AddModelError(nameof(vm.RoleId), "Selected role not found.");
                await PopulateRoles(vm);
                return View(vm);
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null) return NotFound();

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update domain fields
                employee.Name = vm.Name;
                employee.SurName = vm.SurName;
                employee.Age = vm.Age ?? employee.Age;
                employee.Gender = vm.Gender;
                employee.PhoneNumber = vm.PhoneNumber;
                employee.Email = vm.Email;
                employee.RoleId = vm.RoleId;

                _context.Employees.Update(employee);
                await _context.SaveChangesAsync(); // persist domain change

                // Sync Identity user (if linked)
                if (!string.IsNullOrEmpty(employee.ApplicationUserId))
                {
                    var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
                    if (user != null)
                    {
                        var userNeedsUpdate = false;

                        if (user.Email != vm.Email)
                        {
                            user.Email = vm.Email;
                            user.UserName = vm.Email;
                            userNeedsUpdate = true;
                        }

                        if (user.PhoneNumber != vm.PhoneNumber)
                        {
                            user.PhoneNumber = vm.PhoneNumber;
                            userNeedsUpdate = true;
                        }

                        if (userNeedsUpdate)
                        {
                            var updateResult = await _userManager.UpdateAsync(user);
                            if (!updateResult.Succeeded)
                            {
                                foreach (var err in updateResult.Errors)
                                    ModelState.AddModelError("", err.Description);

                                await PopulateRoles(vm);
                                return View(vm);
                            }
                        }

                        // Sync Identity role membership
                        var currentRoles = await _userManager.GetRolesAsync(user); // list<string>
                        var targetRoleName = roleEntity.RoleName;

                        if (!currentRoles.Contains(targetRoleName))
                        {
                            // remove existing roles (or remove only specific roles depending on your policy)
                            if (currentRoles.Count > 0)
                                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                            var addRes = await _userManager.AddToRoleAsync(user, targetRoleName); // ensure correct variable name
                            if (!addRes.Succeeded)
                            {
                                foreach (var err in addRes.Errors)
                                    ModelState.AddModelError("", err.Description);

                                await PopulateRoles(vm);
                                return View(vm);
                            }
                        }
                    }
                }

                await tx.CommitAsync();
                TempData["Success"] = "Employee updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error updating employee.");
                ModelState.AddModelError("", "An error occurred while updating the employee.");
                await PopulateRoles(vm);
                return View(vm);
            }
        }

        // Helper: populate roles selectlist (value = RoleId)
        private async Task PopulateRoles(EmployeeEditViewModel vm)
        {
            vm.Roles = await _context.Roles
                .OrderBy(r => r.RoleName)
                .Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName })
                .ToListAsync();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,Name,SurName,Age,Gender,PhoneNumber,Email,Active")] Employee employee)
        //{
        //    if (id != employee.EmployeeId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(employee);
        //            await _context.SaveChangesAsync();

        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!EmployeeExists(employee.EmployeeId))
        //            {

        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }                 
        //        TempData["success"] = "Employee record updated successfully.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    TempData["error"] = "Error updating employee record. Please try again.";
        //    return View(employee);
        //}

        // GET: Employees/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        //Get Employee Register
        [Authorize(Roles = "WardAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var vm = new Models.ViewModels.EmployeeRegisterViewModel
            {
                Roles = await _context.Roles.
                  Select(r => new SelectListItem { Value = r.RoleName .ToString(), Text = r.RoleName })
                  .ToListAsync()
            };
            return View(vm);

        }

        //Post Employee Register
        [Authorize(Roles = "WardAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Models.ViewModels.EmployeeRegisterViewModel vm)
        {
            // debug: log incoming form keys (temporary — remove after debugging)
            _logger.LogDebug("POST Employees/Register called. Content-Type: {ct}", Request.ContentType);
            if (Request.HasFormContentType)
            {
                foreach (var kv in Request.Form)
                    _logger.LogDebug("FORM: {Key} = {Value}", kv.Key, kv.Value);
            }

            // If model is NOT valid -> repopulate dropdowns and return the view
            if (!ModelState.IsValid)
            {
                // log ModelState errors for easier debugging
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error - {Field}: {Error}", kvp.Key, err.ErrorMessage);
                    }
                }

                await PopulateDropdowns(vm);
                return View(vm);
            }

            // PREVENT DUPLICATE EMAILS
            if (await _userManager.FindByEmailAsync(vm.Email) != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email already in use.");
                await PopulateDropdowns(vm);
                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Ensure Identity role exists
                if (!await _roleManager.RoleExistsAsync(vm.RoleName))
                    await _roleManager.CreateAsync(new IdentityRole(vm.RoleName));

                // Ensure domain Role entity exists (Role table)
                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == vm.RoleName);
                if (roleEntity == null)
                {
                    roleEntity = new Role { RoleName = vm.RoleName };
                    _context.Roles.Add(roleEntity);
                    await _context.SaveChangesAsync();
                }

                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                    Name = vm.Name,
                    SurName = vm.Surname,
                    Age = vm.Age ?? 0,
                    Gender = vm.Gender,
                    PhoneNumber = vm.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user, vm.Password);
                if (!createUserResult.Succeeded)
                {
                    foreach (var e in createUserResult.Errors)
                        ModelState.AddModelError("", e.Description);

                    await PopulateDropdowns(vm);
                    return View(vm);
                }

                // Assign Identity role (only once)
                var addToRoleResult = await _userManager.AddToRoleAsync(user, vm.RoleName);
                if (!addToRoleResult.Succeeded)
                {
                    // rollback user if role assignment fails
                    await _userManager.DeleteAsync(user);
                    foreach (var e in addToRoleResult.Errors)
                        ModelState.AddModelError("", e.Description);

                    await PopulateDropdowns(vm);
                    return View(vm);
                }

                // Create Employee domain record and link to user
                var employee = new Employee
                {
                    Name = vm.Name,
                    SurName = vm.Surname,
                    Age = vm.Age ?? 0,
                    Gender = vm.Gender,
                    RoleId = roleEntity.RoleId,
                    PhoneNumber = vm.PhoneNumber,
                    Email = vm.Email,
                    ApplicationUserId = user.Id,
                    Active = true
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["success"] = $"Employee '{employee.Name} {employee.SurName}' Registered Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error during employee registration");
                ModelState.AddModelError("", "An error occurred while processing your registration. Please try again.");
                await PopulateDropdowns(vm);
                return View(vm);
            }
        }

        //Helper method to repopulate dropdowns
        private async Task PopulateDropdowns(Models.ViewModels.EmployeeRegisterViewModel vm)
        {
            vm.Roles = await _roleManager.Roles.Select(r => new SelectListItem { Value =r.Name, Text = r.Name }).ToListAsync();
            
        }


        // POST: Employees/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Employee record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
