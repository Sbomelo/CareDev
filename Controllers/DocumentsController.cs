using CareDev.Data;
using CareDev.Heplers;
using CareDev.Models;
using CareDev.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Text.Json;



public class DocumentsController : Controller
{
    private readonly ApplicationDbContext _db;
    public DocumentsController(ApplicationDbContext db)
    {
        _db = db;
    }
    // GET: /Documents
    [Authorize(Roles="Patient")]
    public async Task<IActionResult> Index()
    {
        var model = new DownloadOptionsViewModel();

        // populate Wards and Doctors for filters
        var wards = await _db.Wards
            .OrderBy(w => w.Name)
            .Select(w => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = w.Name,
                Value = w.WardId.ToString()
            }).ToListAsync();

        var doctors = await _db.Doctors // or Doctors table
            .OrderBy(d => d.Name)
            .Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = d.Name,
                Value = d.DoctorId.ToString()
            }).ToListAsync();

        // add a blank/default option
        var wardList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
        {
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("-- Any ward --", "")
        };
        wardList.AddRange(wards);

        var doctorList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
        {
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("-- Any doctor --", "")
        };
        doctorList.AddRange(doctors);

        model.Wards = wardList;
        model.Doctors = doctorList;

        return View(model);
    }

    // GET: /Documents/DownloadMyData?format=csv
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> DownloadMyData(string format = "json", bool includeAudits = false, DateTime? from = null, DateTime? to = null, int? wardId = null, int? doctorId = null)
    {
        // get current user id (assumes claim type NameIdentifier)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Forbid();

        // load patient (example assumes Patient has ApplicationUserId or similar)
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
        if (patient == null) return NotFound("Your patient record was not found.");

        // load admissions & discharges
        var admissions = await _db.Admissions
            .Where(a => a.PatientId == patient.PatientId)
            .OrderByDescending(a => a.AdmissionDate)
            .Select(a => new AdmissionDto
            {
                AdmissionId = a.AdmissionId,
                AdmittedAt = a.AdmissionDate,
                DischargedAt = a.DischargeDate,
                Ward = a.Ward.Name,
                Bed = a.Bed.BedNumber,
                DoctorName = a.Doctor.Name,
                Reason = a.AdmissionReason,
                PatientName = a.Patient.Name
            }).ToListAsync();

        var discharges = await _db.Discharges
            .Where(d => d.Admission.PatientId == patient.PatientId)
            .OrderByDescending(d => d.DischargeDate)
            .Select(d => new DischargeDto
            {
                DischargeId = d.DischargeId,
                AdmissionId = d.AdmissionId,
                DischargedAt = d.DischargeDate,
                Summary = d.Notes,
            }).ToListAsync();

        IEnumerable<AuditEntry> audits = Enumerable.Empty<AuditEntry>();
        if (includeAudits)
        {
            audits = await _db.AuditEntries.Where(x => x.UserId == userId).OrderByDescending(a => a.CreatedAt).Take(500).ToListAsync();
        }

        var package = new PersonalExportPackage
        {
            Patient = patient,
            Admissions = admissions,
            Discharges = discharges,
            Audits = audits
        };

        // record a download audit entry
        var downloadAudit = new AuditEntry
        {
            TableName = "Download",
            Action = "Export",
            UserId = userId,
            UserName = User.Identity.Name,
            CreatedAt = DateTime.UtcNow,
            KeyValues = JsonSerializer.Serialize(new { PatientId = patient.PatientId, Format = format }),
            NewValues = null,
            OldValues = null
        };
        _db.AuditEntries.Add(downloadAudit);
        await _db.SaveChangesAsync();

        // prepare file by requested format
        format = (format ?? "json").ToLowerInvariant();
        return format switch
        {
            "csv" => PrepareZipOrSingleCsv(package),
            "pdf" => PreparePdfFile(package, $"personal_export_{patient.PatientId}_{DateTime.UtcNow:yyyyMMdd}.pdf"),
            "zip" => await PrepareZipForPackage(package),
            _ => File(ExportUtils.ToJsonBytes(package), "application/json", $"personal_export_{patient.PatientId}_{DateTime.UtcNow:yyyyMMdd}.json")
        };
    }

    // Downloads a PDF file result
    private IActionResult PreparePdfFile(PersonalExportPackage package, string fileName)
    {
        var bytes = PdfExport.CreatePersonalPdf(package);
        return File(bytes, "application/pdf", fileName);
    }

    // Returns a single CSV when the package is small — we join Admissions + Discharges in one CSV stream.
    private IActionResult PrepareZipOrSingleCsv(PersonalExportPackage package)
    {
        // Create two CSV files (admissions, discharges) inside a zip and return zip
        // For simplicity always zipped
        return PrepareZipForPackage(package).Result;
    }

    private async Task<IActionResult> PrepareZipForPackage(PersonalExportPackage package)
    {
        // Use a temp file to avoid holding huge memory for large datasets
        var tempFile = Path.GetTempFileName();
        try
        {
            using (var fs = System.IO.File.Open(tempFile, FileMode.Truncate))
            using (var archive = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: true))
            {
                // patient.json
                var pEntry = archive.CreateEntry("patient.json");
                using (var entryStream = pEntry.Open())
                {
                    var bytes = ExportUtils.ToJsonBytes(package.Patient);
                    await entryStream.WriteAsync(bytes, 0, bytes.Length);
                }

                // admissions.csv
                var aEntry = archive.CreateEntry("admissions.csv");
                using (var entryStream = aEntry.Open())
                using (var sw = new StreamWriter(entryStream, Encoding.UTF8))
                {
                    sw.Write(ExportUtils.ToCsv(package.Admissions));
                    await sw.FlushAsync();
                }

                // discharges.csv
                var dEntry = archive.CreateEntry("discharges.csv");
                using (var entryStream = dEntry.Open())
                using (var sw = new StreamWriter(entryStream, Encoding.UTF8))
                {
                    sw.Write(ExportUtils.ToCsv(package.Discharges));
                    await sw.FlushAsync();
                }

                // audits.json (optional)
                if (package.Audits != null && package.Audits.Any())
                {
                    var auditEntry = archive.CreateEntry("audits.json");
                    using (var e = auditEntry.Open())
                    {
                        var bytes = ExportUtils.ToJsonBytes(package.Audits);
                        await e.WriteAsync(bytes, 0, bytes.Length);
                    }
                }
            }

            // return the temp file as a FileStreamResult and delete the temp when finished
            var stream = System.IO.File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = File(stream, "application/zip", $"personal_export_{package.Patient.PatientId}_{DateTime.UtcNow:yyyyMMdd}.zip");
            // The file will remain until server deletes it; consider background cleanup of temp files older than X hours.
            return result;
        }
        catch
        {
            // cleanup if something failed
            if (System.IO.File.Exists(tempFile)) System.IO.File.Delete(tempFile);
            throw;
        }
    }

    //// Example filtered admissions export (admin or owner)
    [Authorize(Roles = "Admin,SystemAdmin")]
    public async Task<IActionResult> DownloadFilteredAdmissions(DateTime? from, DateTime? to, int? wardId, int? doctorId, string format = "csv")
    {
        var q = _db.Admissions.Include(a => a.Ward)
            .Include(a => a.Bed)
            .Include(a => a.Doctor)
            .Include(a => a.Ward)
            .Include(a => a.Patient)
            .AsQueryable();

        if (from.HasValue) q = q.Where(a => a.AdmissionDate >= from.Value);
        if (to.HasValue) q = q.Where(a => a.AdmissionDate <= to.Value);
        if (wardId.HasValue) q = q.Where(a => a.WardId == wardId.Value);
        if (doctorId.HasValue) q = q.Where(a => a.DoctorId == doctorId.Value);

        var list = await q.Select(a => new AdmissionDto
        {
            AdmissionId = a.AdmissionId,
            AdmittedAt = a.AdmissionDate,
            DischargedAt = a.DischargeDate,
            Ward = a.Ward.Name,
            Bed = a.Bed.BedNumber,
            DoctorName = a.Doctor.Name,
            Reason = a.Notes,
            PatientName = a.Patient.Name,
            PatientId = a.Patient.PatientId

        }).ToListAsync();

        format = (format ?? "csv").ToLowerInvariant();
        if (format == "json")
        {
            return File(ExportUtils.ToJsonBytes(list), "application/json", $"admissions_{DateTime.UtcNow:yyyyMMdd}.json");
        }
        else if (format == "pdf")
        {
            // create a PDF with a simple admissions list
            var package = new PersonalExportPackage { Admissions = list, Patient = null, Discharges = Enumerable.Empty<DischargeDto>() };
            var bytes = PdfExport.CreatePersonalPdf(package); // re-uses the personal PDF layout, it's fine for lists too
            return File(bytes, "application/pdf", $"admissions_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
        else // csv default
        {
            var csv = ExportUtils.ToCsv(list);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"admissions_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }

    //[Authorize(Roles = "Admin,SystemAdmin")]
    //public async Task<IActionResult> DownloadFilteredAdmissions(
    //DateTime? from = null,
    //DateTime? to = null,
    //int? wardId = null,
    //int? doctorId = null,
    //string format = "csv")    // csv | json | pdf
    //{
    //    // Build query
    //    var q = _db.Admissions
    //        .Include(a => a.Ward)
    //        .Include(a => a.Bed)
    //        .Include(a => a.Doctor)
    //        .Include(a => a.Patient)
    //        .AsQueryable();

    //    if (from.HasValue) q = q.Where(a => a.AdmissionDate >= from.Value);
    //    if (to.HasValue) q = q.Where(a => a.AdmissionDate <= to.Value);
    //    if (wardId.HasValue) q = q.Where(a => a.WardId == wardId.Value);
    //    if (doctorId.HasValue) q = q.Where(a => a.DoctorId == doctorId.Value);

    //    // Projection to DTO to avoid pulling tracked EF entities
    //    var list = await q.Select(a => new AdmissionDto
    //    {
    //        AdmissionId = a.AdmissionId,
    //        AdmittedAt = a.AdmissionDate,
    //        DischargedAt = a.DischargeDate,
    //        Ward = a.Ward != null ? a.Ward.Name : "",
    //        Bed = a.Bed != null ? a.Bed.BedNumber : "",
    //        DoctorName = a.Doctor != null ? a.Doctor.Name : "",
    //        Reason = a.Notes,
    //        Patients = a.Patient.PatientId,
    //        PatientName = a.Patient.Name
    //    }).ToListAsync();

    //    format = (format ?? "csv").ToLowerInvariant();

    //    if (format == "json")
    //    {
    //        return File(ExportUtils.ToJsonBytes(list), "application/json", $"admissions_{DateTime.UtcNow:yyyyMMdd}.json");
    //    }
    //    else if (format == "pdf")
    //    {
    //        // Create a PDF from list (you can reuse PdfExport but make sure it can accept list-only packages)
    //        var pdfBytes = PdfExport.CreateAdmissionsPdf(list); // implement a simple PDF generator for lists
    //        return File(pdfBytes, "application/pdf", $"admissions_{DateTime.UtcNow:yyyyMMdd}.pdf");
    //    }
    //    else
    //    {
    //        // CSV by default
    //        var csv = ExportUtils.ToCsv(list);
    //        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"admissions_{DateTime.UtcNow:yyyyMMdd}.csv");
    //    }
    //}


    // Single admission download (user must own or be admin)
    [Authorize]
    public async Task<IActionResult> DownloadAdmission(int admissionId, string format = "json")
    {
        var admission = await _db.Admissions
            .Include(a => a.Ward).Include(a => a.Bed).Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.AdmissionId == admissionId);
        if (admission == null) return NotFound();

        // authorization: allow if user is the patient owner or Admin
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SystemAdmin");
        if (!isAdmin && admission.Patient.ApplicationUserId != userId) return Forbid();

        var dto = new AdmissionDto
        {
            AdmissionId = admission.AdmissionId,
            AdmittedAt = admission.AdmissionDate,
            DischargedAt = admission.DischargeDate,
            Ward = admission.Ward.Name,
            Bed = admission.Bed.BedNumber,
            DoctorName = admission.Doctor.Name,
            Reason = admission.Notes
        };

        format = (format ?? "json").ToLowerInvariant();
        return format switch
        {
            "csv" => File(Encoding.UTF8.GetBytes(ExportUtils.ToCsv(new[] { dto })), "text/csv", $"admission_{admissionId}.csv"),
            "pdf" => File(PdfExport.CreatePersonalPdf(new PersonalExportPackage { Admissions = new[] { dto } }), "application/pdf", $"admission_{admissionId}.pdf"),
            _ => File(ExportUtils.ToJsonBytes(dto), "application/json", $"admission_{admissionId}.json")
        };
    }

    // GET: /Documents/AdmissionsExport  <-- shows the form + optional preview
    [Authorize(Roles = "Admin,WardAdmin")]
    public async Task<IActionResult> AdmissionsExport(DateTime? from = null, DateTime? to = null, int? wardId = null, int? doctorId = null)
    {
        var model = new DownloadFilteredAdmission
        {
            From = from,
            To = to,
            WardId = wardId,
            DoctorId = doctorId
        };

        // populate select lists
        var wards = await _db.Wards.OrderBy(w => w.Name)
            .Select(w => new SelectListItem { Text = w.Name, Value = w.WardId.ToString() }).ToListAsync();
        wards.Insert(0, new SelectListItem("-- Any ward --", ""));
        model.Wards = wards;

        var doctors = await _db.Doctors.OrderBy(d => d.Name)
            .Select(d => new SelectListItem { Text = d.Name, Value = d.DoctorId.ToString() }).ToListAsync();
        doctors.Insert(0, new SelectListItem("-- Any doctor --", ""));
        model.Doctors = doctors;

        // optional: show preview results in the view (first N rows)
        var q = _db.Admissions.Include(a => a.Ward).Include(a => a.Bed).Include(a => a.Doctor).Include(a => a.Patient).AsQueryable();
        if (from.HasValue) q = q.Where(a => a.AdmissionDate >= from.Value);
        if (to.HasValue) q = q.Where(a => a.AdmissionDate <= to.Value);
        if (wardId.HasValue) q = q.Where(a => a.WardId == wardId.Value);
        if (doctorId.HasValue) q = q.Where(a => a.DoctorId == doctorId.Value);

        model.Results = await q.OrderByDescending(a => a.AdmissionDate)
            .Take(50)
            .Select(a => new AdmissionDto
            {
                AdmissionId = a.AdmissionId,
                PatientId = a.Patient.PatientId,
                PatientName = a.Patient.Name,
                AdmittedAt = a.AdmissionDate,
                DischargedAt = a.DischargeDate,
                Ward = a.Ward != null ? a.Ward.Name : "",
                Bed = a.Bed != null ? a.Bed.BedNumber : "",
                DoctorName = a.Doctor != null ? a.Doctor.Name : ""
            }).ToListAsync();

        return View(model);
    }
}

