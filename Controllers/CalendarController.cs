using CareDev.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace CareDev.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        public CalendarController(IAppointmentService appointmentService) { _appointmentService = appointmentService; }

        public IActionResult Index() => View();

        public async Task<IActionResult> GetEvents()
        {
            var events = (await _appointmentService.GetAllAppointmentsAsync())
                .Select(a => new {
                    id = a.Id,
                    title = $"{a.Patient?.Name ?? a.PatientId}",
                    start = a.Start.ToString("s"),
                    end = a.End.ToString("s"),
                    status = a.Status.ToString()
                });

            return Json(events);
        }
    }
}
