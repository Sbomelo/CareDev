using CareDev.Models;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CareDev.Models.ApplicationUser;

namespace YourNamespace.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorAppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public DoctorAppointmentController(IAppointmentService appointmentService, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> MySchedule()
        {
            var doctorId = _userManager.GetUserId(User);
            var schedule = await _appointmentService.GetDoctorScheduleAsync(doctorId);
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int appointmentId)
        {
            var appt = await _appointmentService.GetByIdAsync(appointmentId);
            if (appt == null) return NotFound();

            appt.Status = AppointmentStatus.Confirmed;
            await _appointmentService.RescheduleAsync(appointmentId, appt.Start, appt.End); // reuse Reschedule or add Confirm method

            await _notificationService.NotifyAsync(appt.PatientId, $"Your appointment at {appt.Start.ToLocalTime():f} is confirmed.");
            return RedirectToAction(nameof(MySchedule));
        }
    }
}
