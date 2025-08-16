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
    public class VitalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VitalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vitals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vitals.ToListAsync());
        }

        // GET: Vitals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vital = await _context.Vitals
                .FirstOrDefaultAsync(m => m.VitalId == id);
            if (vital == null)
            {
                return NotFound();
            }

            return View(vital);
        }

        // GET: Vitals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vitals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VitalId,PatientId,Temperature,HeartRate,BloodPressure,RecordDate")] Vital vital)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vital);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vital);
        }

        // GET: Vitals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vital = await _context.Vitals.FindAsync(id);
            if (vital == null)
            {
                return NotFound();
            }
            return View(vital);
        }

        // POST: Vitals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VitalId,PatientId,Temperature,HeartRate,BloodPressure,RecordDate")] Vital vital)
        {
            if (id != vital.VitalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vital);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VitalExists(vital.VitalId))
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
            return View(vital);
        }

        // GET: Vitals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vital = await _context.Vitals
                .FirstOrDefaultAsync(m => m.VitalId == id);
            if (vital == null)
            {
                return NotFound();
            }

            return View(vital);
        }

        // POST: Vitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vital = await _context.Vitals.FindAsync(id);
            if (vital != null)
            {
                _context.Vitals.Remove(vital);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VitalExists(int id)
        {
            return _context.Vitals.Any(e => e.VitalId == id);
        }
    }
}
