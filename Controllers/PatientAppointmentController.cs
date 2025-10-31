using CareDev.Models;
using CareDev.Models.ViewModels;
using CareDev.Services;
using CareDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YourNamespace.Services.Implementation;

namespace CareDev.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientAppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PatientAppointmentController> _logger;

        public PatientAppointmentController(IAppointmentService appointmentService,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<PatientAppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
            _notificationService = notificationService;
            _logger = logger;
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var vm = new AppointmentCreateViewModel
            {
                Start = DateTime.Now.AddDays(1).Date.AddHours(9),
                End = DateTime.Now.AddDays(1).Date.AddHours(9).AddMinutes(30)
            };

            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            vm.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id, Text = d.Name ?? d.UserName }).ToList();
            return View(vm);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel vm)
        {
            // repopulate doctors so view can re-render on errors
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            vm.Doctors = doctors.Select(d => new SelectListItem { Value = d.Id, Text = d.Name ?? d.UserName }).ToList();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState invalid: {@ModelState}", ModelState);
                return View(vm);
            }

            if (vm.Start >= vm.End)
            {
                ModelState.AddModelError("", "End time must be after start time.");
                return View(vm);
            }

            var patientId = _userManager.GetUserId(User);
            var doctor = await _userManager.FindByIdAsync(vm.DoctorId);
            if (doctor == null)
            {
                ModelState.AddModelError("", "Selected doctor not found.");
                return View(vm);
            }

            try
            {
                // convert to UTC for storage (consistent)
                var startUtc = vm.Start.ToUniversalTime();
                var endUtc = vm.End.ToUniversalTime();

                await _appointmentService.BookAppointmentAsync(vm.DoctorId, patientId, startUtc, endUtc, vm.Notes);

                if (_notificationService != null)
                {
                    await _notificationService.NotifyAsync(vm.DoctorId, $"New appointment request from {User.Identity?.Name} at {vm.Start:f}");
                }

                TempData["SuccessMessage"] = "Appointment requested successfully.";
                return RedirectToAction(nameof(MyAppointments));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "BookAppointment failed");
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error booking appointment");
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View(vm);
            }
        }
        public async Task<IActionResult> MyAppointments()
        {
            var patientId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(patientId))
            {
                return RedirectToAction("Login", "Account");
            }

            var appts = (await _appointmentService.GetPatientAppointmentsAsync(patientId)).ToList();

            _logger?.LogDebug("MyAppointments: returning {Count} appointments for user {UserId}", appts.Count, patientId);

            return View(appts);
        }

    }
}
