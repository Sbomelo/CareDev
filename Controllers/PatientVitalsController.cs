using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CareDev.Controllers
{
    //[Authorize(Roles = "Nurse,NursingSister")]
    public class PatientVitalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientVitalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List of all patients
        public async Task<IActionResult> Patients()
        {
            var patients = await _userManager.GetUsersInRoleAsync("Patient");
            return View(patients); // View must use IEnumerable<ApplicationUser>
        }

        // Show form to create new vitals for a patient
        [HttpGet]
        public async Task<IActionResult> Create(string patientUserId)
        {
            var patient = await _userManager.FindByIdAsync(patientUserId);
            if (patient == null) return NotFound();

            var nurse = await _userManager.GetUserAsync(User);
            if (nurse == null) return Unauthorized();

            var model = new PatientVitalsVM
            {
                PatientUserId = patient.Id,
                NurseUserId = nurse.Id,
                RecordedDate = DateTime.Now
            };

            ViewData["PatientName"] = patient.Name ?? patient.UserName;
            return View(model);
        }

        // Show patient vitals history
        public async Task<IActionResult> Details(string patientUserId)
        {
            var vitals = await _context.PatientVitals
                .Include(v => v.Nurse)
                .Include(v => v.Patient)
                .Where(v => v.PatientUserId == patientUserId)
                .OrderByDescending(v => v.RecordedDate)
                .ToListAsync();

            var patient = await _userManager.FindByIdAsync(patientUserId);
            if (patient == null) return NotFound();

            ViewData["PatientName"] = patient.Name ?? patient.UserName;
            return View(vitals);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientVitalsVM model)
        {
            if (!ModelState.IsValid)
            {
                var patient = await _userManager.FindByIdAsync(model.PatientUserId);
                ViewData["PatientName"] = patient?.Name ?? "Unknown";
                return View(model);
            }

            // Load existing users
            var patientExists = await _userManager.FindByIdAsync(model.PatientUserId);
            var nurseExists = await _userManager.GetUserAsync(User);

            if (patientExists == null || nurseExists == null)
            {
                ModelState.AddModelError("", "Invalid patient or nurse ID.");
                return View(model);
            }

            // Create entity
            var entity = new PatientVitals
            {
                PatientUserId = patientExists.Id,
                NurseUserId = nurseExists.Id,
                Temperature = model.Temperature,
                BloodPressure = model.BloodPressure,
                HeartRate = model.HeartRate,
                RespiratoryRate = model.RespiratoryRate,
                OxygenSaturation = model.OxygenSaturation,
                GlucoseLevel = model.GlucoseLevel,
                RecordedDate = model.RecordedDate
            };

            try
            {
                _context.PatientVitals.Add(entity);
                await _context.SaveChangesAsync(); // EF can now save without trying to create new users
                return RedirectToAction("Details", new { patientUserId = model.PatientUserId });
            }
            catch (Exception ex)
            {
                // Log the inner exception for details
                ModelState.AddModelError("", "An error occurred while saving: " + ex.InnerException?.Message ?? ex.Message);
                return View(model);
            }
        }

    }
}
