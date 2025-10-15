using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace CareDev.Controllers
{
    [Authorize(Roles = "WardAdmin,Doctor")]
    public class AdmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdmissionsController> _logger;

        public AdmissionsController(ApplicationDbContext context, ILogger<AdmissionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admissions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Admissions.Include(a => a.Bed).Include(a => a.Doctor).Include(a => a.Employee).Include(a => a.Patient).Include(a => a.Ward);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admission = await _context.Admissions
                .Include(a => a.Bed)
                .Include(a => a.Doctor)
                .Include(a => a.Employee)
                .Include(a => a.Patient)
                .Include(a => a.Ward)
                .FirstOrDefaultAsync(m => m.AdmissionId == id);
            if (admission == null)
            {
                return NotFound();
            }

            return View(admission);
        }

        //MY CREATE ACTION METHOD CODE FOR ADMITTING PATIENTS
        // GET: Admissions/Create
        [HttpGet]
        [Authorize(Roles = "WardAdmin,Admin")]
        public async Task<IActionResult> Create()
        {
            var vm = new Models.ViewModels.AdmitPatientViewModel();
            await PopulateLookupLists(vm);
            return View(vm);
        }

        // POST: Admissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WardAdmin,Admin")]
        public async Task<IActionResult> Create(Models.ViewModels.AdmitPatientViewModel vm)
        {
            //// Log incoming values for debugging
            //_logger.LogInformation("Attempting to admit patient. VM: PatientId={PatientId}, WardId={WardId}, BedId={BedId}, DoctorId={DoctorId}, EmployeeId={EmployeeId}",
            //    vm.PatientId, vm.WardId, vm.BedId, vm.DoctorId, vm.EmployeeId);

            //if (!ModelState.IsValid)
            //{
            //    _logger.LogWarning("ModelState invalid when attempting to admit patient.");
            //    await PopulateLookupLists(vm);
            //    return View(vm);
            //}

            //// Pre-validate required referenced records to give clearer messages
            //var patient = await _context.Patients.FindAsync(vm.PatientId);
            //if (patient == null)
            //{
            //    ModelState.AddModelError(nameof(vm.PatientId), "Selected patient does not exist.");
            //    await PopulateLookupLists(vm);
            //    return View(vm);
            //}

            //var ward = await _context.Wards.FindAsync(vm.WardId);
            //if (ward == null)
            //{
            //    ModelState.AddModelError(nameof(vm.WardId), "Selected ward does not exist.");
            //    await PopulateLookupLists(vm);
            //    return View(vm);
            //}

            //if (vm.BedId.HasValue)
            //{
            //    var bed = await _context.Beds.FindAsync(vm.BedId.Value);
            //    if (bed == null)
            //    {
            //        ModelState.AddModelError(nameof(vm.BedId), "Selected bed does not exist.");
            //        await PopulateLookupLists(vm);
            //        return View(vm);
            //    }

            //    if (!bed.IsAvailable)
            //    {
            //        ModelState.AddModelError(nameof(vm.BedId), "Selected bed is already occupied.");
            //        await PopulateLookupLists(vm);
            //        return View(vm);
            //    }

            //}

            //using var tx = await _context.Database.BeginTransactionAsync();
            //try
            //{
            //    // Attempt to claim bed atomically if provided
            //    if (vm.BedId.HasValue)
            //    {
            //        var bedId = vm.BedId.Value;
            //        var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            //            $"UPDATE Beds SET IsAvailable = 0 WHERE BedId = {bedId} AND IsAvailable = 1");

            //        _logger.LogInformation("Bed claim attempt for BedId={BedId} returned rowsAffected={Rows}", bedId, rowsAffected);

            //        if (rowsAffected != 1)
            //        {
            //            ModelState.AddModelError(nameof(vm.BedId), "Selected bed is no longer available. Please select another bed.");
            //            await PopulateLookupLists(vm);
            //            return View(vm);
            //        }
            //    }

            //    // Build admission entity
            //    var admission = new Admission
            //    {
            //        PatientId = vm.PatientId,
            //        WardId = vm.WardId,
            //        BedId = vm.BedId,
            //        DoctorId = vm.DoctorId,
            //        AdmissionDate = DateTime.UtcNow,
            //        IsActive = true,
            //        EmployeeId = vm.EmployeeId,
            //        AdmissionReason = vm.AdmissionReason
            //    };

            //    _context.Admissions.Add(admission);

            //    // Mark patient as admitted
            //    patient.IsAdmitted = true;
            //    _context.Patients.Update(patient);

            //    await _context.SaveChangesAsync();
            //    await tx.CommitAsync();

            //    TempData["Success"] = "Patient admitted successfully.";
            //    return RedirectToAction("Index", "Admissions");
            //}
            //catch (DbUpdateException dbEx)
            //{
            //    // DB-level error (FK constraint, etc.)
            //    _logger.LogError(dbEx, "Database update error while admitting patient: PatientId={PatientId}", vm.PatientId);

            //    // Surface inner exception details for debugging (local only)
            //    var sqlMsg = dbEx.InnerException?.Message ?? dbEx.Message;
            //    ModelState.AddModelError("", $"Database error: {sqlMsg}");

            //    await tx.RollbackAsync();
            //    await PopulateLookupLists(vm);

            //    // Optionally set a ViewBag for clearer UI rendering
            //    ViewBag.DetailedError = sqlMsg;
            //    return View(vm);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unexpected error while admitting patient: PatientId={PatientId}", vm.PatientId);

            //    // Surface message for debugging (remove in production)
            //    var message = ex.InnerException?.Message ?? ex.Message;
            //    ModelState.AddModelError("", $"An error occurred while admitting the patient: {message}");

            //    await tx.RollbackAsync();
            //    await PopulateLookupLists(vm);

            //    ViewBag.DetailedError = message;
            //    return View(vm);
            //}
            if (!ModelState.IsValid)
            {
                await PopulateLookupLists(vm);
                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // If bed selected, attempt atomic claim
                if (vm.BedId.HasValue)
                {
                    var bedId = vm.BedId.Value;
                    var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"UPDATE Beds SET IsAvailable = 0 WHERE BedId = {bedId} AND IsAvailable = 1");

                    if (rowsAffected != 1)
                    {
                        ModelState.AddModelError(nameof(vm.BedId), "Selected bed is no longer available. Please choose another bed.");
                        await PopulateLookupLists(vm);
                        return View(vm);
                    }
                }

                // Create the admission record.
                var admission = new Admission
                {
                    PatientId = vm.PatientId,
                    WardId = vm.WardId,
                    BedId = vm.BedId, 
                    DoctorId = vm.DoctorId,
                    AdmissionDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Admissions.Add(admission);

                // Set patient flag IsAdmitted = true (if patient exists)
                var patient = await _context.Patients.FindAsync(vm.PatientId);
                if (patient != null)
                {
                    patient.IsAdmitted = true;
                    _context.Patients.Update(patient);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "Patient admitted successfully.";
                return RedirectToAction("Index", "Admissions");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error admitting patient");
                ModelState.AddModelError("", "An error occurred while admitting the patient. Please try again.");
                await PopulateLookupLists(vm);
                return View(vm);
            }
        }

        private async Task PopulateLookupLists(Models.ViewModels.AdmitPatientViewModel vm)
        {
            // Patients who are not currently admitted
            var admittedPatientIds = await _context.Admissions
                .Where(a => a.IsActive)
                .Select(a => a.PatientId)
                .ToListAsync();

            vm.Patients = await _context.Patients
                .AsNoTracking()
                .Where(p => !admittedPatientIds.Contains(p.PatientId))
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = $"{p.Name} {p.SurName}"
                })
                .ToListAsync();

            // Employees (admitting staff)
            vm.Employees = await _context.Employees
                .AsNoTracking()
                .OrderBy(e => e.Name)
                .Select(e => new SelectListItem { Value = e.EmployeeId.ToString(), Text = e.Name + " " + e.SurName })
                .ToListAsync();

            // Wards
            vm.Wards = await _context.Wards
                .AsNoTracking()
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem { Value = w.WardId.ToString(), Text = w.Name })
                .ToListAsync();

            // Beds: only available beds
            vm.Beds = await _context.Beds
                .AsNoTracking()
                .Where(b => b.IsAvailable) // true = available
                .OrderBy(b => b.BedId)
                .Select(b => new SelectListItem { Value = b.BedId.ToString(), Text = b.BedNumber ?? $"Bed {b.BedId}" })
                .ToListAsync();

            // Doctors table -> Doctors select list
            vm.Doctors= await _context.Doctors
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = d.Name })
                .ToListAsync();
        }



        ////SCAFFOLDED GET: Admissions/Create
        //public IActionResult Create()
        //{
        //    ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedNumber");
        //    ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "DoctorId", "Name");
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name");
        //    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
        //    ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name");
        //    return View();
        //}

        //// POST: Admissions/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("AdmissionId,PatientId,WardId,BedId,DoctorId,EmployeeId,AdmissionDate,DischargeDate,AdmissionReason,Notes")] Admission admission)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(admission);
        //        await _context.SaveChangesAsync();
        //        TempData["success"] = "Admission created successfully.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedNumber", admission.BedId);
        //    ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "DoctorId", "Name", admission.DoctorId);
        //    ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", admission.EmployeeId);
        //    ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", admission.PatientId);
        //    ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", admission.WardId);

        //    TempData["error"] = "Error creating admission. Please check the details and try again.";
        //    return View(admission);
        //}

        // GET: Admissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admission = await _context.Admissions.FindAsync(id);
            if (admission == null)
            {
                return NotFound();
            }
            ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedNumber", admission.BedId);
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "DoctorId", "Name", admission.DoctorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", admission.EmployeeId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", admission.PatientId);
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", admission.WardId);
            return View(admission);
        }

        // POST: Admissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdmissionId,PatientId,WardId,BedId,DoctorId,EmployeeId,AdmissionDate,DischargeDate,AdmissionReason,Notes")] Admission admission)
        {
            if (id != admission.AdmissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admission);
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdmissionExists(admission.AdmissionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Admission updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["BedId"] = new SelectList(_context.Beds, "BedId", "BedNumber", admission.BedId);
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "DoctorId", "Name", admission.DoctorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", admission.EmployeeId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", admission.PatientId);
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", admission.WardId);

            TempData["error"] = "Error updating admission. Please check the details and try again.";
            return View(admission);
        }

        //View a list of active or admitted patients
        public async Task<IActionResult> ActiveAdmissions(DateTime? from, string? q)
        {
            var query = _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.Ward)
                .Include(a => a.Bed)
                .Include(a => a.Employee)
                .Include(a => a.Doctor)
                .Where(a => a.IsActive);

            if (from.HasValue)
            {
                var d = from.Value.Date;
                query = query.Where(a => a.AdmissionDate >= d);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(a =>
                    (a.Patient != null && (a.Patient.Name + " " + a.Patient.SurName).Contains(q)) ||
                    (a.Bed != null && a.Bed.BedNumber.Contains(q)) ||
                    (a.Ward != null && a.Ward.Name.Contains(q)) // if Ward has Name property
                );
            }

            var list = await query
               .OrderByDescending(a => a.AdmissionDate)
               .ToListAsync();

            ViewData["From"] = from?.ToString("yyyy-MM-dd") ?? "";
            ViewData["Q"] = q ?? "";

            return View(list);
        }


        // GET: Discharges/Details/5
        public async Task<IActionResult> AdmissionDetails(int? id)
        {
            if (id == null) return NotFound();

            var admission = await _context.Admissions
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Bed)
                .Include(a => a.Ward)
                .Include(a => a.Doctor)
                .Include(a => a.Employee)
                //.Include(a => a.DischargedByUserId)
                .FirstOrDefaultAsync(a => a.AdmissionId == id);

            if (admission == null) return NotFound();

            if (admission.IsActive != true)
            {
                return RedirectToAction("Index");
            }

            return View(admission);
        }
        // GET: Admissions/Discharge/5
        [Authorize(Roles = "WardAdmin,Nurse,Doctor,Admin")]
        public async Task<IActionResult> Discharge(int? id)
        {
            if (id == null) return NotFound();

            var admission = await _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.Bed)
                .FirstOrDefaultAsync(a => a.AdmissionId == id);

            if (admission == null) return NotFound();

            if (!admission.IsActive)
            {
                TempData["Warning"] = "This admission is already discharged.";
                return RedirectToAction(nameof(Details), new { id = admission.AdmissionId });
            }

            var vm = new DischargeViewModel
            {
                AdmissionId = admission.AdmissionId,
                PatientId = admission.PatientId,
                PatientName = admission.Patient != null ? $"{admission.Patient.Name} {admission.Patient.SurName}" : string.Empty,
                BedId = admission.BedId,
                BedNumber = admission.Bed?.BedNumber,
                AdmissionDate = admission.AdmissionDate,
                DischargeDate = DateTime.Now // suggest current time
            };

            return View(vm);
        }

        // POST: Admissions/Discharge/5
        [Authorize(Roles = "WardAdmin,Nurse,Doctor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Discharge(int id, DischargeViewModel model)
        {
            if (id != model.AdmissionId) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            // Transactional update
            await using var txn = await _context.Database.BeginTransactionAsync();
            try
            {
                var admission = await _context.Admissions
                    .Include(a => a.Bed)
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.AdmissionId == id);

                if (admission == null)
                {
                    ModelState.AddModelError("", "Admission not found.");
                    return View(model);
                }

                if (!admission.IsActive)
                {
                    ModelState.AddModelError("", "Admission is already discharged.");
                    return View(model);
                }

                // apply discharge
                admission.DischargeDate = model.DischargeDate ?? DateTime.UtcNow;
                admission.DischargeNotes = model.DischargeNotes;
                admission.IsActive = false;


                // free the bed if present
                if (admission.BedId.HasValue && admission.Bed != null)
                {
                    admission.Bed.IsAvailable = true;

                }

                // update patient flag
                if (admission.Patient != null)
                {
                    admission.Patient.IsAdmitted = false;
                }

                //Record who discharged
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                admission.DischargedByUserId = userId;

                await _context.SaveChangesAsync();
                await txn.CommitAsync();

                TempData["Success"] = "Patient discharged successfully.";
                return RedirectToAction("Index", "Admissions", new { id = admission.PatientId });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await txn.RollbackAsync();
                _logger.LogWarning(ex, "Concurrency issue discharging AdmissionId {AdmissionId}", id);
                ModelState.AddModelError("", "Another user changed the data. Please reload and try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                await txn.RollbackAsync();
                _logger.LogError(ex, "Error discharging AdmissionId {AdmissionId}", id);
                ModelState.AddModelError("", "An error occurred while discharging the patient.");
                return View(model);
            }
        }

        //// GET: Admissions/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var admission = await _context.Admissions
        //        .Include(a => a.Bed)
        //        .Include(a => a.Doctor)
        //        .Include(a => a.Employee)
        //        .Include(a => a.Patient)
        //        .Include(a => a.Ward)
        //        .FirstOrDefaultAsync(m => m.AdmissionId == id);
        //    if (admission == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(admission);
        //}

        //// POST: Admissions/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var admission = await _context.Admissions.FindAsync(id);
        //    if (admission != null)
        //    {
        //        _context.Admissions.Remove(admission);
        //    }

        //    await _context.SaveChangesAsync();
        //    TempData["success"] = "Admission deleted successfully.";
        //    return RedirectToAction(nameof(Index));
        //}

        private bool AdmissionExists(int id)
        {
            return _context.Admissions.Any(e => e.AdmissionId == id);
        }


    }
}
