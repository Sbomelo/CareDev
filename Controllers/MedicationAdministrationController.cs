using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;

namespace CareDev.Controllers
{
    //[Authorize(Roles = "Nurse,NursingSister")]
    public class MedicationAdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // Updated here

        public MedicationAdministrationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int dispenseId)
        {
            var dispensation = await _context.MedicationDispensations
                .Include(m => m.Patient) // Make sure MedicationDispensation.Patient is of type ApplicationUser
                .FirstOrDefaultAsync(d => d.Id == dispenseId);

            if (dispensation == null)
                return NotFound();

            var model = new AdministerMedicationVM
            {
                DispenseId = dispenseId,
                MedicationName = dispensation.MedicationName,
                TimeGiven = DateTime.Now
            };

            ViewData["PatientName"] = dispensation.Patient.Name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdministerMedicationVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var dispensation = await _context.MedicationDispensations
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(d => d.Id == model.DispenseId);

            if (dispensation == null)
                return NotFound();

            if (dispensation.ScheduleLevel >= 5 && roles.Contains("Nurse"))
            {
                ModelState.AddModelError("", "You are not authorized to administer Schedule 5 or higher medications.");
                model.MedicationName = dispensation.MedicationName;
                ViewData["PatientName"] = dispensation.Patient.Name;
                return View(model);
            }

            var entity = new AdministerMeds
            {
                DispenseId = model.DispenseId,
                TimeGiven = model.TimeGiven,
                Observations = model.Observations,
                AdverseReactions = model.AdverseReactions,
                AdministeredById = user.Id // ApplicationUser Id
            };

            _context.AdministeredMeds.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "MedicationDispensation", new { patientUserId = dispensation.PatientUserId });
        }
    }
}
