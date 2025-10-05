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
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Patient Portal
        [Authorize (Roles="Patient")]
        public async Task<IActionResult> Portal()
        {
            return View(); 
        }
        // GET: Patients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.ToListAsync());
        }

        // GET: Patients/Details/5
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(m => m.Medications)
                .Include(a => a.Allergy)
                .Include(c => c.ChronicCondition)
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
            ViewData["MedicationId"] = new SelectList(_context.Wards, "MedicationId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "WardAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Name,SurName,Age,Gender,PhoneNumber,MedicationId,AllergyId,ChronicConditionId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicationId"] = new SelectList(_context.Wards, "MedicationId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
            return View(patient);
        }

        // GET: Patients/Edit/5
        [Authorize]
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
            ViewData["MedicationId"] = new SelectList(_context.Wards, "MedicationId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,Name,SurName,Age,Gender,PhoneNumber,MedicationId,AllergyId,ChronicConditionId")] Patient patient)
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
            ViewData["MedicationId"] = new SelectList(_context.Wards, "MedicationId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
            ViewData["AllergyId"] = new SelectList(_context.Wards, "AllergyId", "Name");
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
                .Include(m => m.Medications)
                .Include(a => a.Allergy)
                .Include(c => c.ChronicCondition)
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
