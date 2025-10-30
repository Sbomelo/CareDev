// Example controller action to provide JSON events to FullCalendar
using CareDev.Services.IService;
using Microsoft.AspNetCore.Mvc;

public class CalendarController : Controller
{
    private readonly IAppointmentService _appointmentService;
    public CalendarController(IAppointmentService svc) { _appointmentService = svc; }

    public async Task<IActionResult> GetEvents()
    {
        // Return all confirmed appointments (or available slots)
        var allAppts = await _appointmentService.GetAllAppointmentsAsync();
        // Map to FullCalendar format: { id, title, start, end }
        var events = allAppts.Select(a => new {
            id = a.Id,
            title = $"{a.Patient.Name}",
            start = a.Start.ToString("s"),
            end = a.End.ToString("s")
        });
        return Json(events);
    }
}
