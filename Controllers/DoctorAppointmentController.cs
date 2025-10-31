using CareDev.Models;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CareDev.Models.ApplicationUser;

namespace CareDev.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorAppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger<DoctorAppointmentController> _logger;

        public DoctorAppointmentController(IAppointmentService appointmentService, UserManager<ApplicationUser> userManager, INotificationService notificationService, ILogger<DoctorAppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
            _notificationService = notificationService;
            _logger = logger;
        }

        public IActionResult Calendar() => View();

        //Return events (JSON)
        public async Task<IActionResult> GetDoctorEvents()
        {
            var doctorId = _userManager.GetUserId(User);
            var appointments = await _appointmentService.GetDoctorScheduleAsync(doctorId);

            var events = appointments.Select(a => new
            {
                id = a.Id,
                title = $"{a.Patient?.Name ?? a.Patient?.UserName ?? "Patient"}",
                start = a.Start.ToString("s"),
                end = a.End.ToString("s"),
                status = a.Status.ToString(),
                backgroundColor = a.Status switch
                {
                    AppointmentStatus.Confirmed => "#28a745", // green
                    AppointmentStatus.Pending => "#ffc107",   // yellow
                    AppointmentStatus.Cancelled => "#dc3545", // red
                    _ => "#007bff" // blue default
                }
            });

            return Json(events);
        }

        public async Task<IActionResult> MySchedule()
        {
            var doctorId = _userManager.GetUserId(User);
            var schedule = await _appointmentService.GetDoctorScheduleAsync(doctorId);
            return View(schedule);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Confirm(int appointmentId)
        //{
        //    var appt = await _appointmentService.GetByIdAsync(appointmentId);
        //    if (appt == null) return NotFound();

        //    appt.Status = AppointmentStatus.Confirmed;
        //    await _appointmentService.RescheduleAsync(appointmentId, appt.Start, appt.End); 

        //    await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment at {appt.Start.ToLocalTime():f} is confirmed.");
        //    return RedirectToAction(nameof(MySchedule));
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm([FromBody] AppointmentActionDto dto)
        {
            try
            {
                var docId = _userManager.GetUserId(User);
                var appt = await _appointmentService.GetByIdAsync(dto.AppointmentId);
                if (appt == null) return NotFound("Appointment not found");
                if (appt.DoctorId != docId) return Forbid();

                await _appointmentService.ConfirmAsync(dto.AppointmentId);

                await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment at {appt.Start.ToLocalTime():f} was confirmed.");
                return Ok(new { message = "Confirmed" });
            }
            catch (InvalidOperationException ex)
            {
                // business error
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // unexpected error — return message for debugging
                _logger?.LogError(ex, "Error confirming appointment {Id}", dto.AppointmentId);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule([FromBody] AppointmentRescheduleDto dto)
        {
            try
            {
                var docId = _userManager.GetUserId(User);
                var appt = await _appointmentService.GetByIdAsync(dto.AppointmentId);
                if (appt == null) return NotFound("Appointment not found");
                if (appt.DoctorId != docId) return Forbid();

                // parse/validate dto.NewStartUtc and dto.NewEndUtc are expected as ISO local strings from input
                await _appointmentService.RescheduleAsync(dto.AppointmentId, dto.NewStartUtc.ToUniversalTime(), dto.NewEndUtc.ToUniversalTime());

                await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment has been rescheduled to {dto.NewStartUtc:f}");
                return Ok(new { message = "Rescheduled" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error rescheduling appointment {Id}", dto.AppointmentId);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel([FromBody] ConfirmDto dto)
        {
            var docId = _userManager.GetUserId(User);
            var appt = await _appointmentService.GetByIdAsync(dto.AppointmentId);
            if (appt == null) return NotFound("Appointment not found");
            if (appt.DoctorId != docId) return Forbid();

            await _appointmentService.CancelAsync(dto.AppointmentId);

            // optional: notify patient
            await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment on {appt.Start.ToLocalTime():f} was cancelled by the doctor.");

            return Ok();
        }

        
        //DTOs for cleaner JSON binding
        public class ConfirmDto { public int AppointmentId { get; set; } }
        public class AppointmentActionDto { public int AppointmentId { get; set; } }
        public class AppointmentRescheduleDto
        {
            public int AppointmentId { get; set; }
            public DateTime NewStartUtc { get; set; }
            public DateTime NewEndUtc { get; set; }
        }

    }
}
