//using CareDev.Data;
//using CareDev.Models;
//using CareDev.Services.IService;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;

//// Patients create and view their own appointments
//[Authorize(Roles = "Patient")]
//public class PatientAppointmentController : Controller
//{
//    private readonly IAppointmentService _appointmentService;
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly ApplicationDbContext _context;
//    public PatientAppointmentController(IAppointmentService appointmentService,
//                                        UserManager<ApplicationUser> userManager,
//                                        ApplicationDbContext context)
//    {
//        _appointmentService = appointmentService;
//        _userManager = userManager;
//        _context = context;
//    }

//    // Booking form (GET)
//    public async Task<IActionResult> CreateAsync()
//    {
//        // Populate ViewBag with list of doctors, etc.
//        var doctors = await _context.Doctors.OrderBy(d => d.SurName).ThenBy(d => d.Name)
//                .Select(d => new { d.DoctorId, FullName = d.SurName + " " + d.Name }).ToListAsync();

//        ViewBag.Doctors = new SelectList(doctors, "DoctorId", "FullName", doctors);
//        return View();
//    }

//    // Handle booking submission (POST)
//    [HttpPost]
//    public async Task<IActionResult> Create(string doctorId, DateTime start, DateTime end)
//    {
//        var patientId = _userManager.GetUserId(User);
//        await _appointmentService.BookAppointmentAsync(doctorId, patientId, start, end);
//        return RedirectToAction(nameof(MyAppointments));
//    }

//    // List of this patient's appointments
//    public async Task<IActionResult> MyAppointments()
//    {
//        var patientId = _userManager.GetUserId(User);
//        var appts = await _appointmentService.GetPatientAppointmentsAsync(patientId);
//        return View(appts);
//    }

//    // Reschedule (can only reschedule own appointments)
//    public async Task<IActionResult> Reschedule(int id)
//    {
//        var appt = await _context.Appointments.FindAsync(id);
//        if (appt == null || appt.PatientId != _userManager.GetUserId(User))
//            return Unauthorized();
//        return View(appt);
//    }
//    [HttpPost]
//    public async Task<IActionResult> Reschedule(int id, DateTime newStart, DateTime newEnd)
//    {
//        // (Check user owns appt as above)
//        await _appointmentService.RescheduleAsync(id, newStart, newEnd);
//        return RedirectToAction(nameof(MyAppointments));
//    }

//    // Cancel appointment
//    public async Task<IActionResult> Cancel(int id)
//    {
//        // Only the patient (or admin) can cancel
//        await _appointmentService.CancelAsync(id);
//        return RedirectToAction(nameof(MyAppointments));
//    }
//}
using CareDev.Data;
using CareDev.Models;
using CareDev.Models.ViewModels;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Patient")]
public class PatientAppointmentController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PatientAppointmentController> _logger;

    public PatientAppointmentController(IAppointmentService appointmentService, UserManager<ApplicationUser> userManager,
                                        ApplicationDbContext context, INotificationService notificationService, ILogger<PatientAppointmentController> logger)
    {
        _appointmentService = appointmentService;
        _userManager = userManager;
        _context = context;
        _logger = logger;
        _notificationService = notificationService;
    }

    // GET: Create
    public async Task<IActionResult> Create()
    {
        var vm = new AppointmentCreateViewModel();
        // Load doctors (example: all users in "Doctor" role)
        var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
        vm.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id, Text = d.Name ?? d.UserName });
        // set default times, e.g. next available slot or now+1day
        vm.Start = DateTime.Now.AddDays(1).Date.AddHours(9); // example
        vm.End = vm.Start.AddMinutes(30);
        return View(vm);
    }

    //// POST: Create
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(AppointmentCreateViewModel vm)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        // re-populate Doctors for display
    //        var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
    //        vm.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id, Text = d.Name ?? d.UserName });
    //        return View(vm);
    //    }

    //    // check availability (convert to UTC if needed)
    //    var patientId = _userManager.GetUserId(User);
    //    // optionally check doctor exists
    //    var doctor = await _userManager.FindByIdAsync(vm.DoctorId);
    //    if (doctor == null) { ModelState.AddModelError("", "Selected doctor not found."); return View(vm); }

    //    var startUtc = vm.Start.ToUniversalTime();
    //    var endUtc = vm.End.ToUniversalTime();
    //    var available = await _appointmentService.IsSlotAvailableAsync(vm.DoctorId, startUtc, endUtc);
    //    if (!available)
    //    {
    //        ModelState.AddModelError("", "Selected time is no longer available. Please choose another slot.");
    //        var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
    //        vm.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id, Text = d.Name ?? d.UserName });
    //        return View(vm);
    //    }

    //    // book (creates Pending by default in service code earlier)
    //    await _appointmentService.BookAppointmentAsync(vm.DoctorId, patientId, startUtc, endUtc);

    //    // Optionally notify doctor
    //    if (_notificationService != null)
    //    {
    //        await _notificationService.NotifyAsync(vm.DoctorId, $"New appointment request from {User.Identity.Name} at {vm.Start:f}");
    //    }

    //    // success -> redirect to MyAppointments
    //    return RedirectToAction(nameof(MyAppointments));
    //}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentCreateViewModel vm)
    {
        // repopulate doctors right away so the View always has the select list
        var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
        vm.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id, Text = d.Name ?? d.UserName });

        if (!ModelState.IsValid)
        {
            // Model binding/validation failed (will show validation messages in the view)
            _logger.LogWarning("ModelState invalid for Create appointment: {@ModelState}", ModelState);
            return View(vm);
        }

        try
        {
            var patientId = _userManager.GetUserId(User);
            var doctor = await _userManager.FindByIdAsync(vm.DoctorId);
            if (doctor == null)
            {
                ModelState.AddModelError("", "Selected doctor not found.");
                return View(vm);
            }

            // Validate time ordering
            if (vm.Start >= vm.End)
            {
                ModelState.AddModelError("", "End time must be after start time.");
                return View(vm);
            }

            // convert to UTC if your DB stores UTC
            var startUtc = vm.Start.ToUniversalTime();
            var endUtc = vm.End.ToUniversalTime();

            // check availability
            var available = await _appointmentService.IsSlotAvailableAsync(vm.DoctorId, startUtc, endUtc);
            if (!available)
            {
                ModelState.AddModelError("", "Selected time is no longer available. Please choose another slot.");
                return View(vm);
            }

            await _appointmentService.BookAppointmentAsync(vm.DoctorId, patientId, startUtc, endUtc);

            if (_notificationService != null)
            {
                await _notificationService.NotifyAsync(vm.DoctorId, $"New appointment request from {User.Identity.Name} at {vm.Start:f}");
            }

            TempData["SuccessMessage"] = "Appointment requested successfully.";
            return RedirectToAction(nameof(MyAppointments));
        }
        catch (InvalidOperationException ex)
        {
            // expected business error (slot unavailable, etc.)
            _logger.LogWarning(ex, "Business error creating appointment");
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
        catch (Exception ex)
        {
            // unexpected error — log and show friendly message
            _logger.LogError(ex, "Unexpected error in Create appointment");
            ModelState.AddModelError("", "An unexpected error occurred. Please try again or contact support.");
            return View(vm);
        }
    }


    public async Task<IActionResult> MyAppointments()
    {
        var patientId = _userManager.GetUserId(User);
        var appts = await _appointmentService.GetPatientAppointmentsAsync(patientId);
        return View(appts);
    }
}

