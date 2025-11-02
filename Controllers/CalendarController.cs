using CareDev.Services.IService;
using Microsoft.AspNetCore.Mvc;
using CareDev.Data;
using Microsoft.EntityFrameworkCore;
using static CareDev.Models.ApplicationUser;

namespace CareDev.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ApplicationDbContext _context;
        public CalendarController(IAppointmentService appointmentService, ApplicationDbContext context) 
        { 
            _appointmentService = appointmentService;
            _context = context;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> GetEvents()
        {
            //var events = (await _appointmentService.GetAllAppointmentsAsync())
            //    .Select(a => new {
            //        id = a.Id,
            //        title = $"{a.Patient?.Name ?? a.PatientId}",
            //        start = a.Start.ToString("s"),
            //        end = a.End.ToString("s"),
            //        status = a.Status.ToString()
            //    });

            //return Json(events);


            var events = await _context.Appointments
        .AsNoTracking()
        .Include(a => a.Patient)
        .Include(a => a.Doctor)
        .Where(a => a.Status != AppointmentStatus.Cancelled)
        .OrderBy(a => a.Start)
        .Select(a => new {
            id = a.Id,
            title = a.Patient.Name ?? a.Patient.UserName,
            start = a.Start.ToString("s"),
            end = a.End.ToString("s"),
            status = a.Status.ToString()
        })
        .ToListAsync();

            return Json(events);
        }
    }
}
