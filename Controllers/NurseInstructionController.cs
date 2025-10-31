using CareDev.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareDev.Data; // replace with your actual namespace
using CareDev.Models; // replace with your actual namespace

namespace CareDev.Controllers
{
    [Authorize(Roles = "Nurse")]
    public class NurseInstructionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NurseInstructionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NurseInstructions
        public async Task<IActionResult> Index()
        {
            // Get all doctor instructions, include patient & doctor (user) info
            var instructions = await _context.DoctorInstructions
                .Include(i => i.Patient)
                .Include(i => i.User)
                .OrderByDescending(i => i.InstructionDate)
                .ToListAsync();

            return View(instructions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDone(long id)
        {
            var instruction = await _context.DoctorInstructions.FindAsync(id);
            if (instruction == null)
                return NotFound();

            instruction.IsCompleted = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Instruction marked as completed!";
            return RedirectToAction(nameof(Index));
        }

    }
}
