using CareDev.Data;
using CareDev.Models;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CareDev.Models.ApplicationUser;

[Authorize(Roles = "Admin")]
public class AdminAppointmentController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly INotificationService _notificationService; // optional
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminAppointmentController(IAppointmentService appointmentService,
                                      INotificationService notificationService,
                                      UserManager<ApplicationUser> userManager,
                                      ApplicationDbContext context)
    {
        _appointmentService = appointmentService;
        _notificationService = notificationService;
        _userManager = userManager;
        _context = context;
    }

    // GET: Index (list) - already implemented elsewhere
    public async Task<IActionResult> Index()
    {
        var all = await _appointmentService.GetAllAppointmentsAsync();
        return View(all);
    }

    // POST: Cancel appointment (Admin)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel([FromBody] CancelRequest req)
    {
        // Basic validation
        var appt = await _context.Appointments.Include(a => a.Patient).Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == req.AppointmentId);
        if (appt == null) return NotFound("Appointment not found");

        appt.Status = AppointmentStatus.Cancelled;
        await _context.SaveChangesAsync();

        // Optional: notify patient & doctor in-app
        if (_notificationService != null)
        {
            await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment on {appt.Start.ToLocalTime():f} was cancelled by admin.");
            await _notificationService.NotifyAsync(appt.DoctorId, $"Appointment with {appt.Patient.Name} on {appt.Start.ToLocalTime():f} was cancelled by admin.");
        }

        return Ok();
    }

    // POST: Reschedule (Admin)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reschedule([FromBody] RescheduleRequest req)
    {
        var appt = await _context.Appointments.Include(a => a.Patient).Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == req.AppointmentId);
        if (appt == null) return NotFound("Appointment not found");

        // parse incoming datetimes (client sends "YYYY-MM-DD HH:mm" / or ISO)
        if (!DateTime.TryParse(req.NewStart, out var newStart)) return BadRequest("Invalid new start time");
        if (!DateTime.TryParse(req.NewEnd, out var newEnd)) return BadRequest("Invalid new end time");
        // optional: convert to UTC based on your app's strategy
        // check availability
        var available = await _appointmentService.IsSlotAvailableAsync(appt.DoctorId, newStart.ToUniversalTime(), newEnd.ToUniversalTime());
        if (!available) return BadRequest("Selected time is not available for this doctor.");

        appt.Start = newStart.ToUniversalTime();
        appt.End = newEnd.ToUniversalTime();
        // keep previous status or set to Confirmed
        appt.Status = AppointmentStatus.Confirmed;
        await _context.SaveChangesAsync();

        if (_notificationService != null)
        {
            await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment was rescheduled to {appt.Start.ToLocalTime():f} by admin.");
            await _notificationService.NotifyAsync(appt.DoctorId, $"Appointment with {appt.Patient.Name} was rescheduled to {appt.Start.ToLocalTime():f} by admin.");
        }

        return Ok();
    }

    // request DTOs
    public class CancelRequest
    {
        public int AppointmentId { get; set; }
    }
    public class RescheduleRequest
    {
        public int AppointmentId { get; set; }
        public string NewStart { get; set; }
        public string NewEnd { get; set; }
    }
}


