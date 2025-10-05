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
    [Authorize(Roles = "Admin,WardAdmin")]
    public class PatientFoldersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientFoldersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PatientFolders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PatientFolders.Include(p => p.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PatientFolders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientFolder = await _context.PatientFolders
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientFolder == null)
            {
                return NotFound();
            }

            return View(patientFolder);
        }

        // GET: PatientFolders/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            return View();
        }

        // POST: PatientFolders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientFolderId,FolderName,CreatedDate,PatientId")] PatientFolder patientFolder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientFolder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientFolder.PatientId);
            return View(patientFolder);
        }

        // GET: PatientFolders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientFolder = await _context.PatientFolders.FindAsync(id);
            if (patientFolder == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientFolder.PatientId);
            return View(patientFolder);
        }

        // POST: PatientFolders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientFolderId,FolderName,CreatedDate,PatientId")] PatientFolder patientFolder)
        {
            if (id != patientFolder.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientFolder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientFolderExists(patientFolder.PatientId))
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", patientFolder.PatientId);
            return View(patientFolder);
        }

        // GET: PatientFolders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientFolder = await _context.PatientFolders
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientFolder == null)
            {
                return NotFound();
            }

            return View(patientFolder);
        }

        // POST: PatientFolders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientFolder = await _context.PatientFolders.FindAsync(id);
            if (patientFolder != null)
            {
                _context.PatientFolders.Remove(patientFolder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientFolderExists(int id)
        {
            return _context.PatientFolders.Any(e => e.PatientId == id);
        }
    }
}
