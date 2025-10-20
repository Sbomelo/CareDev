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
using static System.Runtime.InteropServices.JavaScript.JSType;


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

        //GET : Admissions
        public async Task<IActionResult> Index(DateTime? dateFrom,DateTime? dateTo,string? q,int? wardId, int? doctorId, int? employeeId)
        {
            var query = _context.Admissions
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Bed)
                .Include(a => a.Ward)
                .Include(a => a.Doctor)
                .Include(a => a.Employee)
                .AsQueryable();

            // Filter: date range (AdmissionDate)
            if (dateFrom.HasValue)
            {
                var start = dateFrom.Value.Date;
                query = query.Where(a => a.AdmissionDate >= start);
            }

            if (dateTo.HasValue)
            {
                var end = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.AdmissionDate <= end);
            }

            // Text search: patient full name OR bed number OR ward name
            if (!string.IsNullOrWhiteSpace(q))
            {
                var qTrim = q.Trim().ToLower();
                query = query.Where(a =>
                    (a.Patient != null && ((a.Patient.SurName + " " + a.Patient.Name).ToLower().Contains(qTrim)
                                           || (a.Patient.Name + " " + a.Patient.SurName).ToLower().Contains(qTrim)))
                    || (a.Bed != null && a.Bed.BedNumber.ToLower().Contains(qTrim))
                    || (a.Ward != null && a.Ward.Name.ToLower().Contains(qTrim))
                );
            }

            // Filter by Ward
            if (wardId.HasValue)
            {
                query = query.Where(a => a.WardId == wardId.Value);
            }

            // Filter by Doctor
            if (doctorId.HasValue)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }

            // Filter by Employee
            if (employeeId.HasValue)
            {
                query = query.Where(a => a.EmployeeId == employeeId.Value);
            }

            // Order by most recent admissions first
            var list = await query.OrderByDescending(a => a.AdmissionDate).ToListAsync();

            //selects for the filter form
            var wards = await _context.Wards.OrderBy(w => w.Name)
                .Select(w => new { w.WardId, w.Name }).ToListAsync();
            var doctors = await _context.Doctors.OrderBy(d => d.SurName).ThenBy(d => d.Name)
                .Select(d => new { d.DoctorId, FullName = d.SurName + " " + d.Name }).ToListAsync();
            var employees = await _context.Employees.OrderBy(e => e.SurName).ThenBy(e => e.Name)
                .Select(e => new { e.EmployeeId, FullName = e.SurName + " " + e.Name }).ToListAsync();

            ViewBag.Wards = new SelectList(wards, "WardId", "Name", wardId);
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName", doctorId);
            ViewBag.Employees = new SelectList(employees, "EmployeeId", "FullName", employeeId);

            //Preserve filter values in ViewData
           ViewData["dateFrom"] = dateFrom?.ToString("yyyy-MM-dd") ?? "";
            ViewData["dateTo"] = dateTo?.ToString("yyyy-MM-dd") ?? "";
            ViewData["q"] = q ?? "";

            return View(list);
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
            var patients = await _context.Patients
                .OrderBy(p => p.SurName).ThenBy(p => p.Name)
                .Where(predicate: p => !p.IsAdmitted)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.SurName + " " + p.Name
                })
                .ToListAsync();
             ViewBag.Patients = new SelectList(patients, "PatientId", "FullName");

            var doctors = await _context.Doctors
                .OrderBy(d => d.SurName).ThenBy(d => d.Name)
                .Select(d => new {
                    d.DoctorId,
                    FullName = d.SurName + " " + d.Name
                })
                .ToListAsync();
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName");

            var employees = await _context.Employees
                .OrderBy(e => e.SurName).ThenBy(e => e.Name)
                .Select(e => new {
                    e.EmployeeId,
                    FullName = e.SurName + " " + e.Name
                })
                .ToListAsync();
            ViewBag.Employees = new SelectList(employees, "EmployeeId", "FullName");

            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");

            //Filter that show only those in selected ward
            var beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();
            ViewBag.Beds = beds;

            return View();
        }

        // POST: Admissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WardAdmin,Admin")]
        public async Task<IActionResult> Create([Bind("PatientId,WardId,BedId,DoctorId,EmployeeId,AdmissionDate,AdmissionReason,Notes")] Admission model)//Models.ViewModels.AdmitPatientViewModel vm)
        {
            // ensure we repopulate selects on validation errors
            ViewBag.Patients = new SelectList(await _context.Patients
                .OrderBy(p => p.Name).ThenBy(p => p.SurName)
                .Where(predicate:p => !p.IsAdmitted)
                .ToListAsync(), "PatientId", "Name", model.PatientId);
            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name", model.WardId);
            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();

            if (!ModelState.IsValid)
            {
                await PopulateAdmissionSelects(model);
                return View(model);
            }

            // server-side validation: if a bed was selected, ensure it exists and is available
            if (model.BedId.HasValue)
            {
                var bed = await _context.Beds.FindAsync(model.BedId.Value);
                if (bed == null)
                {
                    ModelState.AddModelError("BedId", "Selected bed not found.");
                    return View(model);
                }

                if (!bed.IsAvailable)
                {
                    ModelState.AddModelError("BedId", "Selected bed is not available. Please choose a different bed.");
                    return View(model);
                }
            }

            // create admission and mark bed occupied inside a transaction
            await using var txn = await _context.Database.BeginTransactionAsync();
            try
            {
                model.AdmissionDate = model.AdmissionDate == default ? DateTime.UtcNow : model.AdmissionDate;
                model.IsActive = true;

                // mark bed as occupied if provided
                if (model.BedId.HasValue)
                {
                    var bed = await _context.Beds.FindAsync(model.BedId.Value);
                    bed.IsAvailable = false;
                }
                //Create the admission record.
                  var admission = new Admission
                  {
                       PatientId = model.PatientId,
                      WardId = model.WardId,
                      BedId = model.BedId,
                      DoctorId = model.DoctorId,
                      AdmissionDate = DateTime.UtcNow,
                      IsActive = true
                  };      
                _context.Admissions.Add(model);

                // Set patient flag IsAdmitted = true (if patient exists)
                var patient = await _context.Patients.FindAsync(model.PatientId);
                if (patient != null)
                {
                    patient.IsAdmitted = true;
                    _context.Patients.Update(patient);
                }
                await _context.SaveChangesAsync();
                await txn.CommitAsync();

                TempData["Success"] = "Patient admitted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                await txn.RollbackAsync();
                _logger.LogError(ex, "Error creating admission");
                ModelState.AddModelError("", "An error occurred while admitting the patient. Please try again.");
                ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                await txn.RollbackAsync();
                _logger.LogError(ex, "Error creating admission");
                ModelState.AddModelError("", "An unexpected error occurred while admitting the patient.");
                ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();
                return View(model);
            }
        }

        private async Task PopulateAdmissionSelects(object selectedPatient = null, object selectedDoctor = null, object selectedEmployee = null)
        {
            var patients = await _context.Patients
                .OrderBy(p => p.SurName).ThenBy(p => p.Name)
                .Select(p => new { p.PatientId, FullName = p.SurName + " " + p.Name })
                .ToListAsync();
            ViewBag.Patients = new SelectList(patients, "PatientId", "FullName", selectedPatient);

            var doctors = await _context.Doctors
                .OrderBy(d => d.SurName).ThenBy(d => d.Name)
                .Select(d => new { d.DoctorId, FullName = d.SurName + " " + d.Name })
                .ToListAsync();
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName", selectedDoctor);

            var employees = await _context.Employees
                .OrderBy(e => e.SurName).ThenBy(e => e.Name)
                .Select(e => new { e.EmployeeId, FullName = e.SurName + " " + e.Name })
                .ToListAsync();
            ViewBag.Employees = new SelectList(employees, "EmployeeId", "FullName", selectedEmployee);

            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name");
            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();
        }

        //Get Admnissions
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var admission = await _context.Admissions
                .Include(a => a.Bed)
                .FirstOrDefaultAsync(a => a.AdmissionId == id);

            if (admission == null) return NotFound();

            ViewBag.Patients = new SelectList(await _context.Patients.OrderBy(p => p.Name).ThenBy(p => p.SurName).ToListAsync(), "PatientId", "Name", admission.PatientId);
            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name", admission.WardId);
            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();
            var doctors = await _context.Doctors
               .OrderBy(d => d.SurName).ThenBy(d => d.Name)
               .Select(d => new {
                   d.DoctorId,
                   FullName = d.SurName + " " + d.Name
               })
               .ToListAsync();
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName");

            var employees = await _context.Employees
                .OrderBy(e => e.SurName).ThenBy(e => e.Name)
                .Select(e => new {
                    e.EmployeeId,
                    FullName = e.SurName + " " + e.Name
                })
                .ToListAsync();
            ViewBag.Employees = new SelectList(employees, "EmployeeId", "FullName");

            return View(admission);
        }

        // POST: Admissions/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdmissionId,PatientId,WardId,BedId,DoctorId,EmployeeId,AdmissionDate,AdmissionReason,Notes,IsActive,DischargeDate,DischargeNotes")] Admission model)
        {
            if (id != model.AdmissionId) return BadRequest();

            ViewBag.Patients = new SelectList(await _context.Patients.OrderBy(p => p.Name).ThenBy(p => p.SurName).ToListAsync(), "PatientId", "Name", model.PatientId);
            ViewBag.Wards = new SelectList(await _context.Wards.OrderBy(w => w.Name).ToListAsync(), "WardId", "Name", model.WardId);
            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();

            if (!ModelState.IsValid) return View(model);

            // Begin transaction to safely update bed availability if bed changed
            await using var txn = await _context.Database.BeginTransactionAsync();
            try
            {
                var admission = await _context.Admissions
                    .Include(a => a.Bed)
                    .FirstOrDefaultAsync(a => a.AdmissionId == id);

                if (admission == null) return NotFound();

                // if bed changed, update old and new bed availability
                if (admission.BedId != model.BedId)
                {
                    // free old bed
                    if (admission.BedId.HasValue)
                    {
                        var oldBed = await _context.Beds.FindAsync(admission.BedId.Value);
                        if (oldBed != null)
                            oldBed.IsAvailable = true;
                    }

                    // occupy new bed if provided
                    if (model.BedId.HasValue)
                    {
                        var newBed = await _context.Beds.FindAsync(model.BedId.Value);
                        if (newBed == null)
                        {
                            ModelState.AddModelError("BedId", "Selected bed not found.");
                            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();
                            return View(model);
                        }
                        if (!newBed.IsAvailable)
                        {
                            ModelState.AddModelError("BedId", "Selected bed is not available.");
                            ViewBag.Beds = await _context.Beds.Include(b => b.Ward).OrderBy(b => b.BedNumber).ToListAsync();
                            return View(model);
                        }
                        newBed.IsAvailable = false;
                    }
                }

                // update admission properties
                admission.PatientId = model.PatientId;
                admission.WardId = model.WardId;
                admission.BedId = model.BedId;
                admission.DoctorId = model.DoctorId;
                admission.EmployeeId = model.EmployeeId;
                admission.AdmissionDate = model.AdmissionDate;
                admission.AdmissionReason = model.AdmissionReason;
                admission.Notes = model.Notes;
                admission.IsActive = model.IsActive;
                admission.DischargeDate = model.DischargeDate;
                admission.DischargeNotes = model.DischargeNotes;

                await _context.SaveChangesAsync();
                await txn.CommitAsync();

                TempData["Success"] = "Admission updated.";
                return RedirectToAction(nameof(Details), new { id = admission.AdmissionId });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await txn.RollbackAsync();
                _logger.LogWarning(ex, "Concurrency conflict editing admission {AdmissionId}", id);
                ModelState.AddModelError("", "Concurrency error — another user changed the data. Try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                await txn.RollbackAsync();
                _logger.LogError(ex, "Error editing admission {AdmissionId}", id);
                ModelState.AddModelError("", "An error occurred while saving changes.");
                return View(model);
            }
        }

        //View a list of active or admitted patients
        public async Task<IActionResult> ActiveAdmissions(DateTime? from,DateTime?to, string? q, int? wardId, int? doctorId, int? employeeId)
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

            //if (!string.IsNullOrWhiteSpace(q))
            //{
            //    q = q.Trim();
            //    query = query.Where(a =>
            //        (a.Patient != null && (a.Patient.Name + " " + a.Patient.SurName).Contains(q)) ||
            //        (a.Bed != null && a.Bed.BedNumber.Contains(q)) ||
            //        (a.Ward != null && a.Ward.Name.Contains(q)) // if Ward has Name property
            //    );
            //}

            //var list = await query
            //   .OrderByDescending(a => a.AdmissionDate)
            //   .ToListAsync();

            //ViewData["From"] = from?.ToString("yyyy-MM-dd") ?? "";
            //ViewData["Q"] = q ?? "";

            //return View(list);

            // Text search: patient full name OR bed number OR ward name
            if (!string.IsNullOrWhiteSpace(q))
            {
                var qTrim = q.Trim().ToLower();
                query = query.Where(a =>
                    (a.Patient != null && ((a.Patient.SurName + " " + a.Patient.Name).ToLower().Contains(qTrim)
                                           || (a.Patient.Name + " " + a.Patient.SurName).ToLower().Contains(qTrim)))
                    || (a.Bed != null && a.Bed.BedNumber.ToLower().Contains(qTrim))
                    || (a.Ward != null && a.Ward.Name.ToLower().Contains(qTrim))
                );
            }

            // Filter by Ward
            if (wardId.HasValue)
            {
                query = query.Where(a => a.WardId == wardId.Value);
            }

            // Filter by Doctor
            if (doctorId.HasValue)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }

            // Filter by Employee
            if (employeeId.HasValue)
            {
                query = query.Where(a => a.EmployeeId == employeeId.Value);
            }

            // Order by most recent admissions first
            var list = await query.OrderByDescending(a => a.AdmissionDate).ToListAsync();

            //selects for the filter form
            var wards = await _context.Wards.OrderBy(w => w.Name)
                .Select(w => new { w.WardId, w.Name }).ToListAsync();
            var doctors = await _context.Doctors.OrderBy(d => d.SurName).ThenBy(d => d.Name)
                .Select(d => new { d.DoctorId, FullName = d.SurName + " " + d.Name }).ToListAsync();
            var employees = await _context.Employees.OrderBy(e => e.SurName).ThenBy(e => e.Name)
                .Select(e => new { e.EmployeeId, FullName = e.SurName + " " + e.Name }).ToListAsync();

            ViewBag.Wards = new SelectList(wards, "WardId", "Name", wardId);
            ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName", doctorId);
            ViewBag.Employees = new SelectList(employees, "EmployeeId", "FullName", employeeId);

            //Preserve filter values in ViewData
            ViewData["dateFrom"] = from?.ToString("yyyy-MM-dd") ?? "";
            ViewData["dateTo"] = to?.ToString("yyyy-MM-dd") ?? "";
            ViewData["q"] = q ?? "";

            return View(list);
        }


        // GET: Discharges/Details
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
        // GET: Admissions/Discharge
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

        // POST: Admissions/Discharge
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

        private bool AdmissionExists(int id)
        {
            return _context.Admissions.Any(e => e.AdmissionId == id);
        }


    }
}
