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
    public class BedsController : Controller
    { 
        private readonly ApplicationDbContext _context;

        public BedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Beds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Beds.Include(b => b.Ward);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Beds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bed = await _context.Beds
                .Include(b => b.Ward)
                .FirstOrDefaultAsync(m => m.BedId == id);
            if (bed == null)
            {
                return NotFound();
            }

            return View(bed);
        }

        // GET: Beds/Create
        [Authorize(Roles="Admin")] 
        public IActionResult Create()
        {
            try
            {
              var wards = _context.Wards.ToList();
              if (!wards.Any())
              {
                TempData["ErrorMessage"] = "No wards available. Please create a ward first.";
                return RedirectToAction("Index");
              }

              ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name");
              return View();
            }

            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GET Create: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading wards. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // POST: Beds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles="Admin")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BedId,BedNumber,IsOccupied,WardId")] Bed bed)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name");
            return View(bed);
        //    Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");
        //    Console.WriteLine($"WardId received: {bed.WardId}");

        //    // Additional validation - check if Ward exists
        //    if (bed.WardId > 0)
        //    {
        //        var wardExists = await _context.Wards.AnyAsync(w => w.WardId == bed.WardId);
        //        if (!wardExists)
        //        {
        //            ModelState.AddModelError("WardId", "The selected ward does not exist.");
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Ensure the bed is available when created (unless specifically set as occupied)
        //            bed.IsOccupied = false; // New beds should typically be available

        //            _context.Add(bed);
        //            await _context.SaveChangesAsync();

        //            TempData["SuccessMessage"] = $"Bed {bed.BedNumber} created successfully!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            Console.WriteLine($"Database error: {ex.Message}");
        //            ModelState.AddModelError("", "An error occurred while saving to the database. This bed might already exist.");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error creating bed: {ex.Message}");
        //            ModelState.AddModelError("", "An error occurred while creating the bed. Please try again.");
        //        }
        //    }
        //    else
        //    {
        //        // Log all validation errors
        //        foreach (var error in ModelState.Where(ms => ms.Value.Errors.Count > 0))
        //        {
        //            foreach (var subError in error.Value.Errors)
        //            {
        //                Console.WriteLine($"Validation error in {error.Key}: {subError.ErrorMessage}");
        //            }
        //        }
        //    }

        //    // Repopulate the wards dropdown
        //    var wards = await _context.Wards.ToListAsync();
        //    ViewData["WardId"] = new SelectList(wards, "WardId", "Name", bed.WardId);
        //    return View(bed);
        //}

        //// GET: Beds/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var bed = await _context.Beds.FindAsync(id);
        //    if (bed == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", bed.WardId);
        //    return View(bed);
        }

        // POST: Beds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BedId,BedNumber,IsOccupied,WardId")] Bed bed)
        {
            if (id != bed.BedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BedExists(bed.BedId))
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
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", bed.WardId);
            return View(bed);
        }

        // GET: Beds/Delete/5
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bed = await _context.Beds
                .Include(b => b.Ward)
                .FirstOrDefaultAsync(m => m.BedId == id);
            if (bed == null)
            {
                return NotFound();
            }

            return View(bed);
        }

        // POST: Beds/Delete/5
        [Authorize(Roles="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bed = await _context.Beds.FindAsync(id);
            if (bed != null)
            {
                _context.Beds.Remove(bed);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BedExists(int id)
        {
            return _context.Beds.Any(e => e.BedId == id);
        }
    }
}
