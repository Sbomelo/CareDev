using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareDev.Models;

namespace CareDev.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly INotificationService _notificationService;

        public AdminAppointmentController(IAppointmentService appointmentService, INotificationService notificationService)
        {
            _appointmentService = appointmentService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _appointmentService.GetAllAppointmentsAsync();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel([FromBody] CancelRequest req)
        {
            var appt = await _appointmentService.GetByIdAsync(req.AppointmentId);
            if (appt == null) return NotFound("Appointment not found");

            await _appointmentService.CancelAsync(req.AppointmentId);

            await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment on {appt.Start.ToLocalTime():f} was cancelled.");
            await _notificationService.NotifyAsync(appt.DoctorId, $"Appointment with {appt.Patient.Name} was cancelled.");

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule([FromBody] RescheduleRequest req)
        {
            var appt = await _appointmentService.GetByIdAsync(req.AppointmentId);
            if (appt == null) return NotFound("Appointment not found");

            if (!DateTime.TryParse(req.NewStart, out var newStartLocal) || !DateTime.TryParse(req.NewEnd, out var newEndLocal))
                return BadRequest("Invalid dates");

            var newStartUtc = newStartLocal.ToUniversalTime();
            var newEndUtc = newEndLocal.ToUniversalTime();

            await _appointmentService.RescheduleAsync(req.AppointmentId, newStartUtc, newEndUtc);

            await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment was rescheduled to {newStartLocal:f} by admin.");
            await _notificationService.NotifyAsync(appt.DoctorId, $"Appointment with {appt.Patient.Name} was rescheduled.");

            return Ok();
        }

        public class CancelRequest { public int AppointmentId { get; set; } }
        public class RescheduleRequest { public int AppointmentId { get; set; } public string NewStart { get; set; } public string NewEnd { get; set; } }
    }
}
