using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CareDev.Data;
using CareDev.Models;

namespace CareDev.Controllers
{
    //[Authorize(Roles = "Doctor")] // Only doctors can access this controller
    public class DoctorInstructionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorInstructionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DoctorInstructions
        public async Task<IActionResult> Index()
        {
            // Show all doctor’s own instructions (if logged in user is a doctor)
            var userId = User.Identity?.Name; // Or however you identify logged-in users
            var instructions = await _context.DoctorInstructions
                .Include(d => d.Patient)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.InstructionDate)
                .ToListAsync();

            return View(instructions);
        }

        // GET: DoctorInstructions/Create
        public IActionResult Create()
        {
            // Load patients list for selection
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
            return View();
        }

        // POST: DoctorInstructions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Instructions,Medication,FollowUpDate,AdditionalNotes")] DoctorInstruction doctorInstruction)
        {
            if (ModelState.IsValid)
            {
                doctorInstruction.InstructionDate = DateTime.Now;
                doctorInstruction.UserId = User.Identity?.Name; // Link instruction to logged-in doctor
                doctorInstruction.IsCompleted = false; // Default to not completed

                _context.Add(doctorInstruction);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Instruction successfully added for the nurse to carry out.";
                return RedirectToAction(nameof(Index));
            }

            // Reload patient dropdown if something fails
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", doctorInstruction.PatientId);
            return View(doctorInstruction);
        }

        // Optional: Doctor can see instruction details
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var instruction = await _context.DoctorInstructions
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.InstructionId == id);

            if (instruction == null)
                return NotFound();

            return View(instruction);
        }

        private bool DoctorInstructionExists(long id)
        {
            return _context.DoctorInstructions.Any(e => e.InstructionId == id);
        }
    }
}
