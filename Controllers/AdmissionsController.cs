using CareDev.Data;
using CareDev.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace CareDev.Controllers
{
    [Authorize(Roles = "WardAdmin")]
    public class AdmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly ILogger<AdmissionsController> _logger;

        public AdmissionsController(
         ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AdmissionsController> logger

            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Admissions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Admissions.Include(a => a.Bed).Include(a => a.Employee).Include(a => a.Patient).Include(a => a.Ward);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admission = await _context.Admissions
                .Include(a => a.Bed)
                .Include(a => a.Employee)
                .Include(a => a.Patient)
                .Include(a => a.Ward)
                .FirstOrDefaultAsync(m => m.AdmissionId == id);
            if (admission == null)
            {
                return NotFound();
            }

            return View(admission);
        }

        // GET: Admissions/Create
        public IActionResult Create()
        {
            ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name");
            return View();
        }

        // POST: Admissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdmitPatientViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // TODO: re-populate dropdown lists on vm before returning the view
                await PopulateLookupLists(vm);
                return View(vm);
            }

            // Ensure Patient role exists
            if (!await _roleManager.RoleExistsAsync("Patient"))
                await _roleManager.CreateAsync(new IdentityRole("Patient"));

            // Prevent duplicate emails
            if (await _userManager.FindByEmailAsync(vm.Email) != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email already in use");
                await PopulateLookupLists(vm);
                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generate temporary password (or use vm.TempPassword if admin provided)
                var tempPassword = GenerateTemporaryPassword(12);

                // Create ApplicationUser with temporary password
                var appuser = new ApplicationUser
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                    Name = vm.Name,
                    SurName = vm.SurName,
                    Age = vm.Age ?? 0,
                    Gender = vm.Gender,
                    MedicationId = vm.MedicationId,
                    AllergyId = vm.AllergyId,
                    ChronicConditionId = vm.ChronicConditionId,
                    MustChangePassword = true,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(appuser, tempPassword);
                if (!createUserResult.Succeeded)
                {
                    foreach (var e in createUserResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);

                    await PopulateLookupLists(vm);
                    return View(vm);
                }

                // Assign Patient role
                var roleResult = await _userManager.AddToRoleAsync(appuser, "Patient");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(appuser); // rollback
                    foreach (var e in roleResult.Errors) ModelState.AddModelError("", e.Description);
                    await PopulateLookupLists(vm);
                    return View(vm);
                }

                // Create Patient domain record and link to user
                var patient = new Patient
                {
                    Name = vm.Name,
                    SurName = vm.SurName,
                    Age = vm.Age ?? 0,
                    Gender = vm.Gender,
                    MedicationId = vm.MedicationId,
                    AllergyId = vm.AllergyId,
                    ChronicConditionId = vm.ChronicConditionId,
                    ApplicationUserId = appuser.Id
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                // Create Admission record
                var admission1 = new Admission
                {
                    PatientId = patient.PatientId,
                    EmployeeId = vm.EmployeeId,
                    WardId =(int)vm.WardId,
                    BedId = vm.BedId ?? 0,
                    AdmissionDate = DateTime.Now,
                    AdmissionReason = vm.AdmissionReason
                };
                _context.Admissions.Add(admission1);

                // Update bed status to occupied (Status true = available; we mark false = occupied)
                if (vm.BedId.HasValue)
                {
                    var bed = await _context.Beds.FindAsync(vm.BedId.Value);
                    if (bed == null)
                    {
                        ModelState.AddModelError("", "Selected bed not found.");
                        // rollback created user & patient
                        await _userManager.DeleteAsync(appuser);
                        await tx.RollbackAsync();
                        await PopulateLookupLists(vm);
                        return View(vm);
                    }

                    if (!bed.Status) // already occupied
                    {
                        ModelState.AddModelError("", "Selected bed is not available.");
                        await _userManager.DeleteAsync(appuser);
                        await tx.RollbackAsync();
                        await PopulateLookupLists(vm);
                        return View(vm);
                    }

                    bed.Status = false; // mark occupied
                    _context.Beds.Update(bed);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // Communicate temp password securely to admin (display once or print slip)
                TempData["TempPassword"] = tempPassword;
                TempData["Success"] = "Patient admitted successfully. Temporary password has been generated.";

                return RedirectToAction("Portal", "Patients");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error admitting patient");
                ModelState.AddModelError("", "An error occurred while admitting the patient. Please try again.");
                await PopulateLookupLists(vm);
                return View(vm);
            }
        }
        private async Task PopulateLookupLists(AdmitPatientViewModel vm)
        {
           

            // Allergies
            vm.Allergies = await _context.Allergies
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem { Value = a.AllergyId.ToString(), Text = a.Name })
                .ToListAsync();

            // Chronic conditions / Conditions
            vm.ChronicConditions = await _context.ChronicConditions
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.ChronicConditionId.ToString(), Text = c.Name })
                .ToListAsync();

            // Medications
            vm.Medications = await _context.Medications
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Select(m => new SelectListItem { Value = m.MedicationId.ToString(), Text = m.Name })
                .ToListAsync();

            // Wards
            vm.Wards = await _context.Wards
                .AsNoTracking()
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem { Value = w.WardId.ToString(), Text = w.Name })
                .ToListAsync();

            // Beds: only available beds (assuming Bed.Status == true means available)
            vm.Beds = await _context.Beds
                .AsNoTracking()
                .Where(b => b.Status) // true = available
                .OrderBy(b => b.BedId)
                .Select(b => new SelectListItem
                {
                    Value = b.BedId.ToString(),
                    Text = $"Bed {b.BedId}" // change to b.Number or b.Label if you have one
                })
                .ToListAsync();

            // Doctors (if you have a Doctors table)
            vm.Employees = await _context.Employees
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem { Value = d.EmployeeId.ToString(), Text = d.Name })
                .ToListAsync();

           
            vm.Employees = await _context.Employees
                .AsNoTracking()
                .OrderBy(e => e.Name)
                .Select(e => new SelectListItem { Value = e.EmployeeId.ToString(), Text = e.Name })
                .ToListAsync();

            // Set the selected values if vm already has them (useful when re-displaying after validation error)
            // Note: Razor helpers will bind selected value from vm properties (e.g. vm.WardId),
            // so you don't need to set Selected on SelectListItem explicitly unless you build SelectList manually.
        }

        private static string GenerateTemporaryPassword(int length = 12)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()-_=+[]{}<>?";

            var all = upper + lower + digits + symbols;
            var chars = new char[length];

            // Fill with random characters
            for (int i = 0; i < length; i++)
            {
                int idx = RandomNumberGenerator.GetInt32(all.Length);
                chars[i] = all[idx];
            }

            var pwd = new string(chars);

            // Ensure at least one from each required group exists (simple guarantee)
            // Replace random positions with required types if missing
            if (!pwd.Any(char.IsUpper))
            {
                var pos = RandomNumberGenerator.GetInt32(length);
                pwd = pwd.Remove(pos, 1).Insert(pos, upper[RandomNumberGenerator.GetInt32(upper.Length)].ToString());
            }
            if (!pwd.Any(char.IsLower))
            {
                var pos = RandomNumberGenerator.GetInt32(length);
                pwd = pwd.Remove(pos, 1).Insert(pos, lower[RandomNumberGenerator.GetInt32(lower.Length)].ToString());
            }
            if (!pwd.Any(char.IsDigit))
            {
                var pos = RandomNumberGenerator.GetInt32(length);
                pwd = pwd.Remove(pos, 1).Insert(pos, digits[RandomNumberGenerator.GetInt32(digits.Length)].ToString());
            }
            if (!pwd.Any(c => symbols.Contains(c)))
            {
                var pos = RandomNumberGenerator.GetInt32(length);
                pwd = pwd.Remove(pos, 1).Insert(pos, symbols[RandomNumberGenerator.GetInt32(symbols.Length)].ToString());
            }

            return pwd;
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("AdmissionId,AdmissionDate,DischargeDate,AdmissionReason,PatientId,EmployeeId,WardId,BedId,RoomId")] Admission admission, AdmitPatientViewModel vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(admission);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedId", admission.BedId);
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", admission.EmployeeId);
        //    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", admission.PatientId);
        //    ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", admission.WardId);

        //    //Ensure if role exists
        //    if (!await _roleManager.RoleExistsAsync("Patient"))
        //        await _roleManager.CreateAsync(new IdentityRole("Patient"));

        //    //prevent duplicate eamils
        //    if (await _userManager.FindByEmailAsync(vm.Email) != null)
        //    {
        //        ModelState.AddModelError(nameof(vm.Email), "Email already in use");
        //        return View(vm);
        //    }

        //    using var tx = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        //generate temporary password
        //        var tempPassword = GenerateTemporaryPassword(6);

        //        //Create application User with temporary password
        //        var appuser = new ApplicationUser
        //        {
        //            UserName = vm.Email,
        //            Email = vm.Email,
        //            Name = vm.Name,
        //            SurName = vm.SurName,
        //            Age = vm.Age,
        //            Gender = vm.Gender,
        //            MedicationId = vm.MedicationId,
        //            AllergyId = vm.AllergyId,
        //            ChronicConditionId = vm.ChronicConditionId,
        //            MustChangePassword = true,
        //            EmailConfirmed = true

        //        };
        //        var createUserResult = await _userManager.CreateAsync(appuser, tempPassword);

        //        if (!createUserResult.Succeeded)
        //        {
        //            foreach (var e in createUserResult.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, e.Description);
        //            }
        //            return View(vm);
        //        }

        //        //assign role to user
        //        var RoleResult = await _userManager.AddToRoleAsync(appuser, "Patient");
        //        if (!RoleResult.Succeeded)
        //        {
        //            await _userManager.DeleteAsync(appuser); //Rollback user creation if role assignment fails

        //            foreach (var e in RoleResult.Errors)
        //            {
        //                ModelState.AddModelError("", e.Description);
        //            }
        //            return View(vm);
        //        }

        //        //Create Patient domain record and link to user
        //        var patient = new Patient
        //        {
        //            Name = vm.Name,
        //            SurName = vm.SurName,
        //            Age = vm.Age,
        //            Gender = vm.Gender,
        //            MedicationId = vm.MedicationId,
        //            AllergyId = vm.AllergyId,
        //            ChronicConditionId = vm.ChronicConditionId,
        //            ApplicationUserId = appuser.Id
        //        };
        //        _context.Patients.Add(patient);
        //        await _context.SaveChangesAsync();

        //        //Create admission record and link to patient
        //        var admission1 = new Admission
        //        {
        //            PatientId = patient.PatientId,
        //            EmployeeId = vm.EmployeeId,
        //            WardId = vm.WardId,
        //            BedId = (int)vm.BedId,
        //            AdmissionDate = DateTime.Now,
        //            AdmissionReason = vm.AdmissionReason,

        //        };

        //        _context.Admissions.Add(admission1);

        //        //Update bed status to occupied
        //        if (vm.BedId.HasValue)
        //        {
        //            var bed = await _context.Beds.FindAsync(vm.BedId.Value);

        //            if (bed != null)
        //            {
        //                if (!bed.Status)
        //                {
        //                    ModelState.AddModelError("", "Selected bed is not available");

        //                    await _userManager.DeleteAsync(appuser); //Rollback user creation if bed is not available
        //                    return View(vm);
        //                }
        //                bed.Status = true;
        //                _context.Beds.Update(bed);
        //            }
        //        }

        //        await _context.SaveChangesAsync();
        //        await tx.CommitAsync();

        //        //Send email to patient with temporary password
        //        TempData["TempPassword"] = tempPassword;
        //        TempData["Success"] = "Patient admitted successfully. Temporary password has been generated.";

        //        return RedirectToAction("Portal", "Patients");
        //    }

        //    catch (Exception ex)
        //    {
        //        await tx.RollbackAsync();
        //        _logger.LogError(ex, "Error admitting patient");
        //        ModelState.AddModelError("", "An error occurred while admitting the patient. Please try again.");


        //        return View(admission);
        //    }
        //}

        //    //Secure-ish temporary password generator
        //  private static string GenerateTemporaryPassword(int length = 12)
        //  {
        //    const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //    const string lower = "abcdefghijklmnopqrstuvwxyz";
        //    const string digits = "0123456789";
        //    const string symbols = "!@#$%^&*()-_=+[]{}<>?";

        //    var all = upper + lower + digits + symbols;
        //    var rnd = RandomNumberGenerator.Create();
        //    var bytes = new byte[length];
        //    rnd.GetBytes(bytes);
        //    var sb = new StringBuilder(length);
        //    for (int i = 0; i < length; i++)
        //    {
        //        var idx = bytes[i] % all.Length;
        //        sb.Append(all[idx]);
        //    }

        //    // Ensure at least one char from each set is present
        //    var result = sb.ToString();
        //    if (!result.Any(char.IsUpper)) result = upper[rnd.GetBytes(1)[0] % upper.Length] + result.Substring(1); 
        //    if (!result.Any(char.IsLower)) result = result.Substring(0, 1) + lower[rnd.GetBytes(1)[0] % lower.Length] + result.Substring(2);
        //    if (!result.Any(char.IsDigit)) result = result.Substring(0, 2) + digits[rnd.GetBytes(1)[0] % digits.Length] + result.Substring(3);
        //    if (!result.Any(c => symbols.Contains(c))) result = result.Substring(0, 3) + symbols[rnd.GetBytes(1)[0] % symbols.Length] + result.Substring(4);

        //    return result;
        //  }


        // GET: Admissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admission = await _context.Admissions.FindAsync(id);
            if (admission == null)
            {
                return NotFound();
            }
            ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedId", admission.BedId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", admission.EmployeeId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", admission.PatientId);
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", admission.WardId);
            return View(admission);
        }

        // POST: Admissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdmissionId,AdmissionDate,DischargeDate,AdmissionReason,PatientId,EmployeeId,WardId,BedId,RoomId")] Admission admission)
        {
            if (id != admission.AdmissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdmissionExists(admission.AdmissionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedId", admission.BedId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", admission.EmployeeId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", admission.PatientId);
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", admission.WardId);
            return View(admission);
        }

        // GET: Admissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admission = await _context.Admissions
                .Include(a => a.Bed)
                .Include(a => a.Employee)
                .Include(a => a.Patient)
                .Include(a => a.Ward)
                .FirstOrDefaultAsync(m => m.AdmissionId == id);
            if (admission == null)
            {
                return NotFound();
            }

            return View(admission);
        }

        // POST: Admissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admission = await _context.Admissions.FindAsync(id);
            if (admission != null)
            {
                _context.Admissions.Remove(admission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdmissionExists(int id)
        {
            return _context.Admissions.Any(e => e.AdmissionId == id);
        }
    }
}
