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
    public class PatientMovementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientMovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PatientMovements
        [Authorize(Roles = "WardAdmin")] 
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PatientMovements.Include(p => p.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PatientMovements/Details/5
        [Authorize(Roles = "WardAdmin,Patient")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientMovement = await _context.PatientMovements
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.MovementId == id);
            if (patientMovement == null)
            {
                return NotFound();
            }

            return View(patientMovement);
        }

        // GET: PatientMovements/Create
        [Authorize(Roles = "WardAdmin")]
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            ViewData["RoomId"] = new SelectList(_context.RoomTypes, "WardId", "WardName");
            return View();
        }

        // POST: PatientMovements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "WardAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovementId,MovementDate,Location,PatientId,RoomId")] PatientMovement patientMovement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientMovement.PatientId);
            return View(patientMovement);
        }

        // GET: PatientMovements/Edit/5
        [Authorize(Roles = "WardAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientMovement = await _context.PatientMovements.FindAsync(id);
            if (patientMovement == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientMovement.PatientId);
            return View(patientMovement);
        }

        // POST: PatientMovements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "WardAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovementId,MovementDate,Location,PatientId,WardId")] PatientMovement patientMovement)
        {
            if (id != patientMovement.MovementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientMovement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientMovementExists(patientMovement.MovementId))
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientMovement.PatientId);
            return View(patientMovement);
        }

        // GET: PatientMovements/Delete/5
        [Authorize(Roles = "WardAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientMovement = await _context.PatientMovements
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.MovementId == id);
            if (patientMovement == null)
            {
                return NotFound();
            }

            return View(patientMovement);
        }

        // POST: PatientMovements/Delete/5
        [Authorize(Roles = "WardAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientMovement = await _context.PatientMovements.FindAsync(id);
            if (patientMovement != null)
            {
                _context.PatientMovements.Remove(patientMovement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientMovementExists(int id)
        {
            return _context.PatientMovements.Any(e => e.MovementId == id);
        }
    }
}
