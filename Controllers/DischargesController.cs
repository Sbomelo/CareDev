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
    public class DischargesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DischargesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Discharges
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Discharges.Include(d => d.Admission).Include(d => d.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Discharges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discharge = await _context.Discharges
                .Include(d => d.Admission)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.DischargeId == id);
            if (discharge == null)
            {
                return NotFound();
            }

            return View(discharge);
        }

        // GET: Discharges/Create
        public IActionResult Create()
        {
            ViewData["AdmissionId"] = new SelectList(_context.Admissions, "AdmissionId", "AdmissionReason");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            return View();
        }

        // POST: Discharges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DischargeId,DischargeDate,Notes,AdmissionId,PatientId")] Discharge discharge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discharge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdmissionId"] = new SelectList(_context.Admissions, "AdmissionId", "AdmissionReason", discharge.AdmissionId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", discharge.PatientId);
            return View(discharge);
        }

        // GET: Discharges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discharge = await _context.Discharges.FindAsync(id);
            if (discharge == null)
            {
                return NotFound();
            }
            ViewData["AdmissionId"] = new SelectList(_context.Admissions, "AdmissionId", "AdmissionReason", discharge.AdmissionId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", discharge.PatientId);
            return View(discharge);
        }

        // POST: Discharges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DischargeId,DischargeDate,Notes,AdmissionId,PatientId")] Discharge discharge)
        {
            if (id != discharge.DischargeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discharge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DischargeExists(discharge.DischargeId))
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
            ViewData["AdmissionId"] = new SelectList(_context.Admissions, "AdmissionId", "AdmissionReason", discharge.AdmissionId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", discharge.PatientId);
            return View(discharge);
        }

        // GET: Discharges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discharge = await _context.Discharges
                .Include(d => d.Admission)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.DischargeId == id);
            if (discharge == null)
            {
                return NotFound();
            }

            return View(discharge);
        }

        // POST: Discharges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discharge = await _context.Discharges.FindAsync(id);
            if (discharge != null)
            {
                _context.Discharges.Remove(discharge);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DischargeExists(int id)
        {
            return _context.Discharges.Any(e => e.DischargeId == id);
        }
    }
}
