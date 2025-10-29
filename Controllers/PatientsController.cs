using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;

namespace CareDev.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, ILogger<PatientsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        //Patent Portal
        [Authorize(Roles = "Patient")]
        public IActionResult PatientDashboard()
        {
            return View();
        }
        [Authorize(Roles = "Patient")]
        public IActionResult MedicalInformation()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public IActionResult ScheduledEvents()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public IActionResult TrackJourney()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public IActionResult Communicate()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public IActionResult WriteFeedBack()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public IActionResult PatientPortal()
        {
            return View();
        }

        // GET: Patients
        [Authorize(Roles = "WardAdmin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Patients.Include(p => p.Allergy).Include(p => p.ApplicationUser).Include(p => p.ChronicCondition).Include(p => p.Medications);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Patients/Details/5
        [Authorize(Roles = "WardAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Allergy)
                .Include(p => p.ApplicationUser)
                .Include(p => p.ChronicCondition)
                .Include(p => p.Medications)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        [Authorize(Roles = "WardAdmin")]
        public IActionResult Create()
        {
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name");
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name");
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "WardAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Name,SurName,Age,Gender,PhoneNumber,MedicationId,AllergyId,ChronicConditionId,ApplicationUserId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                TempData["success"] = "Patient record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patient.AllergyId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", patient.ApplicationUserId);
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patient.ChronicConditionId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", patient.MedicationId);
            TempData["error"] = "Error creating patient record. Please check the details and try again.";
            return View(patient);
        }

        // GET: Patients/Edit/5
        [Authorize(Roles = "WardAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patient.AllergyId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", patient.ApplicationUserId);
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patient.ChronicConditionId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", patient.MedicationId);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "WardAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,Name,SurName,Age,Gender,PhoneNumber,MedicationId,AllergyId,ChronicConditionId,ApplicationUserId")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Patient record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patient.AllergyId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", patient.ApplicationUserId);
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patient.ChronicConditionId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", patient.MedicationId);
            TempData["error"] = "Error updating patient record. Please check the details and try again.";
            return View(patient);
        }

        //Get Patient/Register
        [Authorize(Roles = "WardAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var vm = new Models.ViewModels.PatientRegisterViewModel
            {
                Medications =await _context.Medications.
                  Select(m => new SelectListItem { Value = m.MedicationId.ToString(), Text = m.Name })
                  .ToListAsync(),
                Allergies =await _context.Allergies
                      .Select(a => new SelectListItem { Value = a.AllergyId.ToString(), Text = a.Name })
                      .ToListAsync(),
                ChronicConditions =await _context.ChronicConditions
                      .Select(c => new SelectListItem { Value = c.ChronicConditionId.ToString(), Text = c.Name })
                      .ToListAsync()
            };
            return View(vm);

        }

        //POST Patient/Register
        [Authorize(Roles = "WardAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Models.ViewModels.PatientRegisterViewModel vm)
        {
            // Optional debug logging of posted form values (remove in production)
            _logger.LogDebug("POST Patients/Register called. Content-Type: {ct}", Request.ContentType);
            if (Request.HasFormContentType)
            {
                foreach (var kv in Request.Form)
                    _logger.LogDebug("FORM: {Key} = {Value}", kv.Key, kv.Value);
            }

            // If model is NOT valid -> repopulate dropdowns and return view
            if (!ModelState.IsValid)
            {
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

            // Ensure the Patient role exists in Identity
            if (!await _roleManager.RoleExistsAsync("Patient"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Patient"));
            }

            // Prevent duplicate email
            if (await _userManager.FindByEmailAsync(vm.Email) != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email already in use.");
                await PopulateDropdowns(vm);
                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create Identity user (ApplicationUser)
                var user = new ApplicationUser
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                    Name = vm.Name,
                    SurName = vm.Surname,
                    Age = vm.Age,
                    Gender = vm.Gender,
                    MedicationId = vm.MedicationId,
                    AllergyId = vm.AllergyId,
                    ChronicConditionId = vm.ChronicConditionId,
                    PhoneNumber = vm.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                    MustChangePassword = false // keep current behaviour; set true if you want them to change at first login
                };

                var createUser = await _userManager.CreateAsync(user, vm.Password);
                if (!createUser.Succeeded)
                {
                    foreach (var e in createUser.Errors)
                        ModelState.AddModelError("", e.Description);

                    await PopulateDropdowns(vm);
                    return View(vm);
                }

                // Assign the Patient role (only once)
                var addRoleResult = await _userManager.AddToRoleAsync(user, "Patient");
                if (!addRoleResult.Succeeded)
                {
                    // Roll back Identity user if role assignment fails
                    await _userManager.DeleteAsync(user);
                    foreach (var e in addRoleResult.Errors)
                        ModelState.AddModelError("", e.Description);

                    await PopulateDropdowns(vm);
                    return View(vm);
                }

                // Create Patient domain record and link to ApplicationUser
                var patient = new Patient
                {
                    Name = vm.Name,
                    SurName = vm.Surname,
                    Age = vm.Age,
                    Gender = vm.Gender,
                    MedicationId = vm.MedicationId,
                    AllergyId = vm.AllergyId,
                    ChronicConditionId = vm.ChronicConditionId,
                    PhoneNumber = vm.PhoneNumber,
                    ApplicationUserId = user.Id
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                TempData["success"] = "Registration successful. Patient account created.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error registering patient");
                ModelState.AddModelError("", "An error occurred while processing your registration. Please try again.");
                await PopulateDropdowns(vm);
                return View(vm);
            }
        }


        //Helper method to repopulate dropdowns
        private async Task PopulateDropdowns(Models.ViewModels.PatientRegisterViewModel vm)
        {
            vm.Medications =await _context.Medications.Select(m => new SelectListItem { Value = m.MedicationId.ToString(), Text = m.Name }).ToListAsync();
            vm.Allergies =await _context.Allergies.Select(a => new SelectListItem { Value = a.AllergyId.ToString(), Text = a.Name }).ToListAsync();
            vm.ChronicConditions =await _context.ChronicConditions.Select(c => new SelectListItem { Value = c.ChronicConditionId.ToString(), Text = c.Name }).ToListAsync();
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Allergy)
                .Include(p => p.ApplicationUser)
                .Include(p => p.ChronicCondition)
                .Include(p => p.Medications)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [Authorize(Roles = "WardAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Patient record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
