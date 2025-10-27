using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize] // restrict to authenticated users; optionally [Authorize(Roles="Patient")]
public class PatientPortalController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PatientPortalController> _logger;

    public PatientPortalController(ApplicationDbContext context, ILogger<PatientPortalController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /PatientPortal
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

        if (patient == null)
        {
            // If you want staff users to view other patients, handle separately
            TempData["Error"] = "No patient record linked to this account.";
            return View(new PatientAdmissionViewModel());
        }

        // active (current) admission
        var currentAdmission = await _context.Admissions
            .AsNoTracking()
            .Include(a => a.Bed).ThenInclude(b => b.Ward)
            .Include(a => a.Ward)
            .Include(a => a.Doctor)
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.PatientId == patient.PatientId && a.IsActive);

        // movements for this admission (or empty)
        var movements = new List<PatientMovement>();
        if (currentAdmission != null)
        {
            movements = await _context.PatientMovements
                .AsNoTracking()
                .Include(pm => pm.FromBed).ThenInclude(b => b.Ward)
                .Include(pm => pm.ToBed).ThenInclude(b => b.Ward)
                .Include(pm => pm.MovedByUser)
                .Where(pm => pm.AdmissionId == currentAdmission.AdmissionId)
                .OrderByDescending(pm => pm.MovedAt)
                .ToListAsync();
        }

        // recent admissions (history / discharge summaries) - last 10
        var recentAdmissions = await _context.Admissions
            .AsNoTracking()
            .Include(a => a.Bed).ThenInclude(b => b.Ward)
            .Include(a => a.Ward)
            .Where(a => a.PatientId == patient.PatientId)
            .OrderByDescending(a => a.AdmissionDate)
            .Take(10)
            .ToListAsync();

        var vm = new PatientAdmissionViewModel
        {
            PatientId = patient.PatientId,
            PatientFullName = $"{patient.SurName} {patient.Name}",
            CurrentAdmission = currentAdmission,
            Movements = movements,
            RecentAdmissions = recentAdmissions
        };

        return View(vm);
    }

    // GET: /PatientPortal/AdmissionDetails/5
    public async Task<IActionResult> AdmissionDetails(int? id)
    {
        if (id == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

        if (patient == null) return NotFound();

        var admission = await _context.Admissions
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Bed).ThenInclude(b => b.Ward)
            .Include(a => a.Ward)
            .Include(a => a.Doctor)
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.AdmissionId == id && a.PatientId == patient.PatientId);

        if (admission == null) return NotFound();

        var movements = await _context.PatientMovements
            .AsNoTracking()
            .Include(pm => pm.FromBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.ToBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.MovedByUser)
            .Where(pm => pm.AdmissionId == admission.AdmissionId)
            .OrderByDescending(pm => pm.MovedAt)
            .ToListAsync();

        var vm = new PatientAdmissionViewModel
        {
            PatientId = patient.PatientId,
            PatientFullName = $"{patient.SurName} {patient.Name}",
            CurrentAdmission = admission,
            Movements = movements
        };

        return View(vm);
    }

    // GET: /PatientPortal/Movements?admissionId=123
    // inside PatientPortalController
    public async Task<IActionResult> Movements(int? admissionId, DateTime? dateFrom, DateTime? dateTo, string? q)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

        if (patient == null) return NotFound();

        // admissions for the patient (for the filter dropdown)
        var admissionsForFilter = await _context.Admissions
            .AsNoTracking()
            .Where(a => a.PatientId == patient.PatientId)
            .OrderByDescending(a => a.AdmissionDate)
            .ToListAsync();

        // base query for movements belonging to this patient
        var qMovements = _context.PatientMovements
            .AsNoTracking()
            .Include(pm => pm.FromBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.ToBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.MovedByUser)
            .Include(pm => pm.Admission)
            .Where(pm => pm.PatientId == patient.PatientId)
            .AsQueryable();

        // Filter by admission if provided
        if (admissionId.HasValue)
        {
            qMovements = qMovements.Where(pm => pm.AdmissionId == admissionId.Value);
        }

        // Filter by date range (MovedAt)
        if (dateFrom.HasValue)
        {
            var start = dateFrom.Value.Date;
            qMovements = qMovements.Where(pm => pm.MovedAt >= start);
        }
        if (dateTo.HasValue)
        {
            var end = dateTo.Value.Date.AddDays(1).AddTicks(-1);
            qMovements = qMovements.Where(pm => pm.MovedAt <= end);
        }

        // Text search across bed numbers, ward names, notes, moved-by-user and admission id
        if (!string.IsNullOrWhiteSpace(q))
        {
            var qTrim = q.Trim().ToLower();

            qMovements = qMovements.Where(pm =>
                (pm.FromBed != null && pm.FromBed.BedNumber.ToLower().Contains(qTrim))
                || (pm.ToBed != null && pm.ToBed.BedNumber.ToLower().Contains(qTrim))
                || (pm.FromBed != null && pm.FromBed.Ward != null && pm.FromBed.Ward.Name.ToLower().Contains(qTrim))
                || (pm.ToBed != null && pm.ToBed.Ward != null && pm.ToBed.Ward.Name.ToLower().Contains(qTrim))
                || (pm.Reason != null && pm.Reason.ToLower().Contains(qTrim))
                || (pm.MovedByUser != null && (pm.MovedByUser.UserName != null && pm.MovedByUser.UserName.ToLower().Contains(qTrim)))
                || pm.AdmissionId.ToString().Contains(qTrim)
            );
        }

        // Execute query (optionally you can add pagination here)
        var movements = await qMovements.OrderByDescending(pm => pm.MovedAt).Take(1000).ToListAsync();

        var vm = new PatientMovementHistoryViewModel
        {
            PatientId = patient.PatientId,
            PatientFullName = $"{patient.SurName} {patient.Name}",
            AdmissionId = admissionId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Query = q,
            Admissions = admissionsForFilter,
            Movements = movements
        };

        return View(vm);
    }


    // GET: /PatientPortal/MovementDetails/5
    public async Task<IActionResult> MovementDetails(int? id)
    {
        if (id == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

        if (patient == null) return NotFound();

        var movement = await _context.PatientMovements
            .AsNoTracking()
            .Include(pm => pm.FromBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.ToBed).ThenInclude(b => b.Ward)
            .Include(pm => pm.MovedByUser)
            .Include(pm => pm.Admission)
            .FirstOrDefaultAsync(pm => pm.MovementId == id && pm.PatientId == patient.PatientId);

        if (movement == null) return NotFound();

        return View(movement);
    }
}
