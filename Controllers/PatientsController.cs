using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;

namespace CareDev.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Patients.Include(p => p.Allergy).Include(p => p.ApplicationUser).Include(p => p.ChronicCondition).Include(p => p.Medications);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Patients/Details/5
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Name,SurName,Age,Gender,PhoneNumber,MedicationId,AllergyId,ChronicConditionId,ApplicationUserId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patient.AllergyId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", patient.ApplicationUserId);
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patient.ChronicConditionId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", patient.MedicationId);
            return View(patient);
        }

        // GET: Patients/Edit/5
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patient.AllergyId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", patient.ApplicationUserId);
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patient.ChronicConditionId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", patient.MedicationId);
            return View(patient);
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
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
