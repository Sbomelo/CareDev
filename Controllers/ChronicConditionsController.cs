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
    public class ChronicConditionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChronicConditionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChronicConditions
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChronicConditions.ToListAsync());
        }

        // GET: ChronicConditions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chronicCondition = await _context.ChronicConditions
                .FirstOrDefaultAsync(m => m.ChronicConditionId == id);
            if (chronicCondition == null)
            {
                return NotFound();
            }

            return View(chronicCondition);
        }

        // GET: ChronicConditions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChronicConditions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChronicConditionId,Name")] ChronicCondition chronicCondition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chronicCondition);
                await _context.SaveChangesAsync();
                TempData["success"] = "Chronic condition created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Error creating chronic condition. Please try again.";
            return View(chronicCondition);
        }

        // GET: ChronicConditions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chronicCondition = await _context.ChronicConditions.FindAsync(id);
            if (chronicCondition == null)
            {
                return NotFound();
            }
            return View(chronicCondition);
        }

        // POST: ChronicConditions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChronicConditionId,Name")] ChronicCondition chronicCondition)
        {
            if (id != chronicCondition.ChronicConditionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chronicCondition);
                    await _context.SaveChangesAsync();
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChronicConditionExists(chronicCondition.ChronicConditionId))
                    {
                       
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                 TempData["success"] = "Chronic condition updated successfully.";
                return RedirectToAction(nameof(Index));
            } 
            TempData["error"] = "Error updating chronic condition. Please try again.";
            return View(chronicCondition);
        }

        // GET: ChronicConditions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chronicCondition = await _context.ChronicConditions
                .FirstOrDefaultAsync(m => m.ChronicConditionId == id);
            if (chronicCondition == null)
            {
                return NotFound();
            }

            return View(chronicCondition);
        }

        // POST: ChronicConditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chronicCondition = await _context.ChronicConditions.FindAsync(id);
            if (chronicCondition != null)
            {
                _context.ChronicConditions.Remove(chronicCondition);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Chronic condition deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ChronicConditionExists(int id)
        {
            return _context.ChronicConditions.Any(e => e.ChronicConditionId == id);
        }
    }
}
