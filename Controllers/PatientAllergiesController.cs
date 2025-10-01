using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;
using Microsoft.AspNetCore.Authorization;

namespace CareDev.Controllers
{
    [Authorize(Roles = "Admin,WardAdmin")]
    public class PatientAllergiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientAllergiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PatientAllergies
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PatientAllergy.Include(p => p.Allergy).Include(p => p.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PatientAllergies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientAllergy = await _context.PatientAllergy
                .Include(p => p.Allergy)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientAllergy == null)
            {
                return NotFound();
            }

            return View(patientAllergy);
        }

        // GET: PatientAllergies/Create
        public IActionResult Create()
        {
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            return View();
        }

        // POST: PatientAllergies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,AllergyId")] PatientAllergy patientAllergy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientAllergy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patientAllergy.AllergyId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientAllergy.PatientId);
            return View(patientAllergy);
        }

        // GET: PatientAllergies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientAllergy = await _context.PatientAllergy.FindAsync(id);
            if (patientAllergy == null)
            {
                return NotFound();
            }
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patientAllergy.AllergyId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientAllergy.PatientId);
            return View(patientAllergy);
        }

        // POST: PatientAllergies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,AllergyId")] PatientAllergy patientAllergy)
        {
            if (id != patientAllergy.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientAllergy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientAllergyExists(patientAllergy.PatientId))
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
            ViewData["AllergyId"] = new SelectList(_context.Allergies, "AllergyId", "Name", patientAllergy.AllergyId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientAllergy.PatientId);
            return View(patientAllergy);
        }

        // GET: PatientAllergies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientAllergy = await _context.PatientAllergy
                .Include(p => p.Allergy)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientAllergy == null)
            {
                return NotFound();
            }

            return View(patientAllergy);
        }

        // POST: PatientAllergies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientAllergy = await _context.PatientAllergy.FindAsync(id);
            if (patientAllergy != null)
            {
                _context.PatientAllergy.Remove(patientAllergy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientAllergyExists(int id)
        {
            return _context.PatientAllergy.Any(e => e.PatientId == id);
        }
    }
}
