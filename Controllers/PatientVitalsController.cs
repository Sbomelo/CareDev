using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;

namespace CareDev.Controllers
{
    public class PatientVitalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientVitalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Patients()
        {
            var patients = await _userManager.GetUsersInRoleAsync("Patient");
            return View(patients);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string patientUserId)
        {
            var patient = await _userManager.FindByIdAsync(patientUserId);
            if (patient == null) return NotFound();

            var nurse = await _userManager.GetUserAsync(User);
            if (nurse == null) return Unauthorized();

            var model = new PatsVitalsVM
            {
                PatientUserId = patient.Id,
                NurseUserId = nurse.Id
            };

            ViewData["PatientName"] = patient.Name ?? patient.UserName;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatsVitalsVM model)
        {
            if (!ModelState.IsValid)
            {
                var patient = await _userManager.FindByIdAsync(model.PatientUserId);
                ViewData["PatientName"] = patient?.Name ?? "Unknown";
                return View(model);
            }

            var entity = new PatsVitals
            {
                PatientUserId = model.PatientUserId,
                NurseUserId = model.NurseUserId,
                Temperature = model.Temperature,
                BloodPressure = model.BloodPressure,
                HeartRate = model.HeartRate,
                RespiratoryRate = model.RespiratoryRate,
                OxygenSaturation = model.OxygenSaturation,
                GlucoseLevel = model.GlucoseLevel,
                RecordedDate = model.RecordedDate
            };

            _context.PatsVitals.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { patientUserId = model.PatientUserId });
        }

        public async Task<IActionResult> Details(string patientUserId)
        {
            if (string.IsNullOrEmpty(patientUserId)) return NotFound();

            var patient = await _userManager.FindByIdAsync(patientUserId);
            if (patient == null) return NotFound();

            var vitals = await _context.PatsVitals
                .Include(v => v.Nurse)
                .Include(v => v.Patient)
                .Where(v => v.PatientUserId == patientUserId)
                .OrderByDescending(v => v.RecordedDate)
                .ToListAsync();

            if (!vitals.Any())
                ViewData["Message"] = "No vitals recorded yet.";

            ViewData["PatientName"] = patient.Name ?? patient.UserName;
            return View(vitals);
        }
    }
}
