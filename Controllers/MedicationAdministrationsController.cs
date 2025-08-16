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
    public class MedicationAdministrationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicationAdministrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MedicationAdministrations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MedicationAdministrations.Include(m => m.Medication).Include(m => m.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MedicationAdministrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationAdministration = await _context.MedicationAdministrations
                .Include(m => m.Medication)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (medicationAdministration == null)
            {
                return NotFound();
            }

            return View(medicationAdministration);
        }

        // GET: MedicationAdministrations/Create
        public IActionResult Create()
        {
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            return View();
        }

        // POST: MedicationAdministrations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Admin_ID,PatientId,MedicationId,EmployeeId,Dosage,Time,Notes")] MedicationAdministration medicationAdministration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicationAdministration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", medicationAdministration.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", medicationAdministration.PatientId);
            return View(medicationAdministration);
        }

        // GET: MedicationAdministrations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationAdministration = await _context.MedicationAdministrations.FindAsync(id);
            if (medicationAdministration == null)
            {
                return NotFound();
            }
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", medicationAdministration.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", medicationAdministration.PatientId);
            return View(medicationAdministration);
        }

        // POST: MedicationAdministrations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Admin_ID,PatientId,MedicationId,EmployeeId,Dosage,Time,Notes")] MedicationAdministration medicationAdministration)
        {
            if (id != medicationAdministration.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicationAdministration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationAdministrationExists(medicationAdministration.PatientId))
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
            ViewData["MedicationId"] = new SelectList(_context.Medications, "MedicationId", "Name", medicationAdministration.MedicationId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", medicationAdministration.PatientId);
            return View(medicationAdministration);
        }

        // GET: MedicationAdministrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationAdministration = await _context.MedicationAdministrations
                .Include(m => m.Medication)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (medicationAdministration == null)
            {
                return NotFound();
            }

            return View(medicationAdministration);
        }

        // POST: MedicationAdministrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicationAdministration = await _context.MedicationAdministrations.FindAsync(id);
            if (medicationAdministration != null)
            {
                _context.MedicationAdministrations.Remove(medicationAdministration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationAdministrationExists(int id)
        {
            return _context.MedicationAdministrations.Any(e => e.PatientId == id);
        }
    }
}
