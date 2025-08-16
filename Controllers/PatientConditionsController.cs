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
    public class PatientConditionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientConditionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PatientConditions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PatientCondition.Include(p => p.ChronicCondition).Include(p => p.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PatientConditions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientCondition = await _context.PatientCondition
                .Include(p => p.ChronicCondition)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientCondition == null)
            {
                return NotFound();
            }

            return View(patientCondition);
        }

        // GET: PatientConditions/Create
        public IActionResult Create()
        {
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            return View();
        }

        // POST: PatientConditions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,ChronicConditionId")] PatientCondition patientCondition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientCondition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patientCondition.ChronicConditionId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientCondition.PatientId);
            return View(patientCondition);
        }

        // GET: PatientConditions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientCondition = await _context.PatientCondition.FindAsync(id);
            if (patientCondition == null)
            {
                return NotFound();
            }
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patientCondition.ChronicConditionId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientCondition.PatientId);
            return View(patientCondition);
        }

        // POST: PatientConditions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,ChronicConditionId")] PatientCondition patientCondition)
        {
            if (id != patientCondition.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientCondition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientConditionExists(patientCondition.PatientId))
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
            ViewData["ChronicConditionId"] = new SelectList(_context.ChronicConditions, "ChronicConditionId", "Name", patientCondition.ChronicConditionId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientCondition.PatientId);
            return View(patientCondition);
        }

        // GET: PatientConditions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientCondition = await _context.PatientCondition
                .Include(p => p.ChronicCondition)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientCondition == null)
            {
                return NotFound();
            }

            return View(patientCondition);
        }

        // POST: PatientConditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientCondition = await _context.PatientCondition.FindAsync(id);
            if (patientCondition != null)
            {
                _context.PatientCondition.Remove(patientCondition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientConditionExists(int id)
        {
            return _context.PatientCondition.Any(e => e.PatientId == id);
        }
    }
}
