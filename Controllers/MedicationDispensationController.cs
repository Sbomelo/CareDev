using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareDev.Controllers
{
    //[Authorize(Roles = "Nurse,NursingSister")]
    public class MedicationDispensationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MedicationDispensationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // List of Patients to select from
        public async Task<IActionResult> SelectPatient()
        {
            var patients = await _userManager.GetUsersInRoleAsync("Patient");
            return View(patients);
        }

        // GET: MedicationDispensation/Create?patientUserId=...
        public async Task<IActionResult> Create(string patientUserId)
        {
            var patient = await _userManager.FindByIdAsync(patientUserId);
            if (patient == null)
                return NotFound("Patient not found.");

            ViewData["PatientName"] = patient.Name ?? patient.UserName;
            ViewData["PatientUserId"] = patient.Id;

            return View(new MedicationDispensationVM { PatientUserId = patient.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicationDispensationVM model)
        {
            var dispenser = await _userManager.GetUserAsync(User);

            if (dispenser == null)
            {
                ModelState.AddModelError("", "Could not identify dispenser user.");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(dispenser);
            if (roles.Contains("Nurse") && model.ScheduleLevel > 4)
            {
                ModelState.AddModelError("", "Nurses may only dispense medications up to Schedule 4.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var entity = new MedicationDispensation
            {
                PatientUserId = model.PatientUserId,   // string
                DispenserUserId = dispenser.Id,        // string
                MedicationName = model.MedicationName,
                Dosage = model.Dosage,
                Route = model.Route,
                Frequency = model.Frequency,
                ScheduleLevel = model.ScheduleLevel,
                Notes = model.Notes,
                TimeDispensed = DateTime.Now
            };

            _context.MedicationDispensations.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { patientUserId = model.PatientUserId });
        }

        public async Task<IActionResult> Details(string patientUserId)
        {
            var dispensedMeds = await _context.MedicationDispensations
                .Include(m => m.Dispenser)
                .Include(m => m.AdministerMeds)
                .Where(m => m.PatientUserId == patientUserId)
                .ToListAsync();

            var patient = await _userManager.FindByIdAsync(patientUserId);
            ViewData["PatientName"] = patient?.Name ?? "Unknown";
            ViewData["PatientId"] = patientUserId;

            return View(dispensedMeds);
        }

        public async Task<IActionResult> AdministerList(string patientUserId)
        {
            var patient = await _userManager.FindByIdAsync(patientUserId);
            if (patient == null)
                return NotFound("Patient not found.");

            var dispensedMeds = await _context.MedicationDispensations
                .Include(m => m.Dispenser)
                .Include(m => m.AdministerMeds)
                .ThenInclude(a => a.AdministeredBy)
                .Where(m => m.PatientUserId == patientUserId)
                .OrderByDescending(m => m.TimeDispensed)
                .ToListAsync();

            ViewData["PatientName"] = patient.Name ?? patient.UserName;
            ViewData["PatientId"] = patient.Id;

            return View(dispensedMeds);
        }

    }
}
