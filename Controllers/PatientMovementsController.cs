using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CareDev.Controllers
{
  [Authorize(Roles = "WardAdmin,Admin,Doctor")]
  public class PatientMovementsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PatientMovementsController> _logger;

    public PatientMovementsController(ApplicationDbContext context, ILogger<PatientMovementsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: PatientMovements
    public async Task<IActionResult> Index()
    {
        var movements = await _context.PatientMovements
            .AsNoTracking()
            .Include(pm => pm.Patient)
            .Include(pm => pm.FromBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.ToBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.MovedByUser)
            .OrderByDescending(pm => pm.MovedAt)
            .ToListAsync();

        return View(movements);
    }

    // GET: PatientMovements/Create?admissionId=123
    public async Task<IActionResult> Create(int? admissionId)
    {
        if (admissionId == null) return BadRequest();

        var admission = await _context.Admissions
            .Include(a => a.Patient)
            .Include(a => a.Bed)
            .Include(a => a.Ward)
            .FirstOrDefaultAsync(a => a.AdmissionId == admissionId && a.IsActive);

        if (admission == null) return NotFound();

        // Provide list of wards and beds for selection
        var wards = await _context.Wards.OrderBy(w => w.Name).ToListAsync();
        // Beds: only show available beds in the selected/other wards and currently occupied bed (so it can be reselected)
        var beds = await _context.Beds
            .Include(b => b.Ward)
            .OrderBy(b => b.BedNumber)
            .ToListAsync();

        var vm = new MovementCreateViewModel
        {
            AdmissionId = admission.AdmissionId,
            PatientId = admission.PatientId,
            PatientName = admission.Patient != null ? $"{admission.Patient.Name} {admission.Patient.SurName}" : "",
            FromWardId = admission.WardId,
            FromBedId = admission.BedId,
            FromWardName = admission.Ward?.Name,
            FromBedNumber = admission.Bed?.BedNumber,
            MovedAt = DateTime.Now
        };

        ViewBag.Wards = new SelectList(wards, "WardId", "Name");
        // In the Create view we'll filter beds by ward via JS; pass all beds for convenience
        ViewBag.Beds = beds;

        return View(vm);
    }

    // POST: PatientMovements/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CareDev.Models.ViewModels.MovementCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // repopulate selects
            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");
            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).ToListAsync();
            return View(model);
        }

        await using var txn = await _context.Database.BeginTransactionAsync();
        try
        {
            // Load admission, current bed, patient and target bed
            var admission = await _context.Admissions
                .Include(a => a.Bed)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AdmissionId == model.AdmissionId && a.IsActive);

            if (admission == null)
            {
                ModelState.AddModelError("", "Active admission not found.");
                ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");
                ViewBag.Beds = await _context.Beds.Include(b => b.Ward).ToListAsync();
                return View(model);
            }

            var targetBed = await _context.Beds.Include(b => b.Ward).FirstOrDefaultAsync(b => b.BedId == model.ToBedId);
            if (targetBed == null)
            {
                ModelState.AddModelError("ToBedId", "Selected bed not found.");
                ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");
                ViewBag.Beds = await _context.Beds.Include(b => b.Ward).ToListAsync();
                return View(model);
            }

            // Ensure target bed is available OR is the same as current bed
            if (!targetBed.IsAvailable && admission.BedId != targetBed.BedId)
            {
                ModelState.AddModelError("ToBedId", "Selected bed is not available.");
                ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");
                ViewBag.Beds = await _context.Beds.Include(b => b.Ward).ToListAsync();
                return View(model);
            }

            // Record movement
            var movement = new PatientMovement
            {
                AdmissionId = admission.AdmissionId,
                PatientId = admission.PatientId,
                FromWardId = admission.WardId,
                FromBedId = admission.BedId,
                ToWardId = targetBed.WardId,
                ToBedId = targetBed.BedId,
                MovedAt = model.MovedAt ?? DateTime.UtcNow,
                Reason = model.Reason,
                MovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            _context.PatientMovements.Add(movement);

            // Update beds: free old bed (if different) and occupy new one (unless same)
            if (admission.BedId.HasValue && admission.BedId != targetBed.BedId)
            {
                var oldBed = admission.Bed;
                if (oldBed != null)
                    oldBed.IsAvailable = true;
            }

            // Occupy target bed
            targetBed.IsAvailable = false;

            // Update Admission to reflect new bed/ward
            admission.WardId = targetBed.WardId;
            admission.BedId = targetBed.BedId;

            // Update patient flag (still admitted) - no change needed but keep it consistent
            if (admission.Patient != null)
                admission.Patient.IsAdmitted = true;

            await _context.SaveChangesAsync();
            await txn.CommitAsync();

            TempData["Success"] = "Patient moved successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await txn.RollbackAsync();
            _logger.LogWarning(ex, "Concurrency issue while moving admission {AdmissionId}", model.AdmissionId);
            ModelState.AddModelError("", "Concurrency error — please try again.");
            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");
            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).ToListAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            await txn.RollbackAsync();
            _logger.LogError(ex, "Error moving admission {AdmissionId}", model.AdmissionId);
            ModelState.AddModelError("", "An error occurred while moving the patient.");
            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");
            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).ToListAsync();
            return View(model);
        }
    }

    // GET: PatientMovements/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var movement = await _context.PatientMovements
            .AsNoTracking()
            .Include(pm => pm.Patient)
            .Include(pm => pm.FromBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.ToBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.MovedByUser)
            .FirstOrDefaultAsync(pm => pm.MovementId == id);

        if (movement == null) return NotFound();
        return View(movement);
    }
  }
}

