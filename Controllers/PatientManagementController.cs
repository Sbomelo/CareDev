using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CareDev.Controllers
{
    [Authorize(Roles = "Nurse")]
    public class PatientManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public PatientManagementController(UserManager<ApplicationUser> userManager,
                                  RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View(new PatientCreateViewModel());
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                SurName = model.SurName,
                Age = model.Age,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Patient");
                TempData["SuccessMessage"] = "Patient added successfully!";
                return RedirectToAction("Index"); // list of patients
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var patientRole = await _roleManager.Roles
                .Where(r => r.Name == "Patient")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var patients = await _userManager.GetUsersInRoleAsync("Patient");
            return View(patients); // IEnumerable<ApplicationUser>
        }
        // GET: Patients/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var patient = await _userManager.FindByIdAsync(id);
            if (patient == null)
                return NotFound();

            var model = new PatientEditViewModel
            {
                Id = patient.Id,
                Name = patient.Name,
                SurName = patient.SurName,
                Age = patient.Age,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email
            };

            return View(model);
        }

        // POST: Patients/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var patient = await _userManager.FindByIdAsync(model.Id);
            if (patient == null)
                return NotFound();

            patient.Name = model.Name;
            patient.SurName = model.SurName;
            patient.Age = model.Age;
            patient.Gender = model.Gender;
            patient.PhoneNumber = model.PhoneNumber;
            patient.Email = model.Email;
            patient.UserName = model.Email;

            var result = await _userManager.UpdateAsync(patient);

            if (result.Succeeded)
            {

                TempData["SuccessMessage"] = "Patient updated successfully!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        // GET: Patients/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            var patient = await _userManager.FindByIdAsync(id);
            return View(patient); // <-- pass ApplicationUser
        }

        // POST: Patients/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Delete any related patient records first
            var relatedPatients = _context.Patients
                                           .Where(p => p.ApplicationUserId == id);

            _context.Patients.RemoveRange(relatedPatients);
            await _context.SaveChangesAsync();

            // Now delete the user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Patient deleted successfully!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return RedirectToAction("Index");
        }

    }

}

